using Liberty.Cache.Services;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Services;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Settings;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Validations;
using Liberty.SysIntegrationEvent.Events;
using Liberty.UnitOfWork.Abstractions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Liberty.Reservation.Application.Models.Responses.SetRoomsResponse;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Commands.Kakusan;

public class KakusanSetRoomsCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IServiceProvider serviceProvider,
    IBookingRoomAppDateService bookingRoomAppDateService,
    IFacilityRepository facilityRepository,
    IAppDateService appDateService,
    IBus bus
) : UpdateCommandHandlerBase<KakusanSetRoomsCommand, SetRoomsResponse>(unitOfWork, mapper)
{
    private readonly ISecurityContextAccessor _securityContextAccessor =
        serviceProvider.GetRequiredService<ISecurityContextAccessor>();

    private readonly IRoomAdjustmentStatusService _roomAdjustmentService =
        serviceProvider.GetRequiredService<IRoomAdjustmentStatusService>();

    private readonly ICacheService _cacheService = serviceProvider.GetRequiredService<ICacheService>();

    private readonly ICheckFacilityService _checkFacilityService =
        serviceProvider.GetRequiredService<ICheckFacilityService>();

    private readonly IOptions<C004Setting> _c004Options = serviceProvider.GetRequiredService<IOptions<C004Setting>>();

    private const int MaxAllowedDateCount = 5000;

    protected override async Task<SetRoomsResponse> HandleAsync(
        KakusanSetRoomsCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        var userCode = _securityContextAccessor.ApplicationUserKey ?? string.Empty;

        var hotelCodes = payload.Hotels.Select(x => x.HotelId);
        var existingHotelCodes = await facilityRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => hotelCodes.Contains(x.Code) && x.IsEnabled)
            .Select(x => x.Code!)
            .ToArrayAsync(cancellationToken);

        var hotelIdsIsValid = await _checkFacilityService.CheckUserManagedFacilityAsync(
            existingHotelCodes,
            userCode,
            cancellationToken
        );

        var validator = new KakusanSetRoomsCommandValidator(_c004Options.Value);

        var isValid = await validator.ValidateAsync(request, cancellationToken);

        if (!isValid.IsValid || !hotelIdsIsValid)
        {
            return new SetRoomsResponse
            {
                Result = EnumResultType.Error,
                Reasons = []
            };
        }

        var (response, isInvalid) = validator.ValidateRequest(
            request.Payload
        );

        if (isInvalid)
        {
            return response;
        }

        var dateCount = request.Payload.Hotels
            .SelectMany(h => h.GetDates())
            .Count();

        if (dateCount > MaxAllowedDateCount)
        {
            var groupedByRoom = request.Payload.Hotels
                .GroupBy(h => (h.HotelId, h.RoomId))
                .ToDictionary(
                    g => g.Key,
                    g => g.ToList()
                );

            var roomAdjustment = new RoomAdjustmentStatus { TotalCount = groupedByRoom.Count };

            await _roomAdjustmentService.CreateAsync(roomAdjustment, true, cancellationToken);

            var aggregationModel = new RoomGroupAppDateAggregationModel
            {
                Hotels = request.Payload.Hotels,
                RoomAdjustmentCode = roomAdjustment.Code,
                UserCode = userCode
            };

            var eventPayload = new RoomGroupAppDateAggregationEvent();
            eventPayload.SerializeJsonData(aggregationModel);

            await bus.Send(eventPayload, cancellationToken);

            response.Result = EnumResultType.Processing;
            response.Code = roomAdjustment.Code;

            return response;
        }

        var setRoomModel = new SetRoomModel
        {
            Hotels =
            [
                .. payload.Hotels.Select(
                    x => new SetRoomModel.Hotel(
                        x,
                        x.GetDates()
                    )
                )
            ]
        };

        var (updatedSetRoomModel, updateData, addData) = await bookingRoomAppDateService.UpdateSetRoomsAsync(
            setRoomModel,
            cancellationToken
        );

        var allDates = request.Payload.Hotels
            .SelectMany(h => h.GetDates())
            .Distinct()
            .ToList();

        var notCreatedAppDateIds = await appDateService.FindNotCreatedAppDatesAsync(
            [.. allDates],
            cancellationToken
        );

        if (notCreatedAppDateIds is { Count: > 0 })
        {
            await bookingRoomAppDateService.BulkUpsertAppDateAsync(
                notCreatedAppDateIds
            );
        }

        if (updatedSetRoomModel.CanUpdate)
        {
            await bookingRoomAppDateService.BulkUpsertRoomGroupAppDateAsync(
                [.. updateData, .. addData]
            );

            response.Result = EnumResultType.Success;
            await RemoveCacheWhenFinishAsync(request, cancellationToken);
            return response;
        }

        // データ更新しない（更新失敗理由としてデータ矛盾を返す）
        response.Result = EnumResultType.Failure;
        response.Reasons = bookingRoomAppDateService.GetAllReasons(
            updatedSetRoomModel,
            payload.Hotels
        );

        return response;
    }

    private async Task RemoveCacheWhenFinishAsync(
        KakusanSetRoomsCommand request,
        CancellationToken cancellationToken
    )
    {
        var compactUserCode = _securityContextAccessor.CompactApplicationUserKey;

        var facilityCodeCacheKeys = request.Payload.Hotels
            .Select(x => x.HotelId)
            .Distinct()
            .ToList();

        var facilityCacheKeys = await facilityRepository.GetQueryableWithAsNoTracking()
            .Where(x => facilityCodeCacheKeys.Contains(x.Code))
            .Select(x => $"*{string.Format(CacheKeys.FacilityPrefixKey, x.Id)}*")
            .ToArrayAsync(cancellationToken);

        await _cacheService.RemoveByPatternsAsync(
            true,
            [
                .. facilityCacheKeys,
                $"*{string.Format(CacheKeys.KakusanGetAllBookingQueryPrefixKey, compactUserCode, string.Empty)}*",
                $"*{string.Format(CacheKeys.KakusanGetAllRoomQueryPrefixKey, compactUserCode, string.Empty)}*",
                $"*{string.Format(CacheKeys.KakusanGetAllRoomTypeQueryPrefixKey, compactUserCode, string.Empty)}*"
            ]
        );
    }
}
