using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Employee.WebAPI.Test.IntegrationTests;

public class ViewCategoriesEndpointIntTest(
    string categoryUrl = "/api/view-categories"
) : BaseCategoriesEndpointIntTest(categoryUrl);
