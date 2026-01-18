using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Employee.WebAPI.Test.IntegrationTests;

public class CheckConfigurationValidityTest : BaseIntegrationTest
{
    [Fact]
    public void CheckConfigurationValidity()
    {
        Assert.NotNull(Factory);
        Assert.NotNull(Client);
    }
}
