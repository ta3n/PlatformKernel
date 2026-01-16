namespace PlatformKernel.Service.Application.Contexts;

public class AppWriteDataContext(
    DbContextOptions<AppWriteDataContext> options
) : AppDataContext(options);
