using System.Security.Cryptography;
using System.Text;
using Liberty.Reservation.Booking.Worker.Application.Options;
using Liberty.Reservation.Manager.Application.Models;
using Liberty.SysIntegrationEvent;
using Liberty.SysIntegrationEvent.Events;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Liberty.Reservation.Booking.Worker.Application.UserCases.Commands.BookingSearch;

public class BookingSearchPrecomputeCommandHandler(
    ILogger<BookingSearchPrecomputeCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IFacilityService facilityService,
    IBus bus,
    IOptions<BookingSearchPrePrecomputeOption> bookingSearchPrePrecomputeOption
) : ActionCommandHandlerBase<BookingSearchPrecomputeCommand, bool>(unitOfWork, mapper)
{
    private const int DefaultPageSize = 200;

    protected override async Task<bool> HandleAsync(
        BookingSearchPrecomputeCommand request,
        CancellationToken cancellationToken
    )
    {
        var currentPageIndex = 1;

        while (true)
        {
            var facilities = await facilityService.FindAllAvailableFacilitiesOfPrecomputeAsync(
                Pageable.Of(currentPageIndex, DefaultPageSize, true),
                cancellationToken
            );

            await ExecuteFacilityBookingSearchAsync(
                facilities.Content,
                cancellationToken
            );

            if (facilities.HasNext)
            {
                currentPageIndex++;
            }
            else
            {
                break;
            }
        }

        return true;
    }

    private async Task ExecuteFacilityBookingSearchAsync(
        IEnumerable<FacilityAvailableModel> facilities,
        CancellationToken cancellationToken
    )
    {
        var facilityAvailableModels = facilities as FacilityAvailableModel[] ?? [.. facilities];
        if (facilityAvailableModels.Length == 0)
        {
            logger.LogWarning("No facilities found for precomputation.");

            return;
        }

        var facilitySiteFlatAvailableModels = FlattenFacilities(facilityAvailableModels).ToList();

        foreach (var facilitySiteFlatAvailableModel in facilitySiteFlatAvailableModels)
        {
            var personAgeTypes = facilitySiteFlatAvailableModel.PersonAgeTypes;
            if (personAgeTypes == null)
            {
                continue;
            }

            var personAgeTypeOfFacilityAvailableModels = personAgeTypes as PersonAgeTypeOfFacilityAvailableModel[] ?? [.. personAgeTypes];
            if (personAgeTypeOfFacilityAvailableModels is not { Length: > 0 })
            {
                continue;
            }

            var partitionCount = bookingSearchPrePrecomputeOption.Value.PartitionCount;

            var shardIndex = GetQueueShardIndex(
                facilitySiteFlatAvailableModel.FacilityId,
                partitionCount
            );

            var partitionEvent = new BookingAuditLogPartitionEvent();
            partitionEvent.SerializeJsonData(facilitySiteFlatAvailableModel);

            var queueName = $"{ReservationQueues.BookingSearchPrecomputePartitionQueue}{shardIndex}";
            var endpoint = await bus.GetSendEndpoint(new Uri($"queue:{queueName}"));

            await endpoint.Send(partitionEvent, cancellationToken);
        }
    }

    private static IEnumerable<FacilitySiteFlatAvailableModel> FlattenFacilities(
        IEnumerable<FacilityAvailableModel> facilities
    )
    {
        return facilities
            .Where(x => x.Sites != null)
            .SelectMany(
                facility => facility.Sites?
                        .Select(
                            site => new FacilitySiteFlatAvailableModel(
                                facility.Id,
                                facility.Code,
                                site.Id,
                                site.Code,
                                facility.PersonAgeTypes
                            )
                        )
                    ?? []
            );
    }

    private static int GetQueueShardIndex(
        long facilityId,
        int partitionCount
    )
    {
        var composite = $"{facilityId}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(composite));

        var hashCode = hash.Aggregate(
            0,
            (
                current,
                t
            ) => (current * 31) ^ t
        );

        hashCode &= 0x7FFFFFFF;

        var shardIndex = hashCode % partitionCount;

        return shardIndex;
    }
}
