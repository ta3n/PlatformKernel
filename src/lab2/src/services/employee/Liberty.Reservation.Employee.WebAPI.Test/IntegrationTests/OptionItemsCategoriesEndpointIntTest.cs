using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Employee.WebAPI.Test.IntegrationTests;

public class OptionItemCategoriesEndpointIntTest(
    string categoryUrl = "/api/option-item-categories"
) : BaseCategoriesEndpointIntTest(categoryUrl);
