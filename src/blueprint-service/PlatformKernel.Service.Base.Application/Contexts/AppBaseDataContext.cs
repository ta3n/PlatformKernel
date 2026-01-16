using Microsoft.EntityFrameworkCore;
using PlatformKernel.UnitOfWork;

namespace PlatformKernel.Service.Base.Application.Contexts;

public class AppBaseDataContext(
    DbContextOptions<AppBaseDataContext> options
) : AppDbContextBase<AppBaseDataContext>(options)
{

}
