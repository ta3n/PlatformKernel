using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.User.Application.Domains.Services.Interfaces;

public interface IFacilityService : IBaseService<Facility>
{
    Task<bool> CheckPaymentOnSitePaymentAvailableAsync(
        long facilityId,
        CancellationToken cancellationToken
    );
}
