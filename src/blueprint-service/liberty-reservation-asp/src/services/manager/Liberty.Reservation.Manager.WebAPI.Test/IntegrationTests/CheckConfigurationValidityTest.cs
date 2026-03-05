using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class CheckConfigurationValidityTest : BaseIntegrationTest
{
    [Fact]
    public void CheckConfigurationValidity()
    {
        Assert.NotNull(Factory);
        Assert.NotNull(Client);
    }
}
