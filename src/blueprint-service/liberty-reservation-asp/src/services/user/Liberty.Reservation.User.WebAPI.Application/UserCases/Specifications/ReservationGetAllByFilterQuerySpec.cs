using System.Linq.Expressions;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.User.WebAPI.Application.Models.Requests;
using Liberty.Specification;
using Liberty.UnitOfWork.DbFunctions;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.User.WebAPI.Application.UserCases.Specifications;

public class ReservationGetAllByFilterQuerySpec : SpecificationBase<ReservationEntity>
{
    private static readonly ReservationStatus[] AllowedReservationStates =
    [
        ReservationStatus.Confirmed,
        ReservationStatus.Reserved,
        ReservationStatus.UserCanceled,
        ReservationStatus.Modified,
        ReservationStatus.ManagerCanceled
    ];

    public override Expression<Func<ReservationEntity, bool>>? Criteria { get; }

    public ReservationGetAllByFilterQuerySpec(
        BookingFilterRequest request
    )
    {
        Expression<Func<ReservationEntity, bool>>? expr = null;

        AddStatusFilters(request.UserBookingFilter, ref expr);

        Criteria = expr;
    }

    private static void AddStatusFilters(
        UserBookingFilter? bookingFilterType,
        ref Expression<Func<ReservationEntity, bool>>? expr
    )
    {
        var now = DateTime.UtcNow.AddHours(DefaultValues.TimeZoneOffset);

        switch (bookingFilterType)
        {
            case UserBookingFilter.ReservationCompleted:
                AddCondition(
                    x => (x.ReservationState == ReservationStatus.Reserved || x.ReservationState == ReservationStatus.Modified)
                        && x.CheckInDate.GetDateTime(x.CheckOutTime).AddDays(x.RestNumber) < now,
                    ref expr
                );
                break;

            case UserBookingFilter.Canceled:
                AddCondition(
                    x =>
                        x.ReservationState == ReservationStatus.UserCanceled
                        || x.ReservationState == ReservationStatus.ManagerCanceled,
                    ref expr
                );
                break;

            default:
                AddCondition(
                    x => AllowedReservationStates.Contains(x.ReservationState),
                    ref expr
                );
                break;
        }
    }

    private static void AddCondition(
        Expression<Func<ReservationEntity, bool>> newCondition,
        ref Expression<Func<ReservationEntity, bool>>? expr
    )
    {
        expr = expr == null ? newCondition : expr.CombineExpressions(newCondition);
    }
}
