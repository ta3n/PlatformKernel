using Liberty.Fax.ErrorCodeFax;
using Liberty.Fax.Models;
using Liberty.Fax.Services.Interfaces;

namespace Liberty.Fax.Services;

public class FaxErrorService : IFaxErrorService
{
    private readonly List<FaxErrorItem> _faxErrors = FaxErrors.FaxErrorItems;

    public List<FaxErrorItem> GetAll()
    {
        return _faxErrors;
    }

    public FaxErrorItem? GetByCode(
        string code
    )
    {
        return _faxErrors.Find(x => x.Code == code);
    }
}
