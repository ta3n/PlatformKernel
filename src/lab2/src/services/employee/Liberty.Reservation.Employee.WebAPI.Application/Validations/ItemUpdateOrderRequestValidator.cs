using Liberty.Reservation.Application.Utils;
using Liberty.SysException;

namespace Liberty.Reservation.Employee.WebAPI.Application.Validations;

public class ItemUpdateOrderRequestValidator : AbstractValidator<ItemUpdateOrderRequest>
{
    public ItemUpdateOrderRequestValidator()
    {
        RuleForEach(x => x.Ids)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001);
    }
}
