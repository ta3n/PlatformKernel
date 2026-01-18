namespace Liberty.Reservation.Manager.Application.Models;

public record BookingDataAvailableModel(
    IEnumerable<PersonAgeType> PersonAgeTypes,
    IEnumerable<OptionItemAppDate> AppDatesOfOptionItems,
    IEnumerable<PlanRoomGroupSiteAppDate> AppDatesOfSiteInPlanRoom,
    IEnumerable<RoomGroupAppDate> AppDatesOfRoom,
    IEnumerable<PlanRoomGroupSitePersonAgeType> PersonAgeTypesOfSiteInPlanRoom,
    IEnumerable<PlanRoomGroupSiteAppDatePriceData> AppDatePriceDataOfSiteInPlanRoom,
    IEnumerable<PlanRoomGroupSiteDiscountData> DiscountDataOfSiteInPlanRoom,
    IEnumerable<ReservationPlanRoomGroupAppDate> AppDatesOfReservations
);
