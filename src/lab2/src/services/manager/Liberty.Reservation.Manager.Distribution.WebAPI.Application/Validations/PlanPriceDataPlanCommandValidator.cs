using System.Globalization;
using FluentValidation;
using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Commands.Plan;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Web.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Validations;

public class PlanPriceDataPlanCommandValidator : AbstractValidator<UpdatePriceDataPlanRoomCommand>
{
    public PlanPriceDataPlanCommandValidator()
    {
        RuleFor(x => x.Payload.ScAgtFacilityCode)
            .Cascade(CascadeMode.Stop)
            .WithBlankAwareMessage(nameof(UpdatePlanRoomRequest.ScAgtFacilityCode));

        RuleFor(x => x.Payload.ScAgtPlanCode)
            .Cascade(CascadeMode.Stop)
            .WithBlankAwareMessage(nameof(UpdatePlanRoomRequest.ScAgtPlanCode));

        RuleFor(x => x.Payload.ScAgtSiteCode)
            .Cascade(CascadeMode.Stop)
            .WithBlankAwareMessage(nameof(UpdatePlanRoomRequest.ScAgtSiteCode));

        RuleFor(x => x.Payload.ScAgtRoomCode)
            .Cascade(CascadeMode.Stop)
            .WithBlankAwareMessage(nameof(UpdatePlanRoomRequest.ScAgtRoomCode));

        RuleFor(x => x.Payload.PriceData)
            .Cascade(CascadeMode.Stop)
            .Must(x => x is { Count: > 0 })
            .WithErrorCode(ErrorCode.E0001)
            .WithMessage($"{nameof(UpdatePlanRoomRequest.PriceData)}このフィールドは必須です。")
            .Must(x => x is { Count: <= 180 })
            .WithErrorCode(ErrorCode.E1053)
            .WithMessage($"{nameof(UpdatePlanRoomRequest.PriceData)} の件数が許容上限を超えています。");

        RuleForEach(p => p.Payload.PriceData)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .ChildRules(
                child =>
                {
                    child.RuleFor(x => x.AppointedDate)
                        .Cascade(CascadeMode.Stop)
                        .WithBlankAwareMessage(nameof(UpdatePriceData.AppointedDate))
                        .Must(
                            date => DateTime.TryParseExact(
                                date,
                                "yyyyMMdd",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.None,
                                out _
                            )
                        )
                        .WithMessage(
                            value => string.Format(
                                ErrorCode.E5003.GetEnumDescriptions(),
                                nameof(UpdatePriceData.AppointedDate),
                                value.AppointedDate
                            )
                        )
                        .Must(
                            date =>
                            {
                                _ = long.TryParse(date, out var dateValue);
                                var now = DateTime.UtcNow.AddHours(DefaultValues.TimeZoneOffset);
                                var nowId = AppDate.GetId(now);
                                return dateValue >= nowId;
                            }
                        )
                        .WithMessage(value => $"過去日の指定はできません。(対象日付：{value.AppointedDate})");

                    child.RuleFor(x => x.StopStartDivision)
                        .Cascade(CascadeMode.Stop)
                        .WithBlankAwareMessage(nameof(UpdatePriceData.StopStartDivision))
                        .Must(value => int.TryParse(value, out var num) && num is 0 or 1)
                        .WithMessage(
                            value => string.Format(
                                ErrorCode.E5003.GetEnumDescriptions(),
                                nameof(UpdatePriceData.StopStartDivision),
                                value.StopStartDivision
                            )
                        );

                    child.RuleFor(x => x.PriceElement)
                        .Cascade(CascadeMode.Stop)
                        .Must(x => x is { Count: > 0 })
                        .WithErrorCode(ErrorCode.E0001)
                        .WithMessage($"{nameof(UpdatePriceData.PriceElement)}このフィールドは必須です。")
                        .Must(x => x is { Count: <= 180 })
                        .WithErrorCode(ErrorCode.E1053)
                        .WithMessage($"{nameof(UpdatePriceData.PriceElement)} の件数が許容上限を超えています。");

                    child.RuleForEach(x => x.PriceElement)
                        .Cascade(CascadeMode.Stop)
                        .ChildRules(
                            priceElement =>
                            {
                                priceElement.RuleFor(x => x.PersonMin)
                                    .WithBlankAwareMessage(nameof(UpdatePriceDataItem.PersonMin))
                                    .Must(value => int.TryParse(value, out var num) && num >= 0)
                                    .WithMessage(
                                        x => string.Format(
                                            ErrorCode.E5003.GetEnumDescriptions(),
                                            nameof(UpdatePriceDataItem.PersonMin),
                                            x.PersonMin
                                        )
                                    );

                                priceElement.RuleFor(x => x.PersonMax)
                                    .WithBlankAwareMessage(nameof(UpdatePriceDataItem.PersonMax))
                                    .Must(
                                        (
                                            model,
                                            value
                                        ) =>
                                        {
                                            var isMaxValid = int.TryParse(value, out var max) && max >= 0;
                                            var isMinValid = int.TryParse(model.PersonMin, out var min);
                                            return isMaxValid && isMinValid && max >= min;
                                        }
                                    )
                                    .WithMessage(
                                        $"{nameof(UpdatePriceDataItem.PersonMax)} は {nameof(UpdatePriceDataItem.PersonMin)} 以降を登録してください。"
                                    );

                                priceElement.RuleFor(x => x.Price)
                                    .WithBlankAwareMessage(nameof(UpdatePriceDataItem.Price))
                                    .Must(
                                        value =>
                                        {
                                            _ = int.TryParse(value, out var num);
                                            return num is >= 0 and <= 999999;
                                        }
                                    )
                                    .WithMessage(
                                        x => string.Format(
                                            ErrorCode.E5003.GetEnumDescriptions(),
                                            nameof(UpdatePriceDataItem.Price),
                                            x.Price
                                        )
                                    );
                            }
                        )
                        .When(x => x.PriceElement!.Count > 0);
                }
            )
            .When(x => x.Payload.PriceData!.Count > 0);
    }
}
