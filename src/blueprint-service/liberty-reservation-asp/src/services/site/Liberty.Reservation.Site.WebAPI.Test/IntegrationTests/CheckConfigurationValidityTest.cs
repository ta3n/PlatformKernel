using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Site.WebAPI.Test.IntegrationTests;

public class CheckConfigurationValidityTest : BaseIntegrationTest
{
    [Fact]
    public void CheckConfigurationValidity()
    {
        Assert.NotNull(Factory);
        Assert.NotNull(Client);
    }
}
