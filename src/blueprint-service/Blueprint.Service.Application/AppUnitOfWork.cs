using SharedKernel.UnitOfWork.Implementations;

namespace Blueprint.Service.Application;

public class AppUnitOfWork(
    DbContext context
) : BaseUnitOfWork(context), IAppUnitOfWork;
