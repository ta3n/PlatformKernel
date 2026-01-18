using System.Net;
using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class PriceCalendarEndpointIntTest : BaseIntegrationTest
{
    private static string BaseUrl => "api/price-calendar";

    [Fact]
    public async Task CreatePriceCalendar_ReturnNoContent()
    {
        var appDateTypeRepository = Factory.GetRequiredService<IFacilityAppDateTypeRepository>();
        var calendarRepository = Factory.GetRequiredService<IFacilityCalendarRepository>();

        var appDateType = new FacilityAppDateType
        {
            FacilityId = FacilityInfo.Id,
            AppDateType = new AppDateType
            {
                Name = "test",
                IsEnabled = true
            }
        };

        var calendar = new FacilityCalendar
        {
            FacilityId = FacilityInfo.Id,
            Calendar = new Calendar { Code = EntityUtil.CreateCode() },
            IsEnabled = true
        };

        var newAppDate = await appDateTypeRepository!.AddAsync(appDateType, true);
        _ = await calendarRepository!.AddAsync(calendar, true);
        var calendars = new List<FacilityCalendarCreateRequest> { new(newAppDate.AppDateType!.Id, 20241010, false) };

        var priceCalendarCreateRequest = new PriceCalendarCreateRequest(calendars);

        var response = await Client.PostAsync(
            BaseUrl,
            TestUtil.ToJsonContent(priceCalendarCreateRequest)
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetPriceCalendar_ReturnOk_WithResponse()
    {
        await CreatePriceCalendar_ReturnNoContent();
        var response = await Client.GetAsync(
            $"{BaseUrl}?startDate=20241010&endDate=20241212"
        );

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }
}
