namespace Liberty.Reservation.Employee.WebAPI.Application.Mappings;

public class MasterCalendarMapperProfile : Profile
{
    public MasterCalendarMapperProfile()
    {
        CreateMap<AppDate, DateOfMasterCalendarResponse>()
            .ConstructUsing(
                src => new DateOfMasterCalendarResponse(
                    src.Id,
                    src.AppDateAppDateTypes!.FirstOrDefault()!.AppDateType!.Name,
                    src.AppDateAppDateTypes!.FirstOrDefault()!.AppDateType!.ShortName,
                    src.AppDateAppDateTypes!.FirstOrDefault()!.AppDateType!.Color,
                    src.AppDateAppDateDatas!.FirstOrDefault()!.AppDateData!.Name
                )
            );
    }
}
