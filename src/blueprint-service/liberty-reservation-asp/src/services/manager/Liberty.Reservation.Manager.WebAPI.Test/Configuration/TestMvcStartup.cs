using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace Liberty.Reservation.Manager.WebAPI.Test.Configuration;

public abstract class TestMvcStartup
{
    public static Action<MvcOptions> ConfigureMvcAuthorization()
    {
        return options => { options.Filters.Add(new AllowAnonymousFilter()); };
    }
}
