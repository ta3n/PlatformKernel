using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Employee.WebAPI.Test.IntegrationTests;

public class FacilityFeatureCategoriesEndpointIntTest(
    string categoryUrl = "/api/facility-feature-categories"
) : BaseCategoriesEndpointIntTest(categoryUrl);
