namespace Liberty.Reservation.Site.Application;

public class SiteUnitOfWork(
    DbContext context
) : BaseUnitOfWork(context), ISiteUnitOfWork;
