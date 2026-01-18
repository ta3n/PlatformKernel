using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.SysException.Exceptions;
using Liberty.UnitOfWork.Abstractions;
using PersonAgeTypeEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.PersonAgeType;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.PersonAgeType;

public class PersonAgeTypeCreateCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<PersonAgeTypeCreateCommandHandler> logger,
    IPersonAgeTypeService personAgeTypeService,
    IPersonAgeTypeSpaTaxDataService personAgeTypeSpaTaxDataService
) : CreateCommandHandlerBase<PersonAgeTypeCreateCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PersonAgeTypeCreateCommand request,
        CancellationToken cancellationToken
    )
    {
        var hasPersonAgeGroup = await personAgeTypeService.CheckExistingPersonAgeGroupAsync(
            request.Payload.Meta.GroupName ?? string.Empty,
            cancellationToken: cancellationToken
        );

        if (hasPersonAgeGroup)
        {
            throw new PersonAgeTypeNotfoundException();
        }

        var newPersonAgeType = Mapper.Map<PersonAgeTypeEntity>(request.Payload);
        newPersonAgeType.IsMaster = true;
        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);
            var personAgeTypeCreated = await personAgeTypeService.CreateAsync(
                newPersonAgeType,
                false,
                cancellationToken
            );

            var personAgeTypeSpaTaxDatas = GetPersonAgeTypeSpaTaxDataList(
                personAgeTypeCreated,
                request.Payload.Spas
            );
            await personAgeTypeSpaTaxDataService.CreateRangeAsync(
                personAgeTypeSpaTaxDatas,
                false,
                cancellationToken
            );
            await UnitOfWork.CommitAsync(cancellationToken);

            return personAgeTypeCreated.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Action} {Message}", nameof(PersonAgeTypeCreateCommandHandler), ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }

    private static List<PersonAgeTypeSpaTaxData> GetPersonAgeTypeSpaTaxDataList(
        PersonAgeTypeEntity personAgeType,
        IEnumerable<SpaOfBathingTaxAgeUpdateRequest> spaRequests
    )
    {
        List<PersonAgeTypeSpaTaxData> personAgeTypeSpaTaxDatas = [];
        var spaTaxData = new HashSet<(int, int)>();
        foreach (var personAgeTypeSpaTax in spaRequests)
        {
            var key = (
                personAgeTypeSpaTax.PriceMin ?? 0,
                personAgeTypeSpaTax.PriceMax ?? 0
            );
            if (!spaTaxData.Add(key))
            {
                continue;
            }

            var personAgeTypeSpaTaxData = new PersonAgeTypeSpaTaxData
            {
                PersonAgeType = personAgeType,
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

        return personAgeTypeSpaTaxDatas;
    }
}
