using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.SysException.Exceptions;
using Liberty.UnitOfWork.Abstractions;
using PersonAgeTypeEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.PersonAgeType;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.PersonAgeType;

public class PersonAgeTypeUpdateCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<PersonAgeTypeUpdateCommandHandler> logger,
    IMapper mapper,
    IPersonAgeTypeService personAgeTypeService,
    IPersonAgeTypeSpaTaxDataService personAgeTypeSpaTaxDataService
) : UpdateCommandHandlerBase<PersonAgeTypeUpdateCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PersonAgeTypeUpdateCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var personAgeTypeCount = await personAgeTypeService.CountByIdsAsync(
            [payload.Id ?? 0],
            cancellationToken
        );

        var hasPersonAgeGroup = await personAgeTypeService.CheckExistingPersonAgeGroupAsync(
            request.Payload.Meta.GroupName ?? string.Empty,
            payload.Id ?? 0,
            cancellationToken
        );

        if (personAgeTypeCount is 0 || hasPersonAgeGroup)
        {
            throw new PersonAgeTypeNotfoundException();
        }

        var editPersonAgeType = Mapper.Map<PersonAgeTypeEntity>(payload);
        var existingPersonAgeType = await personAgeTypeService.GetPersonAgeTypeIsMainAsync(
            payload.Id ?? 0,
            cancellationToken
        );

        if (existingPersonAgeType.IsMain && editPersonAgeType.Meta != null)
        {
            editPersonAgeType.Meta.PersonAgeGroup = existingPersonAgeType.Meta?.PersonAgeGroup ?? PersonAgeGroups.Adult;
            editPersonAgeType.IsMain = true;
        }

        List<PersonAgeTypeSpaTaxData> personAgeTypeSpaTaxDatas = [];

        var spaTaxData = new HashSet<(long, int, int)>();
        foreach (var personAgeTypeSpaTax in payload.Spas)
        {
            var key = (
                editPersonAgeType.Id,
                personAgeTypeSpaTax.PriceMin ?? 0,
                personAgeTypeSpaTax.PriceMax ?? 0
            );
            if (!spaTaxData.Add(key))
            {
                continue;
            }

            var personAgeTypeSpaTaxData = new PersonAgeTypeSpaTaxData
            {
                PersonAgeTypeId = editPersonAgeType.Id,
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
            var personAgeTypeUpdated = await personAgeTypeService.UpdateAsync(
                editPersonAgeType,
                false,
                cancellationToken: cancellationToken
            );

            await personAgeTypeSpaTaxDataService.ChangeSpaTaxDataOfPersonAgeTypeAsync(
                editPersonAgeType.Id,
                personAgeTypeSpaTaxDatas,
                false,
                cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            return personAgeTypeUpdated.Id;
        }
        catch
            (Exception ex)
        {
            logger.LogError(ex, "{Action} {Message}", nameof(PersonAgeTypeUpdateCommandHandler), ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
