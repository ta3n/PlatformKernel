using System.Net;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;

public record IdentityResponse(
    HttpStatusCode StatusCode,
    string Context
);
