using Liberty.Reservation.User.File.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.User.File.WebAPI.Test;

public class CheckConfigurationValidityTest : BaseIntegrationTest
{
    [Fact]
    public void CheckConfigurationValidity()
    {
        Assert.NotNull(Factory);
        Assert.NotNull(Client);
    }
}
