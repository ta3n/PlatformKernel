using System.Reflection;
using System.Text;
using Liberty.Reservation.Site.Application.Models;
using Liberty.SysException.Exceptions;
using Newtonsoft.Json;

namespace Liberty.Reservation.Site.WebAPI.Application.UserCases.Commands.Booking;

public class CheckChangedCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IOptionItemRepository optionItemRepository,
    ICheckChangedService checkChangedService
) : ActionCommandHandlerBase<CheckChangedCommand, bool>(unitOfWork, mapper)
{
    protected override async Task<bool> HandleAsync(
        CheckChangedCommand request,
        CancellationToken cancellationToken
    )
    {
        var lastUpdatedObject = await checkChangedService.GetLastUpdatedAtAsync(
            request.PlanId,
            request.RoomGroupId,
            cancellationToken
        );

        var decodeOfPayload = Convert.FromBase64String(request.Payload.LastUpdatedString!);
        var jsonOfPayload = Encoding.UTF8.GetString(decodeOfPayload);
        LastUpdatedTimeOfBoookingModel? dtoOfPayload;
        try
        {
            dtoOfPayload = JsonConvert.DeserializeObject<LastUpdatedTimeOfBoookingModel>(jsonOfPayload);
        }
        catch (Exception)
        {
            throw new LastUpdateStringInvalidException();
        }

        var properties = typeof(LastUpdatedTimeOfBoookingModel).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var value1 = property.GetValue(lastUpdatedObject);
            var json1 = JsonConvert.SerializeObject(value1);
            var value2 = property.GetValue(dtoOfPayload);
            var json2 = JsonConvert.SerializeObject(value2);

            // Check if values are different
            if (json1 != json2)
            {
                throw property.Name switch
                {
                    nameof(LastUpdatedTimeOfBoookingModel.FacilityUpdatedAt) => new FacilityHasChangesException(),
                    nameof(LastUpdatedTimeOfBoookingModel.SiteUpdatedAt) => new SiteHasChangesException(),
                    nameof(LastUpdatedTimeOfBoookingModel.PlanUpdatedAt) => new PlanHasChangesException(),
                    nameof(LastUpdatedTimeOfBoookingModel.CancellationUpdatedAt) => new CancellationHasChangesException(),
                    nameof(LastUpdatedTimeOfBoookingModel.RoomGroupUpdatedAt) => new RoomGroupHasChangesException(),
                    nameof(LastUpdatedTimeOfBoookingModel.QuestionsUpdatedAt) => new QuestionHasChangesException(),
                    nameof(LastUpdatedTimeOfBoookingModel.FilesUpdatedAt) => new FileHasChangesException(),
                    nameof(LastUpdatedTimeOfBoookingModel.PersonAgeTypeUpdatedAt) => new PersonAgeTypeHasChangesException(),
                    _ => throw new AppLibertyException("Unknown property")
                };
            }
        }

        foreach (var item in request.Payload.OptionItems)
        {
            var optionUpdatedTime = await optionItemRepository.GetQueryableWithAsNoTracking()
                .Where(x => x.Id == item.Id)
                .Select(x => x.UpdatedAt)
                .SingleOrDefaultAsync(cancellationToken);
            if (!long.TryParse(item.LastUpdatedAt, out var lastUpdatedAtLong))
            {
                throw new OptionItemUpdatedTimeInvalidException();
            }

            if (optionUpdatedTime != lastUpdatedAtLong)
            {
                throw new OptionItemHasChangesException();
            }
        }

        return true;
    }
}
