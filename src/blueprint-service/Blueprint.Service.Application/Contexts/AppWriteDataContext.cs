namespace Blueprint.Service.Application.Contexts;

public class AppWriteDataContext(
    DbContextOptions<AppWriteDataContext> options
) : AppDataContext(options);
