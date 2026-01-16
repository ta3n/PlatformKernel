namespace PlatformKernel.Service.Application.Contexts;

public class AppReadDataContext(
    DbContextOptions<AppReadDataContext> options
) : AppDataContext(options);
