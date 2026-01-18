namespace Liberty.Reservation.Manager.WebAPI.Application.Mappings;

public class OptionItemInventoryMapperProfile : Profile
{
    public OptionItemInventoryMapperProfile()
    {
        CreateMap<OptionItemAppDate, OptionItemAppDateResponse>()
            .ConstructUsing(
                src => new OptionItemAppDateResponse(
                    src.AppDateId,
                    src.OptionItemId,
                    src.OptionItem!.Name!.GetValueByHeader(),
                    src.SellNumber ?? 0,
                    src.IsNotSelled
                )
            )
            .ForMember(
                des => des.ReservedNumber,
                opt =>
                    opt.MapFrom(
                        src =>
                            src.OptionItem!.ReservationRoomGroupAppDateOptionItems!
                                .Where(x => x.BookingDateId == src.AppDateId)
                                .Where(
                                    x =>
                                        x.Reservation!.ReservationState == ReservationStatus.Confirmed
                                        || x.Reservation!.ReservationState == ReservationStatus.Reserved
                                        || x.Reservation!.ReservationState == ReservationStatus.Modified
                                )
                                .Sum(x => x.Number)
                    )
            );
    }
}
