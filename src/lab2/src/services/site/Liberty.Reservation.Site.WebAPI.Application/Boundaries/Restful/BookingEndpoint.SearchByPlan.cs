using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Site.WebAPI.Application.UserCases.Commands.Booking;
using Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Booking;
using Microsoft.Extensions.DependencyInjection;

namespace Liberty.Reservation.Site.WebAPI.Application.Boundaries.Restful;

public partial class BookingEndpoint
{
    private async Task CheckBookingByComboAsync(
        long[] planIds,
        long[] roomGroupIds,
        CancellationToken cancellationToken
    )
    {
        var isAvailable = await planRoomGroupService.IsAvailablePlanWithRoomAsync(
            planIds,
            roomGroupIds,
            securityContextAccessor.GetFacilityIdSelected(),
            PlanTypes.Combo,
            cancellationToken
        );
        if (isAvailable is false)
        {
            throw new PlanNotfoundException();
        }
    }

    [HttpPost("search")]
    [ProducesResponseType(typeof(List<BookingSearchByPlanResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchBooking(
        [FromQuery] IPageable pageable,
        [FromBody] BookingSearchPlanRequest request,
        CancellationToken cancellationToken
    )
    {
        request = request with
        {
            GuestsPerRoom = await LoadGuestsPerRoomAsync(request.GuestsPerRoom?.ToList() ?? []),
            OptionItems = await LoadOptionItemsAsync(request.OptionItems?.ToList() ?? [])
        };

        var (headers, response) = await Mediator!.Send(
            new BookingSearchByPlanQuery(request, pageable),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);

        async Task<IEnumerable<PersonOfBookingSearchModel>?> LoadGuestsPerRoomAsync(
            List<PersonOfBookingSearchModel> data
        )
        {
            if (data is not { Count: > 0 })
            {
                return null;
            }

            if (data.TrueForAll(x => string.IsNullOrEmpty(x.PersonAgeTypeCode)))
            {
                return data;
            }

            var facilityPersonAgeTypeRepository = serviceProvider.GetRequiredService<IFacilityPersonAgeTypeRepository>();

            var personAgeTypeCodes = data.Select(x => x.PersonAgeTypeCode).ToArray();

            var existingPersonAgeTypes = await facilityPersonAgeTypeRepository
                .GetQueryableWithAsNoTracking()
                .Where(x => personAgeTypeCodes.Contains(x.PersonAgeType!.Code))
                .Select(
                    x => new
                    {
                        x.PersonAgeType!.Id,
                        x.PersonAgeType!.Code
                    }
                )
                .ToListAsync(cancellationToken);

            foreach (var item in data)
            {
                item.PersonAgeTypeId = existingPersonAgeTypes.Find(x => x.Code == item.PersonAgeTypeCode)?.Id
                    ?? 0;
            }

            return data;
        }

        async Task<IEnumerable<OptionOfBookingSearchModel>?> LoadOptionItemsAsync(
            List<OptionOfBookingSearchModel> data
        )
        {
            if (data is not { Count: > 0 })
            {
                return null;
            }

            if (data.TrueForAll(x => string.IsNullOrEmpty(x.OptionItemCode)))
            {
                return data;
            }

            var optionItemRepository = serviceProvider.GetRequiredService<IOptionItemRepository>();

            var optionItemCodes = data.Select(x => x.OptionItemCode).ToArray();

            var existingOptionItems = await optionItemRepository
                .GetQueryableWithAsNoTracking()
                .Where(x => optionItemCodes.Contains(x.Code))
                .Select(
                    x => new
                    {
                        x.Id,
                        x.Code
                    }
                )
                .ToListAsync(cancellationToken);

            foreach (var item in data)
            {
                item.OptionItemId = existingOptionItems.Find(x => x.Code == item.OptionItemCode)?.Id
                    ?? 0;
            }

            return data;
        }
    }

    [HttpGet("plans/{planId:min(1)}/rooms/{roomGroupId:min(1)}")]
    [ProducesResponseType(typeof(BookingDetailsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBookingDetails(
        [FromRoute] long planId,
        [FromRoute] long roomGroupId,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new BookingGetDetailsQuery(
                planId,
                roomGroupId
            ),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpPost("plans/{planId:min(1)}/rooms/{roomGroupId:min(1)}/price-calendar")]
    [ProducesResponseType(typeof(PriceCalendarOfRoomResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBookingPriceCalendar(
        [FromRoute] long planId,
        [FromRoute] long roomGroupId,
        [FromBody] BookingSearchPlanRequest request,
        CancellationToken cancellationToken
    )
    {
        await CheckBookingByComboAsync(
            [planId],
            [roomGroupId],
            cancellationToken
        );

        var (headers, response) = await Mediator!.Send(
            new BookingGetCalendarPricesQuery(
                planId,
                roomGroupId,
                request
            ),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpPost("plans/{planId:min(1)}/rooms/{roomGroupId:min(1)}/check-night-number")]
    [ProducesResponseType(typeof(BookingPriceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckNightNumber(
        [FromRoute] long planId,
        [FromRoute] long roomGroupId,
        [FromBody] BookingPriceRequest request,
        CancellationToken cancellationToken
    )
    {
        await CheckBookingByComboAsync(
            [planId],
            [roomGroupId],
            cancellationToken
        );

        var response = await Mediator!.Send(
            new BookingCheckNightNumberCommand(
                planId,
                roomGroupId
            ) { Payload = request },
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response);
    }

    [HttpPost("plans/{planId:min(1)}/rooms/{roomGroupId:min(1)}/check-room-number")]
    [ProducesResponseType(typeof(BookingPriceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckRoomNumber(
        [FromRoute] long planId,
        [FromRoute] long roomGroupId,
        [FromBody] BookingPriceRequest request,
        CancellationToken cancellationToken
    )
    {
        await CheckBookingByComboAsync(
            [planId],
            [roomGroupId],
            cancellationToken
        );

        var response = await Mediator!.Send(
            new BookingCheckRoomNumberCommand(
                planId,
                roomGroupId
            ) { Payload = request },
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response);
    }

    [HttpGet("plans/{planId:min(1)}/option-items")]
    [ProducesResponseType(typeof(List<OptionItemOfBookingResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllOptionItemsOfPlan(
        [FromRoute] long planId,
        [FromQuery] BookingOptionRequest request,
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new BookingGetAllOptionItemsQuery(
                planId,
                request,
                pageable
            ),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpPost("plans/{planId:min(1)}/rooms/{roomGroupId:min(1)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateBookingAsync(
        [FromRoute] long planId,
        [FromRoute] long roomGroupId,
        [FromBody] SiteBookingCreateRequest request,
        CancellationToken cancellationToken
    )
    {
        await CheckBookingByComboAsync(
            [planId],
            [roomGroupId],
            cancellationToken
        );

        var response = await Mediator!.Send(
            new BookingCreateCommand(
                planId,
                roomGroupId
            ) { Payload = request },
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response)
            .WithHeaders(
                HeaderUtil.CreateEntityCreationAlert(
                    nameof(BookingCreateRequest),
                    response
                )
            );
    }

    [HttpPost("plans/{planId:min(1)}/rooms/{roomGroupId:min(1)}/change-persons")]
    [ProducesResponseType(typeof(BookingPriceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ChangePersonsAsync(
        [FromRoute] long planId,
        [FromRoute] long roomGroupId,
        [FromBody] BookingPriceRequest request,
        CancellationToken cancellationToken
    )
    {
        await CheckBookingByComboAsync(
            [planId],
            [roomGroupId],
            cancellationToken
        );

        var response = await Mediator!.Send(
            new ChangePersonsBookingCommand(
                planId,
                roomGroupId
            ) { Payload = request },
            cancellationToken
        );
        return ActionResultUtil.WrapOrNotFound(response);
    }

    [HttpPost("plans/{planId:min(1)}/rooms/{roomGroupId:min(1)}/adjust-options")]
    [ProducesResponseType(typeof(BookingPriceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> AdjustOptionsAsync(
        [FromRoute] long planId,
        [FromRoute] long roomGroupId,
        [FromBody] BookingPriceRequest request,
        CancellationToken cancellationToken
    )
    {
        await CheckBookingByComboAsync(
            [planId],
            [roomGroupId],
            cancellationToken
        );

        var response = await Mediator!.Send(
            new AdjustOptionsCommand(
                planId,
                roomGroupId
            ) { Payload = request },
            cancellationToken
        );
        return ActionResultUtil.WrapOrNotFound(response);
    }

    [HttpPost("plans/{planId:min(1)}/rooms/{roomGroupId:min(1)}/check-changed")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckChangedAsync(
        [FromRoute] long planId,
        [FromRoute] long roomGroupId,
        [FromBody] CheckChangedRequest request,
        CancellationToken cancellationToken
    )
    {
        await CheckBookingByComboAsync(
            [planId],
            [roomGroupId],
            cancellationToken
        );

        var response = await Mediator!.Send(
            new CheckChangedCommand(
                planId,
                roomGroupId
            ) { Payload = request },
            cancellationToken
        );
        return ActionResultUtil.WrapOrNotFound(response);
    }

    [HttpGet("/plans/{planCode}/room-groups/{roomGroupCode}")]
    [ProducesResponseType(typeof(BookingPlanDetailResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlanDetails(
        [FromRoute] string planCode,
        [FromRoute] string roomGroupCode,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
             new BookingGetPlanByCodeQuery(
                 planCode,
                 roomGroupCode
             ),
             cancellationToken
         );

        var planResponse = Mapper.Map<BookingPlanDetailResponse>(response);
        return ActionResultUtil.WrapOrNotFound(planResponse).WithHeaders(headers);
    }

    [HttpGet("/plans/{planCode}/room-groups/{roomGroupCode}/room")]
    [ProducesResponseType(typeof(BookingRoomGroupResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoomGroupDetails(
        [FromRoute] string planCode,
        [FromRoute] string roomGroupCode,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
             new BookingGetPlanByCodeQuery(
                 planCode,
                 roomGroupCode
             ),
             cancellationToken
         );
         var roomResponse = Mapper.Map<BookingRoomGroupResponse>(response);
        return ActionResultUtil.WrapOrNotFound(roomResponse).WithHeaders(headers);
    }

    [HttpGet("/plans/{planCode}/room-groups/{roomGroupCode}/cancellation-policy")]
    [ProducesResponseType(typeof(BookingCancellationResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCancellationPolicy(
        [FromRoute] string planCode,
        [FromRoute] string roomGroupCode,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
             new BookingGetPlanByCodeQuery(
                 planCode,
                 roomGroupCode
             ),
             cancellationToken
         );

        var cancellationResponse = Mapper.Map<BookingCancellationResponse>(response);
        return ActionResultUtil.WrapOrNotFound(cancellationResponse).WithHeaders(headers);
    }

    [HttpGet("/plans/{planCode}/room-groups/{roomGroupCode}/special-notes")]
    [ProducesResponseType(typeof(BookingSpecialNoteResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSpecialNotes(
        [FromRoute] string planCode,
        [FromRoute] string roomGroupCode,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
             new BookingGetPlanByCodeQuery(
                 planCode,
                 roomGroupCode
             ),
             cancellationToken
         );

        var noteResponse = Mapper.Map<BookingSpecialNoteResponse>(response);
        return ActionResultUtil.WrapOrNotFound(noteResponse).WithHeaders(headers);
    }
}
