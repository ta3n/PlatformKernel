namespace Liberty.Reservation.Site.Application.Contexts;

public class SiteReadDataContext(
    DbContextOptions<SiteReadDataContext> options
) : SiteDataContext(options);
