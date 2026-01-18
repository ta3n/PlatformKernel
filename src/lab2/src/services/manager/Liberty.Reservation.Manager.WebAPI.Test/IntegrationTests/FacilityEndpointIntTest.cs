using System.Net;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class FacilityEndpointIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/facility";

    [Fact]
    public async Task UpdateFacilityAcceptance_ReturnNoContent()
    {
        var updateAcceptanceReq = new FacilityUpdateAcceptRequest(
            true,
            "Children Comment 1",
            true,
            "Pet Comment 1",
            true,
            "Barrier Free Comment 1"
        );

        var response = await Client.PatchAsync(
            $"{BaseUrl}/accept",
            TestUtil.ToJsonContent(updateAcceptanceReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateFacilityAccess_ReturnNoContent()
    {
        var updateAccessReq = new FacilityUpdateAccessRequest(
            100,
            100,
            "Access comment",
            true,
            "Parking comment",
            true,
            "Transfer comment",
            "Near station comment"
        );

        var response = await Client.PatchAsync(
            $"{BaseUrl}/access",
            TestUtil.ToJsonContent(updateAccessReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateFacilityBasicSettings_ReturnNoContent()
    {
        var areaRepo = Factory.GetRequiredService<IAreaRepository>();
        var newArea = await areaRepo!.AddAsync(
            new Area { Name = "Area" },
            true
        );

        var categoryRepo = Factory.GetRequiredService<ICategoryRepository>();
        var newCategory = await categoryRepo!.AddAsync(
            new Category
            {
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Category" } },
                CategoryType = CategoryTypes.Facility,
                FacilityCategories =
                [
                    new FacilityCategory { FacilityId = FacilityInfo.Id }
                ]
            },
            true
        );

        var updateBasicReq = new FacilityUpdateBasicSettingRequest(
            "Description",
            "00000000000",
            "https://yopaz.vn",
            "Name",
            "Kane",
            "1",
            "Address1",
            "Address2",
            "Address3",
            "Address4",
            "03-6735-8899",
            1,
            1,
            1,
            1,
            newArea.Id,
            newCategory.Id,
            null,
            TimeSpan.FromHours(9),
            "Asia/Tokyo"
        );

        var response = await Client.PatchAsync(
            $"{BaseUrl}/basic-setting",
            TestUtil.ToJsonContent(updateBasicReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateFacilityBath_ReturnNoContent()
    {
        var updateBathReq = new FacilityUpdateBathRequest(
            "100",
            "Spa Name",
            "Spa comment",
            "Spa description"
        );

        var response = await Client.PatchAsync(
            $"{BaseUrl}/bath",
            TestUtil.ToJsonContent(updateBathReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateFacilityClassification_ReturnNoContent()
    {
        var updateClassificationReq = new FacilityUpdateClassificationRequest(
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            []
        );

        var response = await Client.PatchAsync(
            $"{BaseUrl}/classification",
            TestUtil.ToJsonContent(updateClassificationReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateFacilityPaymentMethod_ReturnNoContent()
    {
        var updatePaymentReq = new FacilityUpdatePaymentMethodRequest(
            true,
            true,
            "Onside payment comment",
            "Online payment comment",
            "Payment comment"
        );

        var response = await Client.PatchAsync(
            $"{BaseUrl}/payment-method",
            TestUtil.ToJsonContent(updatePaymentReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateFacilityPublish_ReturnNoContent()
    {
        var updatePubReq = new FacilityUpdatePublicationInformationRequest(
            "Heading 1",
            "PR Point comment",
            "Equipment comment",
            "Room comment",
            "Amenity comment",
            "Leisure comment",
            "FAQ comment",
            "Other comment",
            "Url"
        );

        var response = await Client.PatchAsync(
            $"{BaseUrl}/publish",
            TestUtil.ToJsonContent(updatePubReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateFacilityReservationChange_ReturnNoContent()
    {
        var updateReservationChangeReq = new FacilityUpdateReservationChangeRequest(
            true,
            true
        );

        var response = await Client.PatchAsync(
            $"{BaseUrl}/reservation-change",
            TestUtil.ToJsonContent(updateReservationChangeReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateFacilityReservationSetting_ReturnNoContent()
    {
        var updateReservationSettingReq = new FacilityUpdateReservationSettingRequest(
            true
        );

        var response = await Client.PatchAsync(
            $"{BaseUrl}/reservation-setting",
            TestUtil.ToJsonContent(updateReservationSettingReq)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetFacilityAcceptance_ReturnOk_WithFacilityResponse()
    {
        await UpdateFacilityAcceptance_ReturnNoContent();

        var getResponse = await Client.GetAsync(
            $"{BaseUrl}/accept"
        );

        getResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var responseString = await getResponse.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetFacilityAccess_ReturnOk_WithFacilityResponse()
    {
        await UpdateFacilityAccess_ReturnNoContent();

        var getResponse = await Client.GetAsync(
            $"{BaseUrl}/access"
        );

        getResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var responseString = await getResponse.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetFacilityBasicSettings_ReturnOk_WithFacilityResponse()
    {
        await UpdateFacilityBasicSettings_ReturnNoContent();

        var getResponse = await Client.GetAsync(
            $"{BaseUrl}/basic-setting"
        );

        getResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var responseString = await getResponse.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetFacilityBath_ReturnOk_WithFacilityResponse()
    {
        await UpdateFacilityBath_ReturnNoContent();

        var getResponse = await Client.GetAsync(
            $"{BaseUrl}/bath"
        );

        getResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var responseString = await getResponse.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetFacilityClassification_ReturnOk_WithFacilityResponse()
    {
        await UpdateFacilityClassification_ReturnNoContent();

        var getResponse = await Client.GetAsync(
            $"{BaseUrl}/classification"
        );

        getResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var responseString = await getResponse.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetFacilityPaymentMethod_ReturnOk_WithFacilityResponse()
    {
        await UpdateFacilityPaymentMethod_ReturnNoContent();

        var getResponse = await Client.GetAsync(
            $"{BaseUrl}/payment-method"
        );

        getResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var responseString = await getResponse.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetFacilityPublish_ReturnOk_WithFacilityResponse()
    {
        await UpdateFacilityPublish_ReturnNoContent();

        var getResponse = await Client.GetAsync(
            $"{BaseUrl}/publish"
        );

        getResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var responseString = await getResponse.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetFacilityReservationChange_ReturnOk_WithFacilityResponse()
    {
        await UpdateFacilityReservationChange_ReturnNoContent();

        var getResponse = await Client.GetAsync(
            $"{BaseUrl}/reservation-change"
        );

        getResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var responseString = await getResponse.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetFacilityReservationSetting_ReturnOk_WithFacilityResponse()
    {
        await UpdateFacilityReservationSetting_ReturnNoContent();

        var getResponse = await Client.GetAsync(
            $"{BaseUrl}/reservation-setting"
        );

        getResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var responseString = await getResponse.Content.ReadAsStringAsync();

        Assert.NotNull(responseString);
    }
}
