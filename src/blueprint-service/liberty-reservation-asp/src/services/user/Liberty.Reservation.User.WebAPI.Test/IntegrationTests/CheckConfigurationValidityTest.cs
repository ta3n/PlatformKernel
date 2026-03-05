using Liberty.Reservation.User.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.User.WebAPI.Test.IntegrationTests;

public class CheckConfigurationValidityTest : BaseIntegrationTest
{
    [Fact]
    public void CheckConfigurationValidity()
    {
        Assert.NotNull(Factory);
        Assert.NotNull(Client);
    }
}
