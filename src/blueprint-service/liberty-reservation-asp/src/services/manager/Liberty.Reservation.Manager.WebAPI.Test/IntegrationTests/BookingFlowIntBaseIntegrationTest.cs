using System.Diagnostics;
using System.Net;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using File = System.IO.File;
using SiteEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Site;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class BookingFlowIntTest : BaseIntegrationTest
{
    [Fact]
    public async Task CreateTestDataForBookingFlow()
    {
        var personAgeTypes = await CreatePersonAgeTypesAsync();
        var planCategoryIds = await CreateCategoriesAsync("api/plan-categories");
        var roomCategoryIds = await CreateCategoriesAsync("api/room-group-categories");
        var siteIds = await CreateSitesAsync();
        var optionIds = await CreateOptionItemsAsync();
        var cancellationIds = await CreateCancellationsAsync();
        var questionIds = await CreateQuestionsAsync();
        var roomGroupIds = await CreateRoomGroupsAsync(
            roomCategoryIds,
            siteIds
        );
        var cancellationDataId = await CreateCancellationDataAsync();
        await CreateDataOfCancellationDataAsync(cancellationIds, cancellationDataId);
        await UpdateStandardPriceOfRoomAsync(roomGroupIds, siteIds[0]);
        await CreateSiteInRoomOfRoomAsync(roomGroupIds, siteIds[0]);
        var ageTypes = personAgeTypes as PersonAgeType[] ?? [.. personAgeTypes];
        await UpdateChildrenPriceOfRoomAsync(roomGroupIds, [.. ageTypes], siteIds[0]);
        await UpdateDiscountOfRoomAsync(roomGroupIds, siteIds[0]);

        var planIds = await CreatePlansAsync(
            planCategoryIds,
            roomGroupIds,
            siteIds,
            optionIds,
            cancellationIds,
            questionIds
        );
        await CreatePlanRoomGroupSitePriceAsync(planIds[0], roomGroupIds, siteIds[0]);

        await CreateSiteInRoomOfPlanAsync(planIds, roomGroupIds[0], siteIds[0]);
        await UpdateStandardPriceOfPlanAsync(planIds, roomGroupIds[0], siteIds[0]);
        await UpdateChildrenPriceOfPlanAsync(planIds, [.. ageTypes], roomGroupIds[0], siteIds[0]);
        await UpdateSaleSettingOfPlanAsync(planIds, roomGroupIds[0], siteIds[0]);
        await UpdatePriceCalendarOfPlanAsync(planIds, roomGroupIds[0], siteIds[0]);
        await UpdateDiscountOfPlanAsync(planIds, roomGroupIds[0], siteIds[0]);
        await CreateSystemConfigAsync();
        var exportFilePath = await ExportDatabaseAsync();
        var exportFileExists = File.Exists(exportFilePath);
        Assert.True(exportFileExists);
    }

    private async Task UpdateDiscountOfPlanAsync(
        List<long> planIds,
        long roomId,
        long siteId
    )
    {
        foreach (var planId in planIds)
        {
            var response = await Client.PatchAsync(
                $"/api/plan-prices/{planId}/room-groups/{roomId}/destinations/{siteId}/discount",
                TestUtil.ToJsonContent(
                    new RoomGroupPriceUpdateDiscountRequest(
                        [
                            new(
                                1,
                                2,
                                1,
                                10,
                                PriceSettingTypes.Price,
                                50
                            ),
                            new(
                                3,
                                5,
                                1,
                                10,
                                PriceSettingTypes.Price,
                                50
                            ),
                            new(
                                6,
                                9,
                                1,
                                10,
                                PriceSettingTypes.Price,
                                50
                            )
                        ]
                    )
                )
            );
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }

    private async Task UpdatePriceCalendarOfPlanAsync(
        List<long> planIds,
        long roomId,
        long siteId
    )
    {
        foreach (var planId in planIds)
        {
            var response = await Client.PatchAsync(
                $"/api/plan-prices/{planId}/room-groups/{roomId}/destinations/{siteId}/price-calendar",
                TestUtil.ToJsonContent(
                    new RoomGroupPriceUpdatePriceCalendarRequest(
                        [
                            new(
                                AppDate.GetId(DateTime.Now),
                                1,
                                4,
                                400,
                                false
                            ),
                            new(
                                AppDate.GetId(DateTime.Now.AddDays(1)),
                                1,
                                10,
                                450,
                                false
                            )
                        ]
                    )
                )
            );
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }

    private async Task UpdateSaleSettingOfPlanAsync(
        List<long> planIds,
        long roomId,
        long siteId
    )
    {
        foreach (var planId in planIds)
        {
            var response = await Client.PatchAsync(
                $"/api/plan-prices/{planId}/room-groups/{roomId}/destinations/{siteId}/sale",
                TestUtil.ToJsonContent(
                    new RoomGroupPriceUpdateSaleRequest
                    {
                        UseAutoExtend = false,
                        AutoExtendMonth = 15,
                        AutoExtendEveryMonthDay = 3
                    }
                )
            );
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }

    private async Task UpdateChildrenPriceOfPlanAsync(
        List<long> planIds,
        List<PersonAgeType> personAgeTypes,
        long roomId,
        long siteId
    )
    {
        foreach (var planId in planIds)
        {
            List<RoomTypeUpdateChildrenPersonAgeTypeRequest> reqChildrenPrices = [];
            personAgeTypes.ForEach(
                personAgeType =>
                {
                    reqChildrenPrices.Add(
                        new RoomTypeUpdateChildrenPersonAgeTypeRequest(
                            personAgeType.Id,
                            true,
                            true,
                            PriceSettingTypes.Discount,
                            200
                        )
                    );
                }
            );

            var response = await Client.PatchAsync(
                $"/api/plan-prices/{planId}/room-groups/{roomId}/destinations/{siteId}/children-price",
                TestUtil.ToJsonContent(
                    new RoomGroupPriceUpdateChildrenPriceRequest(
                        reqChildrenPrices
                    )
                )
            );
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }

    private async Task CreateSiteInRoomOfPlanAsync(
        List<long> planIds,
        long roomId,
        long siteId
    )
    {
        foreach (var planId in planIds)
        {
            var response = await Client.PostAsync(
                $"/api/plan-prices/{planId}/room-groups/{roomId}/destinations/{siteId}",
                null
            );
            response.EnsureSuccessStatusCode();
        }
    }

    private async Task UpdateStandardPriceOfPlanAsync(
        List<long> planIds,
        long roomId,
        long siteId
    )
    {
        var appDateTypesList = (await CreateAppDateTypesAsync()).ToList();

        foreach (var planId in planIds)
        {
            List<RomTypeUpdateStandardRequest> reqStandardPrices = [];
            appDateTypesList.ForEach(
                appDateType =>
                {
                    reqStandardPrices.Add(
                        new RomTypeUpdateStandardRequest(
                            appDateType.Id,
                            1,
                            4,
                            400
                        )
                    );

                    reqStandardPrices.Add(
                        new RomTypeUpdateStandardRequest(
                            appDateType.Id,
                            1,
                            10,
                            450
                        )
                    );

                    reqStandardPrices.Add(
                        new RomTypeUpdateStandardRequest(
                            appDateType.Id,
                            1,
                            10,
                            450
                        )
                    );
                }
            );

            var response = await Client.PatchAsync(
                $"/api/plan-prices/{planId}/room-groups/{roomId}/destinations/{siteId}/standard-price",
                TestUtil.ToJsonContent(
                    new RoomGroupPriceUpdateStandardPriceRequest(
                        reqStandardPrices
                    )
                )
            );
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }

    private async Task<List<long>> CreateCategoriesAsync(
        string categoryUrl
    )
    {
        var baseUrl = categoryUrl;

        List<long> ids = [];
        for (var i = 0; i < 5; i++)
        {
            var createReq = new CategoryCreateRequest(
                $"Category {i + 1}",
                $"Category {i + 1} Description"
            );

            var response = await Client.PostAsync(
                baseUrl,
                TestUtil.ToJsonContent(createReq)
            );
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseString);

            _ = long.TryParse(responseString, out var id);
            ids.Add(id);

            var enableReq = new CategoryEnabledRequest(true) { Id = id };

            response = await Client.PatchAsync(
                $"{baseUrl}/{id}/enable",
                TestUtil.ToJsonContent(enableReq)
            );
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        return ids;
    }

    private async Task<List<long>> CreateOptionItemsAsync()
    {
        var baseUrl = "/api/option-items";

        List<long> ids = [];
        for (var i = 0; i < 5; i++)
        {
            var createReq = new OptionItemCreateRequest(
                $"Option Item {i + 1}",
                $"Option Item {i + 1} Description",
                4,
                600
            );

            var response = await Client.PostAsync(
                baseUrl,
                TestUtil.ToJsonContent(createReq)
            );
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseString);

            _ = long.TryParse(responseString, out var id);
            ids.Add(id);

            var enableReq = new OptionItemEnabledRequest(true) { Id = id };

            response = await Client.PatchAsync(
                $"{baseUrl}/{id}/enable",
                TestUtil.ToJsonContent(enableReq)
            );
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            await AdjustAppDatesOfOptionItemsAsync(id);
        }

        return ids;
    }

    private async Task AdjustAppDatesOfOptionItemsAsync(
        long id
    )
    {
        var baseUrl = "api/option-item-inventory";
        var request = new List<OptionItemChangeRemainRequest>
        {
            new(
                AppDate.GetId(DateTime.UtcNow),
                id,
                10,
                false
            ),
            new(
                AppDate.GetId(DateTime.UtcNow.AddDays(1)),
                id,
                10,
                false
            )
        };

        var response = await Client.PutAsync(
            baseUrl,
            TestUtil.ToJsonContent(request)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private async Task<List<long>> CreatePlansAsync(
        List<long> planCategoryIds,
        List<long> roomGroupIds,
        List<long> siteIds,
        List<long> optionIds,
        List<long> cancellationIds,
        List<long> questionIds
    )
    {
        const string baseUrl = "api/plans";

        List<long> ids = [];
        for (var i = 0; i < 5; i++)
        {
            var createReq = new PlanCreateRequest(
                $"Plan {i + 1}",
                $"Plan {i + 1} Description",
                false,
                PlanTypes.Combo
            );

            var response = await Client.PostAsync(baseUrl, TestUtil.ToJsonContent(createReq));
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseString);

            _ = long.TryParse(responseString, out var id);

            await PlanEnableAsync(id);
            await PlanUpdateDisplayAsync(id, planCategoryIds);
            await PlanUpdateRoomAsync(id, roomGroupIds);
            await PlanEnableRoomAsync(id, roomGroupIds);
            await PlanUpdatePublishAcceptAsync(id, siteIds);
            await PlanUpdateOptionAsync(id, optionIds);
            await PlanUpdateSaleAsync(id);
            await PlanUpdatePaymentMethodAsync(id);

            var rand = new Random();
            await PlanUpdateCancelAsync(id, cancellationIds[rand.Next(0, cancellationIds.Count)]);
            await PlanUpdateQuestionAsync(id, questionIds);

            ids.Add(id);
        }

        for (var i = 6; i <= 10; i++)
        {
            var createReq = new PlanCreateRequest(
                $"Plan {i + 1}",
                $"Plan {i + 1} Description",
                false,
                PlanTypes.RoomOnly
            );

            var response = await Client.PostAsync(baseUrl, TestUtil.ToJsonContent(createReq));
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseString);

            _ = long.TryParse(responseString, out var id);

            await PlanEnableAsync(id);
            await PlanUpdateDisplayAsync(id, planCategoryIds);
            await PlanUpdateRoomAsync(id, roomGroupIds);
            await PlanEnableRoomAsync(id, roomGroupIds);
            await PlanUpdatePublishAcceptAsync(id, siteIds);
            await PlanUpdateOptionAsync(id, optionIds);
            await PlanUpdateSaleAsync(id);
            await PlanUpdatePaymentMethodAsync(id);

            var rand = new Random();
            await PlanUpdateCancelAsync(id, cancellationIds[rand.Next(0, cancellationIds.Count)]);
            await PlanUpdateQuestionAsync(id, questionIds);

            ids.Add(id);
        }

        return ids;
    }

    private async Task PlanUpdatePaymentMethodAsync(
        long id
    )
    {
        var baseUrl = "api/plans";

        var updateReq = new PlanUpdatePaymentMethodRequest(true, true);

        var response = await Client.PatchAsync(
            $"{baseUrl}/{id}/payment-method",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private async Task PlanUpdateQuestionAsync(
        long id,
        List<long> questionIds
    )
    {
        var baseUrl = "api/plans";
        var updateReq = new PlanUpdateQuestionRequest(
            questionIds
        );

        var response = await Client.PatchAsync(
            $"{baseUrl}/{id}/question",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private async Task PlanUpdateCancelAsync(
        long id,
        long cancellationId
    )
    {
        var baseUrl = "api/plans";
        var updateReq = new PlanUpdateCancelRequest(
            true,
            5,
            new TimeSpan(12, 0, 0),
            cancellationId
        );

        var response = await Client.PatchAsync(
            $"{baseUrl}/{id}/cancel",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private async Task PlanUpdateOptionAsync(
        long id,
        List<long> optionIds
    )
    {
        var baseUrl = "api/plans";
        var updateReq = new PlanUpdateOptionRequest(
            true,
            optionIds
        );

        var response = await Client.PatchAsync(
            $"{baseUrl}/{id}/option",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private async Task PlanUpdateSaleAsync(
        long id
    )
    {
        var baseUrl = "api/plans";
        var updateReq = new PlanUpdateSaleRequest(
            TimeSpan.FromHours(1),
            TimeSpan.FromHours(3),
            TimeSpan.FromHours(3),
            0,
            10,
            null,
            PlanDaySaleLimitTypes.RoomGroup,
            false,
            null,
            null,
            true,
            1,
            5,
            7,
            3
        );

        var response = await Client.PatchAsync(
            $"{baseUrl}/{id}/sale",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private async Task PlanUpdatePublishAcceptAsync(
        long id,
        List<long> siteIds
    )
    {
        var baseUrl = "api/plans";
        var rand = new Random();
        var updateReq = new PlanUpdatePublishAcceptRequest(
            true,
            AppDate.GetId(DateTime.Now),
            AppDate.GetId(DateTime.Now.AddDays(rand.Next(1, 30))),
            true,
            AppDate.GetId(DateTime.Now),
            AppDate.GetId(DateTime.Now.AddDays(rand.Next(1, 30))),
            true,
            AppDate.GetId(DateTime.Now),
            AppDate.GetId(DateTime.Now.AddDays(rand.Next(1, 30))),
            3,
            1,
            PlanAcceptEndLimitTypes.AfterDays,
            1,
            TimeSpan.Zero,
            siteIds
        );

        var response = await Client.PatchAsync(
            $"{baseUrl}/{id}/publish-accept",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private async Task PlanEnableAsync(
        long id
    )
    {
        var baseUrl = "api/plans";
        var enableReq = new PlanEnabledRequest(true);

        var response = await Client.PatchAsync(
            $"{baseUrl}/{id}/enabled",
            TestUtil.ToJsonContent(enableReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private async Task PlanUpdateDisplayAsync(
        long id,
        List<long> categoryIds
    )
    {
        var baseUrl = "api/plans";
        var updateReq = new PlanUpdateDisplayRequest(null, categoryIds, null);

        var response = await Client.PatchAsync(
            $"{baseUrl}/{id}/display",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private async Task PlanUpdateRoomAsync(
        long id,
        List<long> roomIds
    )
    {
        var baseUrl = "api/plans";
        var updateReq = new PlanUpdateRoomTypeRequest(roomIds);

        var response = await Client.PatchAsync(
            $"{baseUrl}/{id}/room-type",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private async Task PlanEnableRoomAsync(
        long id,
        List<long> roomIds
    )
    {
        var baseUrl = "api/plans";
        var updateReq = new PlanEnabledRoomTypeRequest(true);

        foreach (var roomId in roomIds)
        {
            var response = await Client.PatchAsync(
                $"{baseUrl}/{id}/room-groups/{roomId}/enabled",
                TestUtil.ToJsonContent(updateReq)
            );

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }

    private async Task<List<long>> CreateSitesAsync()
    {
        var destinationRepo = Factory.GetRequiredService<IFacilitySiteRepository>()
            ?? throw new ArgumentException(nameof(IFacilitySiteRepository));

        var destinationsToCreate = new List<FacilitySite>
        {
            new()
            {
                FacilityId = FacilityInfo.Id,
                Site = new SiteEntity
                {
                    Code = "sitetest",
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
                    Description = "Site description",
                    IsEnabled = true
                },
                IsEnabled = true
            }
        };

        var createdDestinations = await destinationRepo.AddRangeAsync(
            destinationsToCreate,
            true
        );

        return [.. createdDestinations.Select(x => x.Site!.Id)];
    }

    private async Task<List<long>> CreateRoomGroupsAsync(
        List<long> roomCategoryIds,
        List<long> siteIds
    )
    {
        var baseUrl = "api/room-groups";

        List<long> ids = [];
        for (var i = 0; i < 5; i++)
        {
            var createReq = new RoomGroupCreateRequest(
                $"Room group {i + 1}",
                $"Room group {i + 1} Description",
                1,
                600,
                10
            );

            var response = await Client.PostAsync(
                baseUrl,
                TestUtil.ToJsonContent(createReq)
            );
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();

            _ = long.TryParse(responseString, out var id);
            ids.Add(id);

            var enableReq = new RoomGroupEnabledRequest(true) { Id = id };

            response = await Client.PatchAsync(
                $"{baseUrl}/{id}/enable",
                TestUtil.ToJsonContent(enableReq)
            );
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            await AdjustAppDatesOfRoomGroup(id);
            await RoomUpdateDisplayAsync(id, roomCategoryIds);
            await RoomUpdatePublishAcceptAsync(id, siteIds);
        }

        return ids;
    }

    private async Task AdjustAppDatesOfRoomGroup(
        long roomGroupId
    )
    {
        var baseUrl = "api/room-group-inventory";

        var putRequest = new List<RoomGroupChangeRemainRequest>
        {
            new(
                AppDate.GetId(DateTime.UtcNow),
                roomGroupId,
                3,
                false
            ),
            new(
                AppDate.GetId(DateTime.UtcNow.AddDays(1)),
                roomGroupId,
                3,
                false
            )
        };

        var response = await Client.PutAsync(
            baseUrl,
            TestUtil.ToJsonContent(putRequest)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private async Task<List<long>> CreateQuestionsAsync()
    {
        var baseUrl = "api/questions";

        List<long> ids = [];
        for (var i = 0; i < 5; i++)
        {
            var createReq = new QuestionCreateRequest(
                $"Question {i + 1}",
                $"Question {i + 1} Description"
            );

            var response = await Client.PostAsync(
                baseUrl,
                TestUtil.ToJsonContent(createReq)
            );
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();
            _ = long.TryParse(responseString, out var id);
            ids.Add(id);

            var updateReq = new QuestionUpdateRequest(
                createReq.Name,
                createReq.Description,
                "{\"type\":\"Text\",\"data\":[],\"selected\":\"\",\"isRequired\":false,\"selectionMin\":null,\"selectionMax\":null,\"placeholder\":\"\",\"valueLabel\":\"\"}",
                QuestionTypes.Text
            );
            await Client.PutAsync(
                $"{baseUrl}/{id}",
                TestUtil.ToJsonContent(updateReq)
            );

            var enableReq = new QuestionEnabledRequest(true) { Id = id };

            response = await Client.PatchAsync(
                $"{baseUrl}/{id}/enable",
                TestUtil.ToJsonContent(enableReq)
            );
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        return ids;
    }

    private async Task<List<long>> CreateCancellationsAsync()
    {
        var baseUrl = "api/cancellation-policies";
        List<long> ids = [];
        for (var i = 0; i < 5; i++)
        {
            var cancellation = new CancellationPolicyCreateRequest(
                $"Cancellation {i + 1}",
                $"Cancel {i + 1} Description"
            );

            var response = await Client.PostAsync(
                baseUrl,
                TestUtil.ToJsonContent(cancellation)
            );
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();

            _ = long.TryParse(responseString, out var id);
            ids.Add(id);

            var enableReq = new CancellationPolicyEnabledRequest(true);

            response = await Client.PatchAsync(
                $"{baseUrl}/{id}/enable",
                TestUtil.ToJsonContent(enableReq)
            );
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        return ids;
    }

    private async Task<string> ExportDatabaseAsync()
    {
        // Get the directory where the executable is located
        var baseDirectory = AppContext.BaseDirectory;

        // Traverse up to the project root by going up a few levels
        var projectRootDirectory = Directory.GetParent(baseDirectory)?.Parent?.Parent?.Parent?.FullName;

        var exportDirectoryPath = Path.Combine(projectRootDirectory!, "Dumps");

        if (!Directory.Exists(exportDirectoryPath))
        {
            Directory.CreateDirectory(exportDirectoryPath);
        }

        const string fileExtension = ".sql";
        const string fileName = "data-dump" + fileExtension;

        var exportFilePath = Path.GetFullPath(
            Path.Combine(
                exportDirectoryPath,
                fileName
            )
        );
        var containerId = Factory.GetContainerName();

        var arguments = $"exec {containerId} pg_dump -U postgres "
            + $"-d {MockPostgreSqlContainer.ReservationDatabase} -F p -f /tmp/backup.sql";

        var processInfo = new ProcessStartInfo
        {
            FileName = "docker",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            EnvironmentVariables = { ["PGPASSWORD"] = "postgres" }
        };

        using var process = new Process();
        process.StartInfo = processInfo;

        try
        {
            process.Start();

            var error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                Console.WriteLine($"Database export failed: {error}");
            }
            else
            {
                var copyFileCommand = $"cp {containerId}:/tmp/backup.sql {exportFilePath}";
                var copyProcess = Process.Start("docker", copyFileCommand);
                await copyProcess.WaitForExitAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during export: {ex.Message}");
        }

        return exportFilePath;
    }

    private async Task<IEnumerable<PersonAgeType>> CreatePersonAgeTypesAsync()
    {
        var personAgeTypeRepo = Factory.GetRequiredService<IFacilityPersonAgeTypeRepository>()
            ?? throw new ArgumentException(nameof(IFacilityPersonAgeTypeRepository));

        var existingPersonAgeTypes = personAgeTypeRepo
            .GetQueryableWithAsNoTracking()
            .Select(x => x.PersonAgeType)
            .ToList();

        if (existingPersonAgeTypes is { Count: > 0 })
        {
            return existingPersonAgeTypes!;
        }

        var personAgeTypesToCreate = new List<FacilityPersonAgeType>
        {
            new()
            {
                FacilityId = FacilityInfo.Id,
                PersonAgeType = new()
                {
                    Code = "A",
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "大人" } },
                    AgeMin = 18,
                    AgeMax = null,
                    IsMain = true,
                    IsMaster = true,
                    PersonAgeTypeSpaTaxDatas =
                    [
                        new()
                        {
                            SpaTaxData = new()
                            {
                                PriceMin = 0,
                                PriceMax = 999999,
                                Tax = 200
                            }
                        }
                    ],
                    Meta = new()
                    {
                        PersonAgeGroup = PersonAgeGroups.Adult,
                        FoodBed = FoodBeds.None
                    },
                    IsEnabled = true
                },
                IsEnabled = true
            },
            new()
            {
                FacilityId = FacilityInfo.Id,
                PersonAgeType = new()
                {
                    Code = "B",
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "子供" } },
                    AgeMin = 6,
                    AgeMax = 17,
                    IsMain = false,
                    IsMaster = true,
                    PersonAgeTypeSpaTaxDatas =
                    [
                        new()
                        {
                            SpaTaxData = new()
                            {
                                PriceMin = 0,
                                PriceMax = 999999,
                                Tax = 0
                            }
                        }
                    ],
                    Meta = new()
                    {
                        PersonAgeGroup = PersonAgeGroups.Teen,
                        FoodBed = FoodBeds.None
                    },
                    IsEnabled = true
                },
                IsEnabled = true
            },
            new()
            {
                FacilityId = FacilityInfo.Id,
                PersonAgeType = new()
                {
                    Code = "C",
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "幼児(食事有・布団有)" } },
                    AgeMin = 0,
                    AgeMax = 5,
                    IsMain = false,
                    IsMaster = true,
                    PersonAgeTypeSpaTaxDatas =
                    [
                        new()
                        {
                            SpaTaxData = new()
                            {
                                PriceMin = 0,
                                PriceMax = 999999,
                                Tax = 0
                            }
                        }
                    ],
                    Meta = new()
                    {
                        PersonAgeGroup = PersonAgeGroups.TeenB,
                        FoodBed = FoodBeds.Bed
                    },
                    IsEnabled = true
                },
                IsEnabled = true
            },
            new()
            {
                FacilityId = FacilityInfo.Id,
                PersonAgeType = new()
                {
                    Code = "D",
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "幼児(食事有・布団無)" } },
                    AgeMin = 0,
                    AgeMax = 5,
                    IsMain = false,
                    IsMaster = true,
                    PersonAgeTypeSpaTaxDatas =
                    [
                        new()
                        {
                            SpaTaxData = new()
                            {
                                PriceMin = 0,
                                PriceMax = 999999,
                                Tax = 0
                            }
                        }
                    ],
                    Meta = new()
                    {
                        PersonAgeGroup = PersonAgeGroups.TeenB,
                        FoodBed = FoodBeds.Food
                    },
                    IsEnabled = true
                },
                IsEnabled = true
            },
            new()
            {
                FacilityId = FacilityInfo.Id,
                PersonAgeType = new()
                {
                    Code = "E",
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "幼児(食事無・布団有)" } },
                    AgeMin = 0,
                    AgeMax = 5,
                    IsMain = false,
                    IsMaster = true,
                    PersonAgeTypeSpaTaxDatas =
                    [
                        new()
                        {
                            SpaTaxData = new()
                            {
                                PriceMin = 0,
                                PriceMax = 999999,
                                Tax = 0
                            }
                        }
                    ],
                    Meta = new()
                    {
                        PersonAgeGroup = PersonAgeGroups.TeenB,
                        FoodBed = FoodBeds.Bed
                    },
                    IsEnabled = true
                },
                IsEnabled = true
            },
            new()
            {
                FacilityId = FacilityInfo.Id,
                PersonAgeType = new()
                {
                    Code = "F",
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "幼児(食事無・布団無)" } },
                    AgeMin = 0,
                    AgeMax = 5,
                    IsMain = false,
                    IsMaster = true,
                    PersonAgeTypeSpaTaxDatas =
                    [
                        new()
                        {
                            SpaTaxData = new()
                            {
                                PriceMin = 0,
                                PriceMax = 999999,
                                Tax = 0
                            }
                        }
                    ],
                    Meta = new()
                    {
                        PersonAgeGroup = PersonAgeGroups.TeenB,
                        FoodBed = FoodBeds.None
                    },
                    IsEnabled = true
                },
                IsEnabled = true
            }
        };

        var createdPersonAgeTypes = await personAgeTypeRepo.AddRangeAsync(
            personAgeTypesToCreate,
            true
        );

        return createdPersonAgeTypes.Select(x => x.PersonAgeType!);
    }

    private async Task<IEnumerable<AppDateType>> CreateAppDateTypesAsync()
    {
        var appDateTypeRepo = Factory.GetRequiredService<IFacilityAppDateTypeRepository>()
            ?? throw new ArgumentException(nameof(IFacilityAppDateTypeRepository));

        var appDateTypesToCreate = new List<FacilityAppDateType>();
        for (var i = 1; i <= 5; i++)
        {
            appDateTypesToCreate.Add(
                new FacilityAppDateType
                {
                    FacilityId = FacilityInfo.Id,
                    AppDateType = new AppDateType
                    {
                        Name = $"App date type {i}",
                        ShortName = $"App date type {i}",
                        Description = $"App date type description {i}",
                        IsEnabled = true
                    },
                    IsEnabled = true
                }
            );
        }

        var createdAppDateTypes = await appDateTypeRepo.AddRangeAsync(
            appDateTypesToCreate,
            true
        );

        return createdAppDateTypes.Select(x => x.AppDateType!);
    }

    private async Task UpdateDiscountOfRoomAsync(
        List<long> roomIds,
        long siteId
    )
    {
        foreach (var roomId in roomIds)
        {
            var response = await Client.PatchAsync(
                $"/api/room-group-prices/{roomId}/destinations/{siteId}/discount",
                TestUtil.ToJsonContent(
                    new RoomGroupPriceUpdateDiscountRequest(
                        [
                            new(
                                1,
                                2,
                                1,
                                10,
                                PriceSettingTypes.Price,
                                50
                            ),
                            new(
                                3,
                                5,
                                1,
                                10,
                                PriceSettingTypes.Price,
                                50
                            ),
                            new(
                                6,
                                9,
                                1,
                                10,
                                PriceSettingTypes.Price,
                                50
                            )
                        ]
                    )
                )
            );
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }

    private async Task UpdatePriceCalendarOfRoomAsync(
        List<long> roomIds,
        long siteId
    )
    {
        foreach (var roomId in roomIds)
        {
            var response = await Client.PatchAsync(
                $"/api/room-group-prices/{roomId}/destinations/{siteId}/price-calendar",
                TestUtil.ToJsonContent(
                    new RoomGroupPriceUpdatePriceCalendarRequest(
                        [
                            new(
                                AppDate.GetId(DateTime.Now),
                                1,
                                4,
                                400,
                                false
                            ),
                            new(
                                AppDate.GetId(DateTime.Now.AddDays(1)),
                                1,
                                10,
                                450,
                                false
                            )
                        ]
                    )
                )
            );
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }

    private async Task UpdateSaleSettingOfRoomAsync(
        List<long> roomIds,
        long siteId
    )
    {
        foreach (var roomId in roomIds)
        {
            var response = await Client.PatchAsync(
                $"/api/room-group-prices/{roomId}/destinations/{siteId}/sale",
                TestUtil.ToJsonContent(
                    new RoomGroupPriceUpdateSaleRequest
                    {
                        UseAutoExtend = false,
                        AutoExtendMonth = 15,
                        AutoExtendEveryMonthDay = 3
                    }
                )
            );
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }

    private async Task UpdateChildrenPriceOfRoomAsync(
        List<long> roomIds,
        List<PersonAgeType> personAgeTypes,
        long siteId
    )
    {
        foreach (var roomId in roomIds)
        {
            List<RoomTypeUpdateChildrenPersonAgeTypeRequest> reqChildrenPrices = [];
            personAgeTypes.ForEach(
                personAgeType =>
                {
                    reqChildrenPrices.Add(
                        new RoomTypeUpdateChildrenPersonAgeTypeRequest(
                            personAgeType.Id,
                            true,
                            true,
                            PriceSettingTypes.Discount,
                            200
                        )
                    );
                }
            );

            var response = await Client.PatchAsync(
                $"/api/room-group-prices/{roomId}/destinations/{siteId}/children-price",
                TestUtil.ToJsonContent(
                    new RoomGroupPriceUpdateChildrenPriceRequest(
                        reqChildrenPrices
                    )
                )
            );
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }

    private async Task CreateSiteInRoomOfRoomAsync(
        List<long> roomIds,
        long siteId
    )
    {
        foreach (var roomId in roomIds)
        {
            var response = await Client.PostAsync(
                $"/api/room-group-prices/{roomId}/destinations/{siteId}",
                null
            );
            response.EnsureSuccessStatusCode();
        }
    }

    private async Task UpdateStandardPriceOfRoomAsync(
        List<long> roomIds,
        long siteId
    )
    {
        var planRepository = Factory.GetRequiredService<IPlanRepository>()
            ?? throw new ArgumentException(nameof(IPlanRepository));
        var planCreate = new Plan
        {
            Code = "test",
            Description = new MultilingualText(new Dictionary<string, string> { { TestUtil.DefaultLanguageCode, "標準" } }),
            PlanType = PlanTypes.RoomOnly
        };
        var plan = await planRepository.AddAsync(planCreate, true);
        var facilityPlanRepository = Factory.GetRequiredService<IFacilityPlanRepository>()
            ?? throw new ArgumentException(nameof(IFacilityPlanRepository));
        var facilityPlanCreate = new FacilityPlan
        {
            PlanId = plan.Id,
            FacilityId = FacilityInfo.Id,
            IsEnabled = true
        };
        await facilityPlanRepository.AddAsync(facilityPlanCreate, true);
        var planRoomRepository = Factory.GetRequiredService<IPlanRoomGroupRepository>();
        var planRoomSiteRepository = Factory.GetRequiredService<IPlanRoomGroupSiteRepository>();

        var appDateTypesList = (await CreateAppDateTypesAsync()).ToList();

        foreach (var roomId in roomIds)
        {
            var planRoomGroupCreate = new PlanRoomGroup
            {
                PlanId = plan.Id,
                RoomGroupId = roomId,
                IsEnabled = true
            };
            await planRoomRepository!.AddAsync(planRoomGroupCreate, true);
            var planRoomGroupSiteCreate = new PlanRoomGroupSite
            {
                PlanId = plan.Id,
                RoomGroupId = roomId,
                SiteId = siteId,
                IsEnabled = true,
                IsEnabledMinimumPrice = false,
                MinimumPrice = null
            };
            await planRoomSiteRepository!.AddAsync(planRoomGroupSiteCreate, true);

            List<RomTypeUpdateStandardRequest> reqStandardPrices = [];
            appDateTypesList.ForEach(
                appDateType =>
                {
                    reqStandardPrices.Add(
                        new RomTypeUpdateStandardRequest(
                            appDateType.Id,
                            1,
                            4,
                            400
                        )
                    );

                    reqStandardPrices.Add(
                        new RomTypeUpdateStandardRequest(
                            appDateType.Id,
                            1,
                            10,
                            450
                        )
                    );

                    reqStandardPrices.Add(
                        new RomTypeUpdateStandardRequest(
                            appDateType.Id,
                            1,
                            10,
                            450
                        )
                    );
                }
            );

            var response = await Client.PatchAsync(
                $"/api/room-group-prices/{roomId}/destinations/{siteId}/standard-price",
                TestUtil.ToJsonContent(
                    new RoomGroupPriceUpdateStandardPriceRequest(
                        reqStandardPrices
                    )
                )
            );
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }

    private async Task RoomUpdatePaymentMethodAsync(
        long id
    )
    {
        var baseUrl = "api/room-groups";

        var updateReq = new RoomGroupUpdatePaymentMethodRequest(true, true);

        var response = await Client.PatchAsync(
            $"{baseUrl}/{id}/payment-method",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private async Task RoomUpdateQuestionAsync(
        long id,
        List<long> questionIds
    )
    {
        var baseUrl = "api/room-groups";
        var updateReq = new RoomGroupUpdateQuestionRequest(
            questionIds
        );

        var response = await Client.PatchAsync(
            $"{baseUrl}/{id}/question",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private async Task RoomUpdateCancelAsync(
        long id,
        long cancellationId
    )
    {
        var baseUrl = "api/room-groups";
        var updateReq = new RoomGroupUpdateCancelRequest(
            true,
            5,
            new TimeSpan(12, 0, 0),
            cancellationId
        );

        var response = await Client.PatchAsync(
            $"{baseUrl}/{id}/cancel",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private async Task RoomUpdateOptionAsync(
        long id,
        List<long> optionIds
    )
    {
        var baseUrl = "api/room-groups";
        var updateReq = new RoomGroupUpdateOptionRequest(
            true,
            optionIds
        );

        var response = await Client.PatchAsync(
            $"{baseUrl}/{id}/option",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private async Task RoomUpdateSaleAsync(
        long id
    )
    {
        var baseUrl = "api/room-groups";
        var updateReq = new RoomGroupUpdateSaleRequest(
            TimeSpan.FromHours(1),
            TimeSpan.FromHours(3),
            TimeSpan.FromHours(3),
            0,
            10,
            1,
            PlanDaySaleLimitTypes.RoomGroup,
            true,
            1,
            10,
            true,
            1,
            5,
            7,
            3
        );

        var response = await Client.PatchAsync(
            $"{baseUrl}/{id}/sale",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private async Task RoomUpdatePublishAcceptAsync(
        long id,
        List<long> siteIds
    )
    {
        var baseUrl = "api/room-groups";
        var rand = new Random();
        var updateReq = new RoomGroupUpdatePublicationSettingRequest(
            [.. siteIds],
            true,
            AppDate.GetId(DateTime.Now),
            AppDate.GetId(DateTime.Now.AddDays(rand.Next(1, 30))),
            true,
            AppDate.GetId(DateTime.Now),
            AppDate.GetId(DateTime.Now.AddDays(rand.Next(1, 30))),
            3,
            1,
            PlanAcceptEndLimitTypes.AfterDays,
            1,
            TimeSpan.Zero
        );

        var response = await Client.PatchAsync(
            $"{baseUrl}/{id}/publication-setting",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private async Task RoomUpdateDisplayAsync(
        long id,
        List<long> categoryIds
    )
    {
        var baseUrl = "api/room-groups";
        var updateReq = new RoomGroupUpdateDisplaySettingRequest(
            null,
            [.. categoryIds],
            null,
            null,
            null,
            null
        );

        var response = await Client.PatchAsync(
            $"{baseUrl}/{id}/display-setting",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private async Task<List<CancellationData>> CreateCancellationDataAsync()
    {
        var destinationRepo = Factory.GetRequiredService<ICancellationDataRepository>()
            ?? throw new ArgumentException(nameof(ICancellationDataRepository));

        var destinationsToCreate = new List<CancellationData>
        {
            new()
            {
                Code = "1",
                DayEnd = 20251203,
                DayStart = 20250103,
                Description = null,
                Rate = 6,
                IsEnabled = true
            }
        };

        var createdDestinations = await destinationRepo.AddRangeAsync(
            destinationsToCreate,
            true
        );

        return [.. createdDestinations];
    }

    private async Task CreateDataOfCancellationDataAsync(
        List<long> cancellationIds,
        List<CancellationData> cancellationDataIds
    )
    {
        var destinationService = Factory.GetRequiredService<IDataOfCancellationService>()
            ?? throw new ArgumentException(nameof(IDataOfCancellationService));
        foreach (var cancellationId in cancellationIds)
        {
            await destinationService.CreateRangeAsync(cancellationId, cancellationDataIds);
        }
    }

    private async Task CreateSystemConfigAsync()
    {
        var systemConfigRepository = Factory.GetRequiredService<ISystemConfigRepository>()
            ?? throw new ArgumentException(nameof(ISystemConfigRepository));
        var systemConfig = new SystemConfig
        {
            CanOnlinePayment = true,
            IsEnabled = true,
            Code = "Test",
            TemplateFormatData = null
        };
        await systemConfigRepository.AddAsync(systemConfig, true);
    }

    private async Task CreatePlanRoomGroupSitePriceAsync(
        long planId,
        List<long> roomGroupIds,
        long siteId,
        CancellationToken cancellationToken = default
    )
    {
        var planRoomGroupSitePriceRepository = Factory.GetRequiredService<IPlanRoomGroupSiteRepository>()
            ?? throw new ArgumentException(nameof(IPlanRoomGroupSiteRepository));
        var planRoomGroupSites = new List<PlanRoomGroupSite>();
        foreach (var roomGroupId in roomGroupIds)
        {
            var planRoomGroupSite = new PlanRoomGroupSite
            {
                PlanId = planId,
                RoomGroupId = roomGroupId,
                SiteId = siteId,
                IsEnabled = true,
                IsEnabledMinimumPrice = false,
                MinimumPrice = null
            };
            planRoomGroupSites.Add(planRoomGroupSite);
        }

        await planRoomGroupSitePriceRepository.AddRangeAsync(planRoomGroupSites, true, cancellationToken);
    }
}
