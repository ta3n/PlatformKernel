using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Utils;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class ItemUpdateArrangeOrderRequestValidator : AbstractValidator<ItemUpdateOrderRequest>
{
    public ItemUpdateArrangeOrderRequestValidator()
    {
        RuleFor(x => x.Ids)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .Must(ids => ids.TrueForAll(id => id > 0))
            .WithErrorCode(ErrorCode.E0010)
            .WithMessage(ErrorCode.E0010.GetEnumDescriptions());
    }
}
