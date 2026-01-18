using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class PlanCategoriesEndpointIntTest(
    string categoryUrl = "/api/plan-categories"
) : BaseCategoriesEndpointIntTest(categoryUrl);
