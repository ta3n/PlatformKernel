using Microsoft.EntityFrameworkCore;

namespace SharedKernel.BulkInsertPipeline.Test.Integration;

public sealed class TestMetricsDbContext(
    DbContextOptions<TestMetricsDbContext> options
) : DbContext(options);
