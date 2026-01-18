using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Site.WebAPI.Application.ExternalServices.Membership.Facility.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Liberty.Reservation.Site.WebAPI.Application.UserCases.Commands.Booking;

public class AdjustOptionsCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IFacilityExternalRepository facilityExternalRepository,
    IBookingSearchService bookingSearchService,
    IBookingCheckAvailableService bookingCheckAvailableService,
    [FromKeyedServices("plan")] IBookingPriceService bookingPriceOfPlanService,
    [FromKeyedServices("room")] IBookingPriceService bookingPriceOfRoomService
) : ActionCommandHandlerBase<AdjustOptionsCommand, BookingPriceResponse>(unitOfWork, mapper)
{
    protected override async Task<BookingPriceResponse> HandleAsync(
        AdjustOptionsCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var facilityCode = securityContextAccessor.GetFacilityCodeSelected();
        var facilityId = securityContextAccessor.GetFacilityIdSelected();
        var siteId = securityContextAccessor.GetSiteIdSelected();
        var planId = request.PlanId;
        var roomGroupId = request.RoomGroupId;
        var reservationDate = payload.CheckInDate;

        var isFacilityExisting = await facilityExternalRepository.CheckFacilityAvailableAsync(
            facilityCode,
            cancellationToken
        );

        if (!isFacilityExisting)
        {
            throw new FacilityNotfoundException();
        }

        var planAvailableBooking = await bookingSearchService.GetBookingDataDetailByPlanAsync(
                new BookingPlanDetailRequest(
                    facilityId,
                    siteId,
                    planId,
                    roomGroupId,
                    reservationDate,
                    payload.RestNumber,
                    payload.Secret
                ),
                new BookingSearchPlanRequest
                {
                    CheckInDate = payload.CheckInDate,
                    CheckOutDate = payload.GetDateEndNight(),
                    RestNumber = payload.RestNumber,
                    RoomNumber = payload.RoomNumber,
                    Secret = payload.Secret,
                    GuestsPerRoom = payload.GuestsPerRoom.Select(
                        x => new PersonOfBookingSearchModel
                        {
                            AppDateId = x.AppDateId,
                            RestIndex = x.RestIndex,
                            RoomGroupIndex = x.RoomGroupIndex,
                            PersonAgeTypeId = x.PersonAgeTypeId,
                            Persons = x.Persons,
                            MalePersons = x.MalePersons,
                            FemalePersons = x.FemalePersons
                        }
                    ),
                    OptionItems = payload.OptionItems?.Select(
                        x => new OptionOfBookingSearchModel
                        {
                            AppDateId = x.AppDateId,
                            RoomGroupIndex = x.RoomGroupIndex,
                            OptionItemId = x.OptionItemId,
                            Number = x.Number
                        }
                    )
                },
                cancellationToken
            )
            ?? throw new PlanNotfoundException();

        var isInvalidOptionItems = await bookingCheckAvailableService.IsInvalidOptionItemsAsync(
            payload,
            facilityId,
            cancellationToken: cancellationToken
        );

        if (isInvalidOptionItems)
        {
            throw new ReservationOptionException();
        }

        try
        {
            var bookingPriceResponse = planAvailableBooking.PlanType is PlanTypes.Combo
                ? bookingPriceOfPlanService.GetBookingPrice(
                    request.Payload,
                    planAvailableBooking,
                    roomGroupId
                )
                : bookingPriceOfRoomService.GetBookingPrice(
                    request.Payload,
                    planAvailableBooking,
                    roomGroupId
                );

            return bookingPriceResponse!;
        }
        catch (Exception ex) when (ex is PlanNotfoundException or ReservationPriceException)
        {
            throw new ReservationOptionException();
        }
    }
}
