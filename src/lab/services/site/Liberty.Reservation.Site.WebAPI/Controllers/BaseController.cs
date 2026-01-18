using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Liberty.Reservation.Employee.WebAPI.Controllers;

[Authorize]
public abstract class BaseController : ControllerBase
{
}
