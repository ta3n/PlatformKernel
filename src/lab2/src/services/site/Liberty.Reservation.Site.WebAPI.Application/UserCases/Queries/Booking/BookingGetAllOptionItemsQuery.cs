namespace Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Booking;

public record BookingGetAllOptionItemsQuery(
    long PlanId,
    BookingOptionRequest Payload,
    IPageable Pageable
) : IQueryPagedBase<OptionItemOfBookingResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
