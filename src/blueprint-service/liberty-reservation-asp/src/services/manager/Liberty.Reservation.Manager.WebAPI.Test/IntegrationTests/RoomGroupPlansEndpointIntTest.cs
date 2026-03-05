using System.Net;
using Liberty.ApplicationShared.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Entity.Utils;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class RoomGroupPlansEndpointIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/room-groups";

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
            IsEnabled = true,
            PlanType = PlanTypes.RoomOnly
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
        var plan = await CreatePlanAsync();

        var roomGroupRepo = Factory.GetRequiredService<IRoomGroupRepository>();
        var existingRoomGroup = await roomGroupRepo!.GetAllAsync();
        if (existingRoomGroup.Any())
        {
            return existingRoomGroup[0];
        }

        var mockData = new RoomGroup
        {
            Code = EntityUtil.CreateCode(),
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Name" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Description" } },
            IsEnabled = true
        };

        var newRoomGroup = await roomGroupRepo.AddAsync(mockData, true);

        var planRoomGroup = Factory.GetRequiredService<IPlanRoomGroupRepository>();

        var mockDataPlanRoomGroup = new PlanRoomGroup
        {
            PlanId = plan.Id,
            RoomGroupId = newRoomGroup.Id
        };

        _ = await planRoomGroup!.AddAsync(mockDataPlanRoomGroup, true);

        return newRoomGroup;
    }

    [Fact]
    public async Task GetRoomGroupSale_ReturnOk_WithPlanSaleResponse()
    {
        var roomGroup = await CreateRoomGroupAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{roomGroup.Id}/sale"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetRoomGroupOption_ReturnOk_WithPlanOptionResponse()
    {
        var roomGroup = await CreateRoomGroupAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{roomGroup.Id}/option"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetRoomGroupCancel_ReturnOk_WithPlanCancelResponse()
    {
        var roomGroup = await CreateRoomGroupAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{roomGroup.Id}/cancel"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetRoomGroupQuestion_ReturnOk_WithPlanQuestionResponse()
    {
        var roomGroup = await CreateRoomGroupAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{roomGroup.Id}/question"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetRoomGroupSpecial_ReturnOk_WithPlanSpecialResponse()
    {
        var roomGroup = await CreateRoomGroupAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{roomGroup.Id}/special"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetRoomGroupImportantNote_ReturnOk_WithPlanImportantNoteResponse()
    {
        var roomGroup = await CreateRoomGroupAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{roomGroup.Id}/important-note"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetRoomGroupPaymentMethod_ReturnOk_WithPlanPaymentMethodResponse()
    {
        var roomGroup = await CreateRoomGroupAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{roomGroup.Id}/payment-method"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetRoomGroupMeal_ReturnOk_WithPlanMealResponse()
    {
        var roomGroup = await CreateRoomGroupAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{roomGroup.Id}/meal"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task UpdateRoomGroupSale_ReturnNoContent()
    {
        var roomGroup = await CreateRoomGroupAsync();

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
            $"{BaseUrl}/{roomGroup.Id}/sale",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateRoomGroupPaymentMethod_ReturnNoContent()
    {
        var roomGroup = await CreateRoomGroupAsync();

        var updateReq = new PlanUpdatePaymentMethodRequest(true, true);

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{roomGroup.Id}/payment-method",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateRoomGroupMeal_ReturnNoContent()
    {
        var roomGroup = await CreateRoomGroupAsync();

        var updateReq = new PlanUpdateMealRequest([]);

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{roomGroup.Id}/meal",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateRoomGroupOption_ReturnNoContent()
    {
        var roomGroup = await CreateRoomGroupAsync();

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
            $"{BaseUrl}/{roomGroup.Id}/option",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateRoomGroupCancel_ReturnNoContent()
    {
        var roomGroup = await CreateRoomGroupAsync();
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
            $"{BaseUrl}/{roomGroup.Id}/cancel",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateRoomGroupQuestion_ReturnNoContent()
    {
        var roomGroup = await CreateRoomGroupAsync();

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
            $"{BaseUrl}/{roomGroup.Id}/question",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateRoomGroupSpecial_ReturnNoContent()
    {
        var roomGroup = await CreateRoomGroupAsync();

        var updateReq = new PlanUpdateSpecialRequest(true, "Secret");

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{roomGroup.Id}/special",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateRoomGroupImportantNote_ReturnNoContent()
    {
        var roomGroup = await CreateRoomGroupAsync();

        var updateReq = new PlanUpdateImportantNoteRequest
        {
            Meal = "Meal note",
            Other = "Other note",
            Payment = "Payment note"
        };

        var response = await Client.PatchAsync(
            $"{BaseUrl}/{roomGroup.Id}/important-note",
            TestUtil.ToJsonContent(updateReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
