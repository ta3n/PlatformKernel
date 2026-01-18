using Liberty.Reservation.Manager.Application.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Manager.File.WebAPI.Test.InfrastructureOfTest.Utilities;

public static class DbUtil
{
    public static async Task TruncateAllTablesAsync(
        this ManagerDataContext dbContext
    )
    {
        string[] ignoreTableNames =
        [
        ];

        var tableNames = dbContext.Model
            .GetEntityTypes()
            .Where(t => !t.IsOwned())
            .Where(t => !ignoreTableNames.Contains(t.GetTableName()!))
            .Select(t => $"\"{t.GetTableName()}\"")
            .Distinct()
            .ToList();

        var sql = $"TRUNCATE TABLE {string.Join(", ", tableNames)} RESTART IDENTITY CASCADE;";

        var connection = dbContext.Database.GetDbConnection();

        await using var command = connection.CreateCommand();
        command.CommandText = sql;

        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        await command.ExecuteNonQueryAsync();
    }
}
