namespace Liberty.Reservation.Manager.WebAPI.Application.Mappings;

public class RoomGroupInventoryMapperProfile : Profile
{
    public RoomGroupInventoryMapperProfile()
    {
        CreateMap<RoomGroupAppDate, RoomGroupAppDateResponse>()
            .ConstructUsing(
                src => new RoomGroupAppDateResponse(
                    src.AppDateId,
                    src.RoomGroupId,
                    src.RoomGroup!.Name!.GetValueByHeader(),
                    src.SellNumber ?? 0,
                    src.IsNotSelled,
                    src.RoomGroup!.BaseNumber,
                    src.RoomGroup!.GroupName
                )
            )
            .ForMember(
                des => des.ReservedNumber,
                opt =>
                    opt.MapFrom(
                        src =>
                            src.RoomGroup!.ReservationPlanRoomGroupAppDates!
                                .Where(x => x.BookingDateId == src.AppDateId)
                                .Count(
                                    x =>
                                        x.Reservation!.ReservationState == ReservationStatus.Confirmed
                                        || x.Reservation!.ReservationState == ReservationStatus.Reserved
                                        || x.Reservation!.ReservationState == ReservationStatus.Modified
                                )
                    )
            );
    }
}
