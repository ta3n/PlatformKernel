using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Employee.WebAPI.Test.IntegrationTests;

public class MealTypeCategoriesEndpointIntTest(
    string categoryUrl = "/api/meal-type-categories"
) : BaseCategoriesEndpointIntTest(categoryUrl);
