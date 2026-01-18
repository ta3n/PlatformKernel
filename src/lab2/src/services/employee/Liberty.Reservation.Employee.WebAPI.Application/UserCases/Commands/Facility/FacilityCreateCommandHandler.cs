using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.ExternalServices.Membership.Facility.Repositories;
using Liberty.SysException.Exceptions;
using Liberty.UnitOfWork.Abstractions;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Facility;

public class FacilityCreateCommandHandler(
    ILogger<FacilityCreateCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IFacilityService facilityService,
    IFacilityInitDefaultDataService facilityInitDefaultService,
    IFacilityExternalRepository facilityExternalRepository
) : CreateCommandHandlerBase<FacilityCreateCommand, FacilityResponse>(unitOfWork, mapper)
{
    protected override async Task<FacilityResponse> HandleAsync(
        FacilityCreateCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        var facilityExternal = await facilityExternalRepository.GetFacilityByIdAsync(
                payload.Id,
                cancellationToken
            )
            ?? throw new FacilityNotfoundException();

        var facility = new Reservation.Application.Contexts.DataContexts.Entities.Data.Facility
        {
            Id = facilityExternal.Id,
            Code = facilityExternal.Code,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), facilityExternal.Name ?? string.Empty } },
            Kana = facilityExternal.Kana,
            Postcode = facilityExternal.Postcode,
            Meta = new Reservation.Application.Contexts.DataContexts.Entities.Metas.FacilityMeta { SystemEMail = payload.SystemEMail },
            Address1 = new MultilingualText
                {
                    { LanguageHeaderUtil.GetLanguageCodeFromHeader(), facilityExternal.Address1 ?? string.Empty }
                },
            Address2 = new MultilingualText
                {
                    { LanguageHeaderUtil.GetLanguageCodeFromHeader(), facilityExternal.Address2 ?? string.Empty }
                },
            Address3 = new MultilingualText
                {
                    { LanguageHeaderUtil.GetLanguageCodeFromHeader(), facilityExternal.Address3 ?? string.Empty }
                },
            Address4 = new MultilingualText
                {
                    { LanguageHeaderUtil.GetLanguageCodeFromHeader(), facilityExternal.Address4 ?? string.Empty }
                },
            RecordMemo = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
            Memo = payload.Memo,
            CanOnLinePayment = payload.CanOnLinePayment ?? false,
            IsEnabled = payload.IsEnabled ?? false,
            Fax = payload.Fax ?? string.Empty,
            UseFax = payload.UseFax
        };

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var newFacility = await facilityService.CreateAsync(
                facility,
                false,
                cancellationToken
            );

            await facilityInitDefaultService.AddDefaultPersonAgeTypesAsync(
                newFacility,
                false,
                cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            var response = Mapper.Map<FacilityResponse>(newFacility);

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Create facility failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
