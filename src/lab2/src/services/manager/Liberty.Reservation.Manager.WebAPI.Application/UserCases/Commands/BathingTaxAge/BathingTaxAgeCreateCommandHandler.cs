using Liberty.ApplicationShared.Utils;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BathingTaxAge;

public class BathingTaxAgeCreateCommandHandler(
    ILogger<BathingTaxAgeCreateCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IFacilityPersonAgeTypeService facilityPersonAgeTypeService,
    IPersonAgeTypeSpaTaxDataService personAgeTypeSpaTaxDataService,
    IFacilityService facilityService
) : CreateCommandHandlerBase<BathingTaxAgeCreateCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        BathingTaxAgeCreateCommand request,
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

        var newFacilityPersonAgeType = new FacilityPersonAgeType
        {
            FacilityId = facilityId,
            PersonAgeType = new PersonAgeType
            {
                Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), payload.Name ?? string.Empty } },
                Code = EntityUtil.CreateCode(),
                AgeMax = payload.AgeMax,
                AgeMin = payload.AgeMin,
                DisplayOrder = payload.DisplayOrder ?? AppDate.GetId(DateTime.UtcNow),
                IsEnabled = payload.IsEnabled,
                Meta = new PersonAgeTypeMeta
                {
                    FoodBed = (int)payload.Meta.Food + payload.Meta.Bed,
                    PersonAgeGroup = payload.Meta.PersonAgeGroup,
                    GroupName = payload.Meta.GroupName
                }
            }
        };
        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);
            var facilityPersonAgeType = await facilityPersonAgeTypeService.CreateAsync(
                newFacilityPersonAgeType,
                true,
                cancellationToken
            );
            List<PersonAgeTypeSpaTaxData> personAgeTypeSpaTaxDatas = [];
            foreach (var personAgeTypeSpaTax in payload.Spas)
            {
                var personAgeTypeSpaTaxData = new PersonAgeTypeSpaTaxData
                {
                    PersonAgeTypeId = facilityPersonAgeType.PersonAgeTypeId,
                    SpaTaxData = new SpaTaxData
                    {
                        Code = EntityUtil.CreateCode(),
                        PriceMax = personAgeTypeSpaTax.PriceMax,
                        PriceMin = personAgeTypeSpaTax.PriceMin,
                        Tax = personAgeTypeSpaTax.Tax,
                        IsEnabled = true
                    }
                };
                personAgeTypeSpaTaxDatas.Add(personAgeTypeSpaTaxData);
            }

            await personAgeTypeSpaTaxDataService.CreateRangeAsync(
                personAgeTypeSpaTaxDatas,
                false,
                cancellationToken
            );
            await UnitOfWork.CommitAsync(cancellationToken);

            return facilityPersonAgeType.PersonAgeTypeId;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Action} {Message}", nameof(BathingTaxAgeCreateCommandHandler), ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
