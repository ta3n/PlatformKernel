using PlatformKernel.UnitOfWork.Implementations;

namespace PlatformKernel.Service.Application;

public class AppUnitOfWork(
    DbContext context
) : BaseUnitOfWork(context), IAppUnitOfWork;
