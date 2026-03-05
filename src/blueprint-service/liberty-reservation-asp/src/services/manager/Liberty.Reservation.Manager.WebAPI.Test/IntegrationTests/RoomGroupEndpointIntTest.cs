using System.Net;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class RoomGroupEndpointIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/room-groups";

    private async Task<string> CreateRoomGroupAsync()
    {
        var createReq = new RoomGroupCreateRequest(
            "Room group 1",
            "Room group 1 Description",
            4,
            600,
            10
        );

        var response = await Client.PostAsync(
            BaseUrl,
            TestUtil.ToJsonContent(createReq)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        return responseString;
    }

    [Fact]
    public async Task CreateRoomGroup_ReturnsOk_WithRoomGroup()
    {
        var responseString = await CreateRoomGroupAsync();
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task OrderRoomGroup_Return_NoContent()
    {
        var newRoom1 = await CreateRoomGroupAsync();
        _ = long.TryParse(newRoom1, out var newRoomId1);

        var newRoom2 = await CreateRoomGroupAsync();
        _ = long.TryParse(newRoom2, out var newRoomId2);

        var orderRequest = new ItemUpdateOrderRequest(
            [
                newRoomId1, newRoomId2
            ]
        );

        var response = await Client.PutAsync(
            $"{BaseUrl}/order",
            TestUtil.ToJsonContent(orderRequest)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBasicConfigurationOfRoomGroup_ReturnsNoContent()
    {
        var newRoomGroupResponse = await CreateRoomGroupAsync();
        var newRoomGroupId = long.TryParse(newRoomGroupResponse, out var id) ? id : 0;

        var bedTypeRepo = Factory.GetRequiredService<IBedTypeRepository>();
        var addBedTypes = await bedTypeRepo!.AddRangeAsync(
            new List<BedType>
            {
                new() { Name = "Bed 1" },
                new() { Name = "Bed 2" },
                new() { Name = "Bed 3" },
                new() { Name = "Bed 4" },
                new() { Name = "Bed 5" },
                new() { Name = "Bed 6" },
                new() { Name = "Bed 7" }
            },
            true
        );

        var updateReq = new RoomGroupUpdateBasicConfigurationRequest(
            "Room group",
            "Group name",
            "Overview",
            "Summary",
            "Description",
            1,
            2,
            3,
            1,
            RoomGroupSizeUnitTypes.M2,
            addBedTypes.Select(
                x => new BedTypeOfRoomGroupUpdateBasicConfigurationRequest(x.Id, 50)
            ),
            false,
            true,
            true,
            true,
            true,
            []
        ) { Id = newRoomGroupId };

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{newRoomGroupId}/basic-configuration",
            TestUtil.ToJsonContent(updateReq)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePublicationSettingOfRoomGroup_ReturnsNoContent()
    {
        var newRoomGroupResponse = await CreateRoomGroupAsync();
        var newRoomGroupId = long.TryParse(newRoomGroupResponse, out var id) ? id : 0;

        var updateReq = new RoomGroupUpdatePublicationSettingRequest(
            [],
            true,
            1,
            2,
            true,
            1,
            2,
            3,
            1,
            PlanAcceptEndLimitTypes.AfterDays,
            1,
            TimeSpan.Zero
        ) { Id = newRoomGroupId };

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{newRoomGroupId}/publication-setting",
            TestUtil.ToJsonContent(updateReq)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateDisplaySettingOfRoomGroup_ReturnsNoContent()
    {
        var newRoomGroupResponse = await CreateRoomGroupAsync();
        var newRoomGroupId = long.TryParse(newRoomGroupResponse, out var id) ? id : 0;

        var categoryRepo = Factory.GetRequiredService<ICategoryRepository>();

        var roomGroupMasterCategoryIds = categoryRepo!.GetQueryable()
            .Where(x => x.IsMaster && x.IsEnabled && x.CategoryType == CategoryTypes.RoomGroup)
            .Select(x => x.Id)
            .AsEnumerable();

        var roomGroupCategoryIds = categoryRepo.GetQueryable()
            .Where(x => !x.IsMaster && x.IsEnabled && x.CategoryType == CategoryTypes.RoomGroup)
            .Select(x => x.Id)
            .AsEnumerable();

        var roomGroupFeatureCategoryIds = categoryRepo.GetQueryable()
            .Where(x => x.IsEnabled && x.CategoryType == CategoryTypes.RoomGroupFeature)
            .Select(x => x.Id)
            .AsEnumerable();

        var roomGroupEquipmentCategoryIds = categoryRepo.GetQueryable()
            .Where(x => x.IsEnabled && x.CategoryType == CategoryTypes.RoomGroupEquipment)
            .Select(x => x.Id)
            .AsEnumerable();

        var roomGroupAmenityCategoryIds = categoryRepo.GetQueryable()
            .Where(x => x.IsEnabled && x.CategoryType == CategoryTypes.Amenity)
            .Select(x => x.Id)
            .AsEnumerable();

        var updateReq = new RoomGroupUpdateDisplaySettingRequest(
            [.. roomGroupMasterCategoryIds],
            [.. roomGroupCategoryIds],
            [.. roomGroupFeatureCategoryIds],
            [.. roomGroupEquipmentCategoryIds],
            [.. roomGroupAmenityCategoryIds],
            []
        ) { Id = newRoomGroupId };

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{newRoomGroupId}/display-setting",
            TestUtil.ToJsonContent(updateReq)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task EnableRoomGroup_ReturnsNoContent()
    {
        var newRoomGroupResponse = await CreateRoomGroupAsync();
        var newRoomGroupId = long.TryParse(newRoomGroupResponse, out var id) ? id : 0;

        var updateReq = new RoomGroupEnabledRequest(
            true
        ) { Id = newRoomGroupId };

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{newRoomGroupId}/enable",
            TestUtil.ToJsonContent(updateReq)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteRoomGroup_ReturnsNoContent()
    {
        var newRoomGroupResponse = await CreateRoomGroupAsync();
        var newRoomGroupId = long.TryParse(newRoomGroupResponse, out var id) ? id : 0;

        var response = await Client.DeleteAsync(
            $"{BaseUrl}/{newRoomGroupId}"
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetAllRoomGroups_ReturnOk_WithRoomGroupResponse()
    {
        _ = await CreateRoomGroupAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}"
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetAllEnableRoomGroups_ReturnOk_WithRoomGroupResponse()
    {
        _ = await CreateRoomGroupAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/?enable=true"
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetBasicConfigurationOfRoomGroup_ReturnOk_WithRoomGroupDetailBasicConfigurationResponse()
    {
        var newRoomGroupResponse = await CreateRoomGroupAsync();
        var newRoomGroupId = long.TryParse(newRoomGroupResponse, out var id) ? id : 0;

        var bedTypeRepo = Factory.GetRequiredService<IBedTypeRepository>();
        var addBedTypes = await bedTypeRepo!.AddRangeAsync(
            new List<BedType>
            {
                new() { Name = "Bed 1" },
                new() { Name = "Bed 2" },
                new() { Name = "Bed 3" },
                new() { Name = "Bed 4" },
                new() { Name = "Bed 5" },
                new() { Name = "Bed 6" },
                new() { Name = "Bed 7" }
            },
            true
        );

        var updateReq = new RoomGroupUpdateBasicConfigurationRequest(
            "Room group",
            "Group name1",
            "Overview",
            "Summary",
            "Description",
            1,
            2,
            3,
            1,
            RoomGroupSizeUnitTypes.M2,
            addBedTypes.Select(
                x => new BedTypeOfRoomGroupUpdateBasicConfigurationRequest(x.Id, 50)
            ),
            false,
            true,
            true,
            true,
            true,
            []
        ) { Id = newRoomGroupId };

        var updateResponse = await Client.PatchAsync(
            $"{BaseUrl}/{newRoomGroupId}/basic-configuration",
            TestUtil.ToJsonContent(updateReq)
        );
        updateResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var response = await Client.GetAsync(
            $"{BaseUrl}/{newRoomGroupId}/basic-configuration"
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetPublicationSettingOfRoomGroup_ReturnOk_WithRoomGroupDetailPublicationSettingResponse()
    {
        var newRoomGroupResponse = await CreateRoomGroupAsync();
        var newRoomGroupId = long.TryParse(newRoomGroupResponse, out var id) ? id : 0;

        var updateReq = new RoomGroupUpdatePublicationSettingRequest(
            [],
            true,
            1,
            2,
            true,
            1,
            2,
            3,
            1,
            PlanAcceptEndLimitTypes.AfterDays,
            1,
            TimeSpan.Zero
        ) { Id = newRoomGroupId };

        var updateResponse = await Client.PatchAsync(
            $"{BaseUrl}/{newRoomGroupId}/publication-setting",
            TestUtil.ToJsonContent(updateReq)
        );
        updateResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var response = await Client.GetAsync(
            $"{BaseUrl}/{newRoomGroupId}/publication-setting"
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetDisplaySettingOfRoomGroup_ReturnOk_WithRoomGroupDetailPublicationSettingResponse()
    {
        var newRoomGroupResponse = await CreateRoomGroupAsync();
        var newRoomGroupId = long.TryParse(newRoomGroupResponse, out var id) ? id : 0;

        var categoryRepo = Factory.GetRequiredService<ICategoryRepository>();

        var roomGroupMasterCategoryIds = categoryRepo!.GetQueryable()
            .Where(x => x.IsMaster && x.IsEnabled && x.CategoryType == CategoryTypes.RoomGroup)
            .Select(x => x.Id)
            .AsEnumerable();

        var roomGroupCategoryIds = categoryRepo.GetQueryable()
            .Where(x => !x.IsMaster && x.IsEnabled && x.CategoryType == CategoryTypes.RoomGroup)
            .Select(x => x.Id)
            .AsEnumerable();

        var roomGroupFeatureCategoryIds = categoryRepo.GetQueryable()
            .Where(x => x.IsEnabled && x.CategoryType == CategoryTypes.RoomGroupFeature)
            .Select(x => x.Id)
            .AsEnumerable();

        var roomGroupEquipmentCategoryIds = categoryRepo.GetQueryable()
            .Where(x => x.IsEnabled && x.CategoryType == CategoryTypes.RoomGroupEquipment)
            .Select(x => x.Id)
            .AsEnumerable();

        var roomGroupAmenityCategoryIds = categoryRepo.GetQueryable()
            .Where(x => x.IsEnabled && x.CategoryType == CategoryTypes.Amenity)
            .Select(x => x.Id)
            .AsEnumerable();

        var updateReq = new RoomGroupUpdateDisplaySettingRequest(
            [.. roomGroupMasterCategoryIds],
            [.. roomGroupCategoryIds],
            [.. roomGroupFeatureCategoryIds],
            [.. roomGroupEquipmentCategoryIds],
            [.. roomGroupAmenityCategoryIds],
            []
        ) { Id = newRoomGroupId };

        var updateResponse = await Client.PatchAsync(
            $"{BaseUrl}/{newRoomGroupId}/display-setting",
            TestUtil.ToJsonContent(updateReq)
        );
        updateResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var response = await Client.GetAsync(
            $"{BaseUrl}/{newRoomGroupId}/display-setting"
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetAllPublishedInOfRoomGroup_ReturnOk_WithSiteOfRoomGroupDetailPublicationSettingResponse()
    {
        var newRoomGroupResponse = await CreateRoomGroupAsync();
        var newRoomGroupId = long.TryParse(newRoomGroupResponse, out var id) ? id : 0;

        var response = await Client.GetAsync(
            $"{BaseUrl}/{newRoomGroupId}/published-in"
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }
}
