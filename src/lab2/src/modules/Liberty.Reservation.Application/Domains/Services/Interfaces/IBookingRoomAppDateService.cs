using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using static Liberty.Reservation.Application.Models.Responses.SetRoomsResponse;

namespace Liberty.Reservation.Application.Domains.Services.Interfaces;

/// <summary>
/// Provides methods for retrieving room application dates for booking operations.
/// </summary>
/// <remarks>
/// Implementations of this interface are responsible for fetching available room application dates
/// based on booking criteria such as booking request and plan details.
/// </remarks>
/// <example>
/// <code>
/// var roomAppDates = service.GetAllRoomAppDates(request, plan);
/// </code>
/// </example>
public interface IBookingRoomAppDateService
{
    IEnumerable<BookingRoomAppDateModel> GetAllRoomAppDates(
        BookingCreateRequest bookingCreateRequest,
        BookingPlanModel bookingPlanAvailable
    );

    Task<List<RoomGroupAppDate>> GetAppDatesOfRoomAsync(
        SetRoomModel.Hotel hotel,
        CancellationToken cancellationToken
    );

    Task<RoomGroup?> GetRoomGroupByGroupNameAndFacility(
        string? groupName,
        string? facilityCode,
        CancellationToken cancellationToken = default
    );

    Task<Dictionary<(string roomId, string hotelId), (long roomGroupId, long facilityId)>> GetRoomGroupFacilityIdMapAsync(
        Dictionary<string, string> roomHotelDict,
        CancellationToken cancellationToken = default
    );

    Task<(SetRoomModel, List<RoomGroupAppDate>, List<RoomGroupAppDate>)>
        UpdateSetRoomsAsync(
            SetRoomModel roomGroupModel,
            CancellationToken cancellationToken = default
        );

    List<Reason> GetAllReasons(
        SetRoomModel setRoomModel,
        List<HotelModel> hotels
    );

    Task BulkUpsertRoomGroupAppDateAsync(
        List<RoomGroupAppDate> roomGroupAppDates
    );

    Task BulkUpsertAppDateAsync(
        List<long> appDateIds
    );
}
