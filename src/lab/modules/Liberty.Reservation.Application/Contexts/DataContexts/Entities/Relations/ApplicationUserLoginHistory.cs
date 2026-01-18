using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class ApplicationUserLoginHistory : EntityRelation
{
    public long UserId { get; set; }
    public User? User { get; set; }

    public long LoginHistoryId { get; set; }
    public LoginHistory? LoginHistory { get; set; }

    public ApplicationUserLoginHistory()
    {
    }

    public ApplicationUserLoginHistory(
        User user,
        LoginHistory loginHistory
    )
    {
        UserId = user.Id;
        User = user;
        LoginHistoryId = loginHistory.Id;
        LoginHistory = loginHistory;
    }
}
