namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class ReservationRoomGroupAppDatePersonAgeTypeRepository(
    ManagerDataContext dbContext
) : RepositoryBase<ReservationRoomGroupAppDatePersonAgeType>(dbContext),
    IReservationRoomGroupAppDatePersonAgeTypeRepository;
