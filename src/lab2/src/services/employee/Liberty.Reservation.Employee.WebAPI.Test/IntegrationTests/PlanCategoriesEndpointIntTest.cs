using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Employee.WebAPI.Test.IntegrationTests;

public class PlanCategoriesEndpointIntTest(
    string categoryUrl = "/api/plan-categories"
) : BaseCategoriesEndpointIntTest(categoryUrl);
