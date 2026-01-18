using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Employee.WebAPI.Test.IntegrationTests;

public class FacilityCategoriesEndpointIntTest(
    string categoryUrl = "/api/facility-categories"
) : BaseCategoriesEndpointIntTest(categoryUrl);
