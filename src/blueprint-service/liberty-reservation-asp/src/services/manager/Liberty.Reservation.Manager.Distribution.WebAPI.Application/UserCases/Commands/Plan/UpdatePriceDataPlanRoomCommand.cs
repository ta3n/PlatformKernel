using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Commands.Plan;

public record UpdatePriceDataPlanRoomCommand : UpdateCommandBase<UpdatePlanRoomRequest, BaseDataResponse<UpdatePriceDataResponse>>;
