using Liberty.Reservation.Application.Cqrs.BaseCommands;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.SystemConfig;

public record SystemConfigCanOnlinePaymentCommand : UpdateCommandBase<SystemConfigCanOnlinePaymentRequest, SystemConfigResponse>;
