using System.Text;
using Liberty.ApplicationShared.Utils;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Employee.Application.Contexts;
using Liberty.Reservation.Employee.Application.Contexts.SeedData;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;

namespace Liberty.Reservation.Employee.WebAPI.Test.Configuration;

public static class DataSeeder
{
    public static async Task SeedingAsync(
        string webRootPath,
        EmployeeDataContext context
    )
    {
        var seedDataPath = webRootPath;

        await SeedLanguageAsync(context);
        await SeedFaxServiceAsync(context);
        await SeedMailTemplateAsync(context, seedDataPath);
        await SeedAllergenAsync(context, seedDataPath);
        await SeedPersonAgeTypeAsync(context, seedDataPath);
        await SeedConsumptionTaxAsync(context, seedDataPath);
        await SeedMealTypeAsync(context, seedDataPath);
    }

    private static async Task SeedFaxServiceAsync(
        EmployeeDataContext context
    )
    {
        var isFaxService = await context.FaxServices.AnyAsync();
        if (isFaxService)
        {
            Log.Warning("${FaxServices} seeding skipped because data already exists.", nameof(FaxService));
            return;
        }

        var faxService = new FaxService
        {
            Name = "FaxsimoSilver",
            Code = EntityUtil.CreateCode(),
            RecordMemo = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
            IsEnabled = true,
            MailFormat = string.Empty
        };
        await context.FaxServices.AddAsync(faxService);
        await context.SaveChangesAsync();
    }

    private static async Task SeedMailTemplateAsync(
        EmployeeDataContext context,
        string seedDataPath
    )
    {
        var isSystemConfig = await context.SystemConfigs.AnyAsync();
        if (isSystemConfig)
        {
            Log.Warning("${SystemConfig} seeding skipped because data already exists.", nameof(SystemConfig));
            return;
        }

        var filePath = Path.Combine(seedDataPath, "SystemConfigSeed.json");
        using var reader = new StreamReader(filePath, Encoding.UTF8);
        var json = await reader.ReadToEndAsync();
        var systemConfigSeedData = JsonConvert.DeserializeObject<SeedConfig>(json);

        if (systemConfigSeedData is null)
        {
            return;
        }

        var systemConfig = new SystemConfig
        {
            TemplateFormatData = systemConfigSeedData.TemplateFormatData,
            IsEnabled = true,
            IsVisible = true,
            Code = string.Empty,
            RecordMemo = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
        };
        await context.AddAsync(systemConfig);
        await context.SaveChangesAsync();
    }

    private static async Task SeedAllergenAsync(
        EmployeeDataContext context,
        string seedDataPath
    )
    {
        var isExisting = await context.Allergens.AsNoTracking().AnyAsync();
        if (isExisting)
        {
            Log.Warning("${Allergen} seeding skipped because data already exists.", nameof(Allergen));
            return;
        }

        var filePath = Path.Combine(seedDataPath, "AllergenSeed.json");
        using var reader = new StreamReader(filePath, Encoding.UTF8);
        var json = await reader.ReadToEndAsync();
        var seedData = JsonConvert.DeserializeObject<IList<Allergen>>(json);

        if (seedData is not { Count: > 0 })
        {
            return;
        }

        foreach (var data in seedData)
        {
            data.Code = EntityUtil.CreateCode();
            data.RecordMemo = EntityUtil.CreateRecordMemo();
            data.IsEnabled = true;
            await context.Allergens.AddAsync(data);
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedPersonAgeTypeAsync(
        EmployeeDataContext context,
        string seedDataPath
    )
    {
        var isExisting = await context.PersonAgeTypeSpaTaxDatas.AsNoTracking().AnyAsync();
        if (isExisting)
        {
            Log.Warning("${PersonAgeTypeSeed} seeding skipped because data already exists.", nameof(PersonAgeTypeSeed));
            return;
        }

        var filePath = Path.Combine(seedDataPath, "PersonAgeTypeSeed.json");
        using var reader = new StreamReader(filePath, Encoding.UTF8);
        var json = await reader.ReadToEndAsync();
        var seedData = JsonConvert.DeserializeObject<IList<PersonAgeTypeSeed>>(json);

        if (seedData is not { Count: > 0 })
        {
            return;
        }

        var personAgeTypeSpaTaxDatas = new List<PersonAgeTypeSpaTaxData>();
        var personAgeTypes = new List<PersonAgeType>();

        foreach (var data in seedData)
        {
            var personAgeType = new PersonAgeType
            {
                Code = EntityUtil.CreateCode(),
                Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), data.Name ?? string.Empty } },
                IsMaster = data.IsMaster,
                IsMain = data.IsMain,
                AgeMax = data.AgeMax,
                AgeMin = data.AgeMin,
                Meta = data.Meta,
                RecordMemo = EntityUtil.CreateRecordMemo(),
                IsEnabled = true
            };
            personAgeTypes.Add(personAgeType);

            personAgeTypeSpaTaxDatas.AddRange(
                data.SpaTaxes.Select(
                    spaTax => new PersonAgeTypeSpaTaxData
                    {
                        PersonAgeType = personAgeType,
                        SpaTaxData = new SpaTaxData
                        {
                            Code = EntityUtil.CreateCode(),
                            PriceMax = spaTax.PriceMax,
                            PriceMin = spaTax.PriceMin,
                            Tax = spaTax.Tax,
                            RecordMemo = EntityUtil.CreateRecordMemo(),
                            IsEnabled = true
                        }
                    }
                )
            );
        }

        var strategy = context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(
            async () =>
            {
                await using var transaction = await context.Database.BeginTransactionAsync();
                try
                {
                    await context.PersonAgeTypes.AddRangeAsync(personAgeTypes);
                    await context.PersonAgeTypeSpaTaxDatas.AddRangeAsync(personAgeTypeSpaTaxDatas);

                    await context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                }
            }
        );
    }

    private static async Task SeedConsumptionTaxAsync(
        EmployeeDataContext context,
        string seedDataPath
    )
    {
        var isExisting = await context.ConsumptionTaxs.AsNoTracking().AnyAsync();
        if (isExisting)
        {
            Log.Warning("${ConsumptionTax} seeding skipped because data already exists.", nameof(ConsumptionTax));
            return;
        }

        var filePath = Path.Combine(seedDataPath, "ConsumptionTaxSeed.json");
        using var reader = new StreamReader(filePath, Encoding.UTF8);
        var json = await reader.ReadToEndAsync();
        var seedData = JsonConvert.DeserializeObject<IList<ConsumptionTax>>(json);

        if (seedData is not { Count: > 0 })
        {
            return;
        }

        foreach (var data in seedData)
        {
            data.Code = EntityUtil.CreateCode();
            data.RecordMemo = EntityUtil.CreateRecordMemo();
            data.IsEnabled = true;
            await context.ConsumptionTaxs.AddAsync(data);
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedMealTypeAsync(
        EmployeeDataContext context,
        string seedDataPath
    )
    {
        var isExisting = await context.MealTypes.AsNoTracking().AnyAsync();
        if (isExisting)
        {
            Log.Warning("${MealType} seeding skipped because data already exists.", nameof(MealType));
            return;
        }

        var filePath = Path.Combine(seedDataPath, "MealTypeSeed.json");
        using var reader = new StreamReader(filePath, Encoding.UTF8);
        var json = await reader.ReadToEndAsync();
        var seedData = JsonConvert.DeserializeObject<IList<MealType>>(json);

        if (seedData is not { Count: > 0 })
        {
            return;
        }

        foreach (var data in seedData)
        {
            data.Code = EntityUtil.CreateCode();
            data.RecordMemo = EntityUtil.CreateRecordMemo();
            data.IsEnabled = true;
            await context.MealTypes.AddAsync(data);
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedLanguageAsync(
        EmployeeDataContext context
    )
    {
        var isExisting = await context.Languages.AnyAsync();
        if (isExisting)
        {
            Log.Warning("${Language} seeding skipped because data already exists.", nameof(Language));
            return;
        }

        Language[] languages =
        [
            new()
            {
                Code = "ja",
                Name = "日本語",
                Description = "",
                IsMaster = true
            },
            new()
            {
                Code = "en",
                Name = "English",
                Description = "",
                IsMaster = true
            }
        ];

        await context.Languages.AddRangeAsync(languages);
        await context.SaveChangesAsync();
    }
}
