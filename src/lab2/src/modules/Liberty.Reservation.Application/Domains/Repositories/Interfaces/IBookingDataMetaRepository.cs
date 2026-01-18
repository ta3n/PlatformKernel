namespace Liberty.Reservation.Application.Domains.Repositories.Interfaces;

/// <summary>
/// Interface that aggregates multiple booking data repository interfaces.
/// Provides a unified contract for accessing various booking-related data,
/// including application dates, reservations, prices, option items, and person types.
/// </summary>
public interface IBookingDataMetaRepository :
    IBookingDataAppDateRepository,
    IBookingDataReservationRepository,
    IBookingDataPriceRepository,
    IBookingDataOptionItemRepository,
    IBookingDataPersonTypeRepository;
