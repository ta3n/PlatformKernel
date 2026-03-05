using Liberty.GmoPaymentGateway.Models;

namespace Liberty.GmoPaymentGateway.Services;

public interface IGmoErrorCodeService
{
    /// <summary>
    /// Get all GMO error codes.
    /// </summary>
    List<GmoErrorItem> GetAll();

    /// <summary>
    /// Find an error by its detailed code (e.g., 42G020000, EZ1095008…).
    /// </summary>
    GmoErrorItem? FindByDetailCode(
        string detailCode
    );
}
