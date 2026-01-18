using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BathingTaxAge;

public class BathingTaxAgeUpdateCommandHandler(
    ILogger<BathingTaxAgeUpdateCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IFacilityPersonAgeTypeService facilityPersonAgeTypeService,
    IPersonAgeTypeSpaTaxDataService personAgeTypeSpaTaxDataService,
    IFacilityService facilityService,
    IPlanService planService
) : UpdateCommandHandlerBase<BathingTaxAgeUpdateCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        BathingTaxAgeUpdateCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        var facilityId = securityContextAccessor.FacilityKey;

        var existingFacilityCount = await facilityService.CountByIdsAsync(
            [facilityId],
            cancellationToken
        );
        if (existingFacilityCount == 0)
        {
            throw new FacilityNotfoundException();
        }

        if (!payload.IsEnabled)
        {
            await CheckPlanHasBeenSetup(false, cancellationToken);
        }

        var facilityPersonAgeType = await facilityPersonAgeTypeService.FindByFacilityIdAsync(
                facilityId,
                payload.Id,
                cancellationToken
            )
            ?? throw new FacilityPersonAgeTypeNotfoundException();

        List<PersonAgeTypeSpaTaxData> personAgeTypeSpaTaxDatas = [];
        facilityPersonAgeType.PersonAgeType!.AgeMin = payload.AgeMin;
        facilityPersonAgeType.PersonAgeType!.AgeMax = payload.AgeMax;
        facilityPersonAgeType.PersonAgeType!.Name ??= [];
        facilityPersonAgeType.PersonAgeType!.Name?.UpdateLocalized(
            new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), payload.Name ?? string.Empty } }
        );
        // facilityPersonAgeType.PersonAgeType!.Meta = new PersonAgeTypeMeta
        // {
        //     FoodBed = (int)payload.Meta.Food + payload.Meta.Bed,
        //     PersonAgeGroup = payload.Meta.PersonAgeGroup,
        //     GroupName = payload.Meta.GroupName
        // };
        facilityPersonAgeType.PersonAgeType!.IsEnabled = payload.IsEnabled;

        foreach (var personAgeTypeSpaTax in payload.Spas)
        {
            var personAgeTypeSpaTaxData = new PersonAgeTypeSpaTaxData
            {
                PersonAgeTypeId = facilityPersonAgeType.PersonAgeTypeId,
                SpaTaxData = new SpaTaxData
                {
                    PriceMax = personAgeTypeSpaTax.PriceMax,
                    PriceMin = personAgeTypeSpaTax.PriceMin,
                    Tax = personAgeTypeSpaTax.Tax,
                    IsEnabled = true
                }
            };
            personAgeTypeSpaTaxDatas.Add(personAgeTypeSpaTaxData);
        }

        try

        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);
            await facilityPersonAgeTypeService.UpdateAsync(
                facilityPersonAgeType,
                false,
                cancellationToken
            );

            await personAgeTypeSpaTaxDataService.ChangeSpaTaxDataOfPersonAgeType(
                payload.Id,
                facilityId,
                personAgeTypeSpaTaxDatas,
                false,
                cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            return facilityId;
        }
        catch
            (Exception ex)
        {
            logger.LogError(ex, "{Action} {Message}", nameof(BathingTaxAgeUpdateCommandHandler), ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }

    private async Task CheckPlanHasBeenSetup(
        bool isRequirePlanSetupCheck,
        CancellationToken cancellationToken
    )
    {
        if (!isRequirePlanSetupCheck)
        {
            return;
        }

        var anyPlans = await planService.AnyAsync(cancellationToken);
        if (anyPlans)
        {
            throw new PlanHasBeenSetUpException();
        }
    }
}
