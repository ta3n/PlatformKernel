using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Employee.WebAPI.Test.IntegrationTests;

public class LeisureCategoriesEndpointIntTest(
    string categoryUrl = "/api/leisure-categories"
) : BaseCategoriesEndpointIntTest(categoryUrl);
