using Liberty.GmoPaymentGateway.ErrorCodeGmo;
using Liberty.GmoPaymentGateway.Models;

namespace Liberty.GmoPaymentGateway.Services.Implementations;

public class GmoErrorCodeService : IGmoErrorCodeService
{
    private readonly List<GmoErrorItem> _errorCodes;

    public GmoErrorCodeService()
    {
        _errorCodes = GmoErrors.GmoErrorItems;
    }

    public List<GmoErrorItem> GetAll()
    {
        return _errorCodes;
    }

    public GmoErrorItem? FindByDetailCode(
        string detailCode
    )
    {
        return _errorCodes.Find(e => e.DetailCode == detailCode);
    }
}
