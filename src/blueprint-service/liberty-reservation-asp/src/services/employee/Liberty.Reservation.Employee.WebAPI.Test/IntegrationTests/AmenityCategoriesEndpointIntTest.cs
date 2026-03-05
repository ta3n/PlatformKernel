using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Employee.WebAPI.Test.IntegrationTests;

public class AmenityCategoriesEndpointIntTest(
    string categoryUrl = "/api/amenity-categories"
) : BaseCategoriesEndpointIntTest(categoryUrl);
