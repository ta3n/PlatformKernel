namespace Liberty.Reservation.User.Application.Contexts;

public class UserReadDataContext(
    DbContextOptions<UserReadDataContext> options
) : UserDataContext(options);
