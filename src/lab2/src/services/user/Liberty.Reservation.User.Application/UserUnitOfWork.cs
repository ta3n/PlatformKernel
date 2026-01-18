using Liberty.UnitOfWork.Implementations;

namespace Liberty.Reservation.User.Application;

public class UserUnitOfWork(
    DbContext context
) : BaseUnitOfWork(context), IUserUnitOfWork;
