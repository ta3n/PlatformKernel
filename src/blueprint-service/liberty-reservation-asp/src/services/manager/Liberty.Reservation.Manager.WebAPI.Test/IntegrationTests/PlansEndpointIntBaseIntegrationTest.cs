using System.Net;
using Liberty.ApplicationShared.Utils;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class PlansEndpointIntBaseIntegrationTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/plans";

    private async Task<Plan> CreatePlanAsync()
    {
        var planRepo = Factory.GetRequiredService<IPlanRepository>();
        var facilityPlanService = Factory.GetRequiredService<IFacilityPlanService>();
        var existingPlan = await planRepo!.GetAllAsync();
        if (existingPlan.Any())
        {
            return existingPlan[0];
        }

        var mockData = new Plan
        {
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Name" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Description" } },
            IsEnabled = true
        };
        var newPlan = await planRepo.AddAsync(mockData, true);

        var mockFacilityPlan = new FacilityPlan
        {
            PlanId = newPlan.Id,
            FacilityId = FacilityInfo.Id,
            IsEnabled = true
        };
        _ = await facilityPlanService!.CreateAsync(mockFacilityPlan);

        return newPlan;
    }

    private async Task<RoomGroup> CreateRoomGroupAsync()
    {
        var roomGroupRepo = Factory.GetRequiredService<IRoomGroupRepository>();
        var existingRoomGroup = await roomGroupRepo!.GetAllAsync();
        if (existingRoomGroup.Any())
        {
            return existingRoomGroup[0];
        }

        var mockData = new RoomGroup
        {
            Code = EntityUtil.CreateCode(),
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Name}" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Description" } },
            IsEnabled = true
        };

        var newRoomGroup = await roomGroupRepo.AddAsync(mockData, true);

        return newRoomGroup;
    }

    private async Task<Site> CreateSiteAsync()
    {
        var siteRepo = Factory.GetRequiredService<ISiteRepository>();
        var exitingSite = await siteRepo!.GetAllAsync();
        if (exitingSite.Any())
        {
            return exitingSite[0];
        }

        var mockData = new Site
        {
            Code = EntityUtil.CreateCode(),
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Name" } },
            Description = "Description",
            Url = "test.com",
            IsEnabled = true
        };

        var newSite = await siteRepo.AddAsync(mockData, true);

        return newSite;
    }

    [Fact]
    public async Task CreatePlan_ReturnOK()
    {
        var createReq = new PlanCreateRequest(
            "Name",
            "Description",
            false,
            PlanTypes.Combo
        );

        var response = await Client.PostAsync(BaseUrl, TestUtil.ToJsonContent(createReq));
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task OrderPlan_Return_NoContent()
    {
        var planRepo = Factory.GetRequiredService<IPlanRepository>();
        var facilityPlanService = Factory.GetRequiredService<IFacilityPlanService>();
        var mockData = new Plan
        {
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Name" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Description" } },
            IsEnabled = true
        };
        var newPlan1 = await planRepo!.AddAsync(mockData, true);
        var mockData2 = new Plan
        {
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Name" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Description" } },
            IsEnabled = true
        };
        var newPlan2 = await planRepo.AddAsync(mockData2, true);

        var mockFacilityPlan1 = new FacilityPlan
        {
            PlanId = newPlan1.Id,
            FacilityId = FacilityInfo.Id,
            IsEnabled = true
        };
        var mockFacilityPlan2 = new FacilityPlan
        {
            PlanId = newPlan2.Id,
            FacilityId = FacilityInfo.Id,
            IsEnabled = true
        };
        _ = await facilityPlanService!.CreateRangeAsync([mockFacilityPlan1, mockFacilityPlan2]);

        var orderRequest = new ItemUpdateOrderRequest(
            [
                newPlan1.Id, newPlan2.Id
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
    public async Task GetPlans_ReturnOk_WithListPlanResponse()
    {
        _ = await CreatePlanAsync();

        var response = await Client.GetAsync(
            BaseUrl
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotEmpty(responseString);
    }

    [Fact]
    public async Task GetAllEnablePlans_ReturnOk_WithListPlanResponse()
    {
        _ = await CreatePlanAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/?enable=true"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotEmpty(responseString);
    }

    [Fact]
    public async Task GetPlanBasicSetting_ReturnOk_WithPlanBasicSettingResponse()
    {
        var plan = await CreatePlanAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{plan.Id}/basic-setting"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetPlanDisplay_ReturnOk_WithPlanDisplayResponse()
    {
        var plan = await CreatePlanAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{plan.Id}/display"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetPlanRoomType_ReturnOk_WithPlanRoomTypeResponse()
    {
        var plan = await CreatePlanAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{plan.Id}/room-type"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetPlanPublishAccept_ReturnOk_WithPlanPublishAcceptResponse()
    {
        var plan = await CreatePlanAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{plan.Id}/publish-accept"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetPlanSale_ReturnOk_WithPlanSaleResponse()
    {
        var plan = await CreatePlanAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{plan.Id}/sale"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetPlanOption_ReturnOk_WithPlanOptionResponse()
    {
        var plan = await CreatePlanAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{plan.Id}/option"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetPlanCancel_ReturnOk_WithPlanCancelResponse()
    {
        var plan = await CreatePlanAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{plan.Id}/cancel"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetPlanQuestion_ReturnOk_WithPlanQuestionResponse()
    {
        var plan = await CreatePlanAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{plan.Id}/question"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetPlanSpecial_ReturnOk_WithPlanSpecialResponse()
    {
        var plan = await CreatePlanAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{plan.Id}/special"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetPlanImportantNote_ReturnOk_WithPlanImportantNoteResponse()
    {
        var plan = await CreatePlanAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{plan.Id}/important-note"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetPlanPaymentMethod_ReturnOk_WithPlanPaymentMethodResponse()
    {
        var plan = await CreatePlanAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{plan.Id}/payment-method"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetPlanMeal_ReturnOk_WithPlanMealResponse()
    {
        var plan = await CreatePlanAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{plan.Id}/meal"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task EnablePlan_ReturnNoContent()
    {
        var plan = await CreatePlanAsync();

        var enableReq = new PlanEnabledRequest(true);

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{plan.Id}/enabled",
            TestUtil.ToJsonContent(enableReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePlanBasicSetting_ReturnNoContent()
    {
        var plan = await CreatePlanAsync();

        var updateReq = new PlanUpdateBasicSettingRequest(
            "Name",
            "Sumary",
            "NameForImport",
            "Description",
            []
        );

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{plan.Id}/basic-setting",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePlanDisplay_ReturnNoContent()
    {
        var plan = await CreatePlanAsync();
        var facilityCategoryRepo = Factory.GetRequiredService<IFacilityCategoryRepository>();

        var mockData = new FacilityCategory
        {
            FacilityId = FacilityInfo.Id,
            Category = new Category
            {
                CategoryType = CategoryTypes.Plan,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Name" } },
                Code = EntityUtil.CreateCode()
            }
        };
        var newCategory = await facilityCategoryRepo!.AddAsync(mockData, true);

        var updateReq = new PlanUpdateDisplayRequest(["test1", "test"], [newCategory.CategoryId], null);

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{plan.Id}/display",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePlanRoomType_ReturnNoContent()
    {
        var plan = await CreatePlanAsync();
        var roomType = await CreateRoomGroupAsync();

        var updateReq = new PlanUpdateRoomTypeRequest([roomType.Id]);

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{plan.Id}/room-type",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePlanPublishAccept_ReturnNoContent()
    {
        var plan = await CreatePlanAsync();
        var site = await CreateSiteAsync();
        var rand = new Random();

        var updateReq = new PlanUpdatePublishAcceptRequest(
            true,
            AppDate.GetId(DateTime.Now),
            AppDate.GetId(DateTime.Now.AddDays(rand.Next(1, 30))),
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
            TimeSpan.Zero,
            [site.Id]
        );

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{plan.Id}/publish-accept",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePlanSale_ReturnNoContent()
    {
        var plan = await CreatePlanAsync();

        var updateReq = new PlanUpdateSaleRequest(
            TimeSpan.FromHours(1),
            TimeSpan.FromHours(3),
            TimeSpan.FromHours(3),
            0,
            1,
            null,
            PlanDaySaleLimitTypes.RoomGroup,
            false,
            null,
            null,
            true,
            1,
            2,
            7,
            3
        );

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{plan.Id}/sale",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePlanPaymentMethod_ReturnNoContent()
    {
        var plan = await CreatePlanAsync();

        var updateReq = new PlanUpdatePaymentMethodRequest(true, true);

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{plan.Id}/payment-method",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePlanMeal_ReturnNoContent()
    {
        var plan = await CreatePlanAsync();

        var updateReq = new PlanUpdateMealRequest([]);

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{plan.Id}/meal",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePlanOption_ReturnNoContent()
    {
        var plan = await CreatePlanAsync();

        var facilityOptionItemRepo = Factory.GetRequiredService<IFacilityOptionItemRepository>();

        var optionItemMockData = new OptionItem
        {
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Name" } },
            Description = "Description",
            Code = EntityUtil.CreateCode()
        };
        var optionItemOfFacility = await facilityOptionItemRepo!.AddAsync(
            new FacilityOptionItem
            {
                FacilityId = FacilityInfo.Id,
                OptionItem = optionItemMockData
            },
            true
        );

        var updateReq = new PlanUpdateOptionRequest(
            true,
            [optionItemOfFacility.OptionItemId]
        );

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{plan.Id}/option",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePlanCancel_ReturnNoContent()
    {
        var plan = await CreatePlanAsync();
        var cancellationRepo = Factory.GetRequiredService<ICancellationRepository>();
        var mockData = new Cancellation
        {
            Code = EntityUtil.CreateCode(),
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Name" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Description" } }
        };
        var newCancellation = await cancellationRepo!.AddAsync(mockData, true);

        var updateReq = new PlanUpdateCancelRequest(
            true,
            1,
            TimeSpan.Zero,
            newCancellation.Id
        );

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{plan.Id}/cancel",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePlanQuestion_ReturnNoContent()
    {
        var plan = await CreatePlanAsync();

        var facilityQuestionRepo = Factory.GetRequiredService<IFacilityQuestionRepository>();

        var questionMockData = new Question
        {
            QuestionType = QuestionTypes.Text,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Name" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Description" } },
            Code = EntityUtil.CreateCode()
        };
        var questionOfFacility = await facilityQuestionRepo!.AddAsync(
            new FacilityQuestion
            {
                FacilityId = FacilityInfo.Id,
                Question = questionMockData
            },
            true
        );

        var updateReq = new PlanUpdateQuestionRequest(
            [questionOfFacility.QuestionId]
        );

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{plan.Id}/question",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePlanSpecial_ReturnNoContent()
    {
        var plan = await CreatePlanAsync();

        var updateReq = new PlanUpdateSpecialRequest(true, "Secret");

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{plan.Id}/special",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePlanImportantNote_ReturnNoContent()
    {
        var plan = await CreatePlanAsync();

        var updateReq = new PlanUpdateImportantNoteRequest
        {
            Meal = "Meal note",
            Other = "Other note",
            Payment = "Payment note"
        };

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{plan.Id}/important-note",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeletePlan_ReturnNoContent()
    {
        var plan = await CreatePlanAsync();

        var response = await Client.DeleteAsync(
            $"{BaseUrl}/{plan.Id}"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetAllDestinationsOfPlan_ReturnOK_WithListSiteResponse()
    {
        var plan = await CreatePlanAsync();
        await CreateSiteAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{plan.Id}/destinations"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(responseString);
    }
}
