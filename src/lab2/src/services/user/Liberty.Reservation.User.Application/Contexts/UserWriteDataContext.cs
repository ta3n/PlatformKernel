namespace Liberty.Reservation.User.Application.Contexts;

public class UserWriteDataContext(
    DbContextOptions<UserWriteDataContext> options
) : UserDataContext(options);
