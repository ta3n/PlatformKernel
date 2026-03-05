using System.Net;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Employee.WebAPI.Test.IntegrationTests;

public class MailTypeEndpointIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/mail-types";

    [Fact]
    public async Task GetAllMailType_ReturnOK()
    {
        var response = await Client.GetAsync(BaseUrl);

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(responseString);
    }
}
