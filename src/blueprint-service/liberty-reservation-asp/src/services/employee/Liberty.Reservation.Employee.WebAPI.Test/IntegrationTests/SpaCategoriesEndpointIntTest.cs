using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Employee.WebAPI.Test.IntegrationTests;

public class SpaCategoriesEndpointIntTest(
    string categoryUrl = "/api/spa-categories"
) : BaseCategoriesEndpointIntTest(categoryUrl);
