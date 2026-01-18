using System.Text;
using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Contexts;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;

namespace Liberty.Reservation.Manager.WebAPI.Test.Configuration;

public static class DataSeeder
{
    public static async Task SeedingAsync(
        string webRootPath,
        ManagerDataContext context
    )
    {
        await SeedBedTypeAsync(context, webRootPath);
    }

    private static async Task SeedBedTypeAsync(
        ManagerDataContext context,
        string webRootPath
    )
    {
        var isExisting = await context.BedTypes.AsNoTracking().AnyAsync();
        if (isExisting)
        {
            Log.Warning("${BadType} seeding skipped because data already exists.", nameof(BedType));
            return;
        }

        var filePath = Path.Combine(
            webRootPath,
            "BedTypeSeed.json"
        );
        using var reader = new StreamReader(filePath, Encoding.UTF8);
        var json = await reader.ReadToEndAsync();
        var seedData = JsonConvert.DeserializeObject<IList<BedType>>(json);

        if (seedData is not { Count: > 0 })
        {
            return;
        }

        foreach (var data in seedData)
        {
            data.Code = EntityUtil.CreateCode();
            data.RecordMemo = EntityUtil.CreateRecordMemo();
            data.BedTypeUnitType = BedTypeUnitTypes.Pair;
            data.DisplayOrder = seedData.IndexOf(data) + 1;
            await context.BedTypes.AddAsync(data);
        }

        await context.SaveChangesAsync();
    }
}
