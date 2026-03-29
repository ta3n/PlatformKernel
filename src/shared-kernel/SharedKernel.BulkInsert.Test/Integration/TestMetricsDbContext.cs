using Microsoft.EntityFrameworkCore;

namespace SharedKernel.BulkInsert.Test.Integration;

public sealed class TestMetricsDbContext(DbContextOptions<TestMetricsDbContext> options) : DbContext(options);
