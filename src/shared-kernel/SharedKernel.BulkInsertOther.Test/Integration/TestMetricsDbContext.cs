using Microsoft.EntityFrameworkCore;

namespace SharedKernel.BulkInsertOther.Test.Integration;

public sealed class TestMetricsDbContext(DbContextOptions<TestMetricsDbContext> options) : DbContext(options);
