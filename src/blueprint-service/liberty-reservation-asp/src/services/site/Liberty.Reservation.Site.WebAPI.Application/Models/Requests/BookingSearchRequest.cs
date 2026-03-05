namespace Liberty.Reservation.Site.WebAPI.Application.Models.Requests;

public record BookingSearchRequest : BookingSearchModel
{
    public BookingSearchRequest()
    {
        CheckInDate = AppDate.GetId(DateTime.Today);
    }
}
