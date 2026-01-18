using System.Net;
using System.Text.Json;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.AggregateLogs;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Employee.WebAPI.Test.IntegrationTests;

public class BookingAggregateFaxAuditLogEndpointIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/booking-aggregate-fax-audit-log";

    private async Task CreateFaxAuditLog()
    {
        var faxAuditLogRepo = Factory.GetRequiredService<IBookingAggregateFaxAuditLogRepository>();
        var mockData = new BookingAggregateFaxAuditLog
        {
            FaxNumber = "123456789",
            Subject = "Test Fax",
            Result = "Success",
            IsSuccess = true,
            AggregateCode = "RES123"
        };

        _ = await faxAuditLogRepo!.AddAsync(mockData, true);
    }

    [Fact]
    public async Task SearchFaxAuditLog_RetrunOk()
    {
        await CreateFaxAuditLog();

        var response = await Client.GetAsync(
            $"{BaseUrl}/search"
        );

        response.EnsureSuccessStatusCode();
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        var data = JsonSerializer.Deserialize<List<BookingAggregateFaxAuditLogSearchResponse>>(
            responseString,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(data);
    }
}
