namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.OptionItemInventory;

public record OptionItemInventoryAdjustCommand
    : UpdateCommandBase<IEnumerable<OptionItemChangeRemainRequest>, long[]>;
