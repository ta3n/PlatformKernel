using System;
using System.Linq.Expressions;
using Liberty.Specification;
using Liberty.UnitOfWork.DbFunctions;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Specifications;

internal class ReservationGetAllByFilterQuerySpec : SpecificationBase<ReservationEntity>
{
    public override Expression<Func<ReservationEntity, bool>>? Criteria { get; }

    public static class ReservationSortFields
    {
        public const string ReservationDateTime = nameof(ReservationEntity.ReservationDateTime);
        public const string Code = nameof(ReservationEntity.Code);
        public const string CancelledDateTime = nameof(ReservationEntity.CancelledDateTime);
        public const string CheckInDate = nameof(ReservationEntity.CheckInDate);
        public const string ReserverName = "ReserverName";
    }

    public ReservationGetAllByFilterQuerySpec(
        ReservationGetAllBylFilterRequest request,
        TimeSpan timeZone,
        IPageable pageable
    )
    {
        Expression<Func<ReservationEntity, bool>>? expr = null;

        AddBasicFilters(request, ref expr);
        AddDateFilters(request, timeZone, ref expr);
        AddReserverFilters(request, ref expr);
        ApplySorting(pageable.Sort);

        Criteria = expr;
    }

    private static void AddBasicFilters(
        ReservationGetAllBylFilterRequest request,
        ref Expression<Func<ReservationEntity, bool>>? expr
    )
    {
        if (!string.IsNullOrEmpty(request.Code))
        {
            AddCondition(
                x => x.BookingData!.RootCode == request.Code,
                ref expr
            );
        }

        if (request.ReservationStatus?.Any() is true)
        {
            AddCondition(
                x => request.ReservationStatus.Contains(x.ReservationState),
                ref expr
            );
        }
        else
        {
            AddCondition(
                x => ReservationStatusFilter.AllowedStatuses.Contains(x.ReservationState),
                ref expr
            );
        }

        if (request.SiteIds?.Any() is true)
        {
            AddCondition(
                x => request.SiteIds.Contains(x.SiteId),
                ref expr
            );
        }

        if (request.PaymentTypes?.Any() is true)
        {
            AddCondition(
                x => request.PaymentTypes.Contains(x.PaymentType),
                ref expr
            );
        }

        if (request.RoomIds?.Any() is true)
        {
            AddCondition(
                x => request.RoomIds.Contains(x.RoomGroupId),
                ref expr
            );
        }
    }

    private static void AddDateFilters(
        ReservationGetAllBylFilterRequest request,
        TimeSpan timeZone,
        ref Expression<Func<ReservationEntity, bool>>? expr
    )
    {
        if (request.StartCheckInDate is not null)
        {
            AddCondition(
                x => x.CheckInDate >= request.StartCheckInDate,
                ref expr
            );
        }

        if (request.EndCheckInDate is not null)
        {
            AddCondition(
                x => x.CheckInDate <= request.EndCheckInDate,
                ref expr
            );
        }

        if (request.StartReservationAcceptanceDate is not null)
        {
            var startReservationAcceptanceDate = AppDate
                .GetDateTime(request.StartReservationAcceptanceDate)!.Value
                .Add(-1 * timeZone);

            AddCondition(
                x => x.ReservationDateTime >= startReservationAcceptanceDate,
                ref expr
            );
        }

        if (request.EndReservationAcceptanceDate is not null)
        {
            var endReservationAcceptanceDate = AppDate
                .GetDateTime(request.EndReservationAcceptanceDate)!.Value
                .Add(new TimeSpan(23, 59, 59))
                .Add(-1 * timeZone);

            AddCondition(
                x => x.ReservationDateTime <= endReservationAcceptanceDate,
                ref expr
            );
        }

        if (request.StartCancellationDate is not null)
        {
            var startCancellationDate = AppDate
                .GetDateTime(request.StartCancellationDate)!.Value
                .Add(-1 * timeZone);

            AddCondition(
                x => x.CancelledDateTime >= startCancellationDate,
                ref expr
            );
        }

        if (request.EndCancellationDate is not null)
        {
            var endCancellationDate = AppDate
                .GetDateTime(request.EndCancellationDate)!.Value
                .Add(new TimeSpan(23, 59, 59))
                .Add(-1 * timeZone);

            AddCondition(
                x => x.CancelledDateTime <= endCancellationDate,
                ref expr
            );
        }

        if (request.StartNoShowDate is not null)
        {
            var startNoShowDate = AppDate
                .GetDateTime(request.StartNoShowDate)!.Value
                .Add(-1 * timeZone);

            AddCondition(
                x => x.NoShowDateTime >= startNoShowDate,
                ref expr
            );
        }

        if (request.EndNoShowDate is not null)
        {
            var endNoShowDate = AppDate
                .GetDateTime(request.EndNoShowDate)!.Value
                .Add(new TimeSpan(23, 59, 59))
                .Add(-1 * timeZone);

            AddCondition(
                x => x.NoShowDateTime <= endNoShowDate,
                ref expr
            );
        }
    }

    private static void AddReserverFilters(
        ReservationGetAllBylFilterRequest request,
        ref Expression<Func<ReservationEntity, bool>>? expr
    )
    {
        if (!string.IsNullOrEmpty(request.Name))
        {
            AddCondition(
                x => x.Reserver!.Name!.Contains(request.Name),
                ref expr
            );
        }

        if (!string.IsNullOrEmpty(request.Kana))
        {
            AddCondition(
                x => x.Reserver!.Kana!.Contains(request.Kana),
                ref expr
            );
        }

        if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            AddCondition(
                x => x.Reserver!.Phone!.Contains(request.PhoneNumber),
                ref expr
            );
        }

        if (!string.IsNullOrEmpty(request.EMail))
        {
            AddCondition(
                x => x.Reserver!.EMail!.Contains(request.EMail),
                ref expr
            );
        }

        if (!string.IsNullOrEmpty(request.PostCode))
        {
            AddCondition(
                x => x.Reserver!.PostCode!.Contains(request.PostCode),
                ref expr
            );
        }

        if (!string.IsNullOrEmpty(request.PlanSearch))
        {
            AddCondition(
                x => x.BookingData!.Plan.Name!.Contains(request.PlanSearch.Trim()),
                ref expr
            );
        }

        if (!string.IsNullOrEmpty(request.Address1))
        {
            var keyword = request.Address1.Trim();

            AddCondition(
                x =>
                    EF.Functions.ILike(
                        (x.Reserver!.Address1 ?? "") + (x.Reserver.Address2 ?? "") + (x.Reserver.Address3 ?? ""),
                        $"%{keyword}%"
                    ),
                ref expr
            );
        }

        if (!string.IsNullOrEmpty(request.FreeInput))
        {
            AddCondition(
                x => x.Memo!.Contains(request.FreeInput),
                ref expr
            );
        }

        if (request.IsNoShow is not null)
        {
            AddCondition(
                x => x.IsNoShow == request.IsNoShow,
                ref expr
            );
        }

        if (request.DayUse is not null)
        {
            AddCondition(
                x => request.DayUse == x.BookingData!.Plan.DayUse,
                ref expr
            );
        }
    }

    private void ApplySorting(
        Sort sort
    )
    {
        if (sort.IsUnsorted())
        {
            return;
        }

        foreach (var order in sort.Orders)
        {
            switch (order.Property)
            {
                case ReservationSortFields.ReservationDateTime:
                    ApplyOrder(order, x => x.ReservationDateTime);
                    break;

                case ReservationSortFields.Code:
                    ApplyOrder(order, x => x.BookingData!.RootCode!);
                    break;

                case ReservationSortFields.CancelledDateTime:
                    ApplyOrder(order, x => x.CancelledDateTime!);
                    break;

                case ReservationSortFields.CheckInDate:
                    ApplyOrder(order, x => x.CheckInDate);
                    break;

                case ReservationSortFields.ReserverName:
                    ApplyOrder(order, x => x.Reserver!.Name!);
                    break;
            }
        }
    }

    private void ApplyOrder(
        Pagination.Order sortOrder,
        Expression<Func<ReservationEntity, object>> expr
    )
    {
        if (sortOrder.Direction == Direction.Desc)
        {
            ApplyOrderByDescending(expr);
        }
        else
        {
            ApplyOrderBy(expr);
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
