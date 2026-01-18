using System.Net;

namespace Liberty.Reservation.Site.Public.WebAPI.Models;

public class HttpResponseModel
{
    public HeaderDictionary Headers { get; set; } = [];
    public byte[] Content { get; set; } = [];
    public HttpStatusCode StatusCode { get; set; }
}
