namespace Liberty.Reservation.Site.Application.Contexts;

public class SiteWriteDataContext(
    DbContextOptions<SiteWriteDataContext> options
) : SiteDataContext(options);
