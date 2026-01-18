namespace Liberty.Reservation.Site.Public.WebAPI.Models.Requests;

public record BookingSearchPlanRequest : BookingSearchModel
{
    public BookingSearchPlanRequest()
    {
        CheckInDate = 0;
    }
}
