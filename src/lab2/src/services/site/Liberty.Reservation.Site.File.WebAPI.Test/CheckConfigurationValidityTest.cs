using Liberty.Reservation.Site.File.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Site.File.WebAPI.Test;

public class CheckConfigurationValidityTest : BaseIntegrationTest
{
    [Fact]
    public void CheckConfigurationValidity()
    {
        Assert.NotNull(Factory);
        Assert.NotNull(Client);
    }
}
