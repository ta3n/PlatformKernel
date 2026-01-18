namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class ReservationRepository(
    ManagerDataContext dbContext
) : RepositoryBase<Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation>(dbContext),
    IReservationRepository
{
    public async Task<string?> GetCodeByIdAsync(
        long reservationId,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = GetQueryableWithAsNoTracking()
            .Where(r => r.Id == reservationId)
            .Select(r => r.Code);

        var code = await queryable.FirstOrDefaultAsync(cancellationToken);

        return code;
    }
}
