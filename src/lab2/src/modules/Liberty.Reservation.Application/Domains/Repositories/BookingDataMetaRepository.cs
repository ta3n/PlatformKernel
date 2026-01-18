using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;

namespace Liberty.Reservation.Application.Domains.Repositories;

public class BookingDataMetaRepository(
    IBookingDataAppDateRepository bookingDataAppDateRepository,
    IBookingDataReservationRepository bookingDataReservationRepository,
    IBookingDataPriceRepository bookingDataPriceRepository,
    IBookingDataOptionItemRepository bookingDataOptionItemRepository,
    IBookingDataPersonTypeRepository bookingDataPersonTypeRepository
) : IBookingDataMetaRepository
{
    public async Task<IEnumerable<BookingMetaStandardPriceModel>> GetAllStandardPriceModelsAsync(
        long facilityId,
        long siteId,
        long[] planIds,
        long[] roomGroupIds,
        bool useCache,
        CancellationToken cancellationToken
    )
    {
        return await bookingDataPriceRepository.GetAllStandardPriceModelsAsync(
            facilityId,
            siteId,
            planIds,
            roomGroupIds,
            useCache,
            cancellationToken
        );
    }

    public async Task<IEnumerable<BookingMetaPriceDataModel>> GetAllBookingMetaPriceDataModelsAsync(
        long facilityId,
        long siteId,
        long[] planIds,
        long[] roomGroupIds,
        BookingSearchPlanRequest search,
        CancellationToken cancellationToken
    )
    {
        return await bookingDataPriceRepository.GetAllBookingMetaPriceDataModelsAsync(
            facilityId,
            siteId,
            planIds,
            roomGroupIds,
            search,
            cancellationToken
        );
    }

    public async Task<IEnumerable<BookingMetaDiscountDataModel>> GetAllBookingMetaDiscountDataModelsAsync(
        long facilityId,
        long siteId,
        long[] planIds,
        long[] roomGroupIds,
        bool useCache,
        CancellationToken cancellationToken
    )
    {
        return await bookingDataPriceRepository.GetAllBookingMetaDiscountDataModelsAsync(
            facilityId,
            siteId,
            planIds,
            roomGroupIds,
            useCache,
            cancellationToken
        );
    }

    public async Task<IEnumerable<BookingMetaRangePersonModel>> GetAllRangePersonModelsAsync(
        long facilityId,
        long siteId,
        long[] planIds,
        long[] roomGroupIds,
        bool useCache,
        CancellationToken cancellationToken
    )
    {
        return await bookingDataPriceRepository.GetAllRangePersonModelsAsync(
            facilityId,
            siteId,
            planIds,
            roomGroupIds,
            useCache,
            cancellationToken
        );
    }

    public async Task<IEnumerable<BookingMetaRoomAppDateModel>> GetAllBookingMetaRoomAppDateModelsAsync(
        long facilityId,
        long siteId,
        long[] roomGroupIds,
        BookingSearchPlanRequest search,
        CancellationToken cancellationToken
    )
    {
        return await bookingDataAppDateRepository.GetAllBookingMetaRoomAppDateModelsAsync(
            facilityId,
            siteId,
            roomGroupIds,
            search,
            cancellationToken
        );
    }

    public async Task<IEnumerable<BookingMetaPlanAppDateModel>> GetAllBookingMetaPlanAppDateModelsAsync(
        long facilityId,
        long siteId,
        long[] planIds,
        long[] roomGroupIds,
        BookingSearchPlanRequest search,
        CancellationToken cancellationToken
    )
    {
        return await bookingDataAppDateRepository.GetAllBookingMetaPlanAppDateModelsAsync(
            facilityId,
            siteId,
            planIds,
            roomGroupIds,
            search,
            cancellationToken
        );
    }

    public async Task<IEnumerable<BookingMetaPersonTypeModel>> GetAllBookingMetaPersonTypeModelsAsync(
        long facilityId,
        long siteId,
        long[] planIds,
        long[] roomGroupIds,
        bool facilityStateUseSpaTax,
        bool useCache,
        CancellationToken cancellationToken
    )
    {
        return await bookingDataPersonTypeRepository.GetAllBookingMetaPersonTypeModelsAsync(
            facilityId,
            siteId,
            planIds,
            roomGroupIds,
            facilityStateUseSpaTax,
            useCache,
            cancellationToken
        );
    }

    public async Task<IEnumerable<BookingMetaOptionItemModel>> GetAllBookingMetaOptionItemModelsAsync(
        long facilityId,
        long siteId,
        long[] planIds,
        BookingSearchPlanRequest search,
        long? existingReservationId,
        CancellationToken cancellationToken
    )
    {
        return await bookingDataOptionItemRepository.GetAllBookingMetaOptionItemModelsAsync(
            facilityId,
            siteId,
            planIds,
            search,
            existingReservationId,
            cancellationToken
        );
    }

    public async Task<IEnumerable<BookingMetaReservationModel>> GetAllBookingMetaReservationModelsAsync(
        long facilityId,
        long siteId,
        long[] planIds,
        BookingSearchPlanRequest search,
        CancellationToken cancellationToken
    )
    {
        return await bookingDataReservationRepository.GetAllBookingMetaReservationModelsAsync(
            facilityId,
            siteId,
            planIds,
            search,
            cancellationToken
        );
    }
}
