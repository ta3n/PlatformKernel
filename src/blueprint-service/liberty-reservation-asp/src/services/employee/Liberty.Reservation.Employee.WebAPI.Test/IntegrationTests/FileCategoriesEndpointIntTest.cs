using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Employee.WebAPI.Test.IntegrationTests;

public class FileCategoriesEndpointIntTest(
    string categoryUrl = "/api/file-categories"
) : BaseCategoriesEndpointIntTest(categoryUrl);
