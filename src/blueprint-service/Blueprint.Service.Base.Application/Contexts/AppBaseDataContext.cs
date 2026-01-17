using Microsoft.EntityFrameworkCore;
using SharedKernel.UnitOfWork;

namespace Blueprint.Service.Base.Application.Contexts;

public class AppBaseDataContext(
    DbContextOptions<AppBaseDataContext> options
) : AppDbContextBase<AppBaseDataContext>(options)
{

}
