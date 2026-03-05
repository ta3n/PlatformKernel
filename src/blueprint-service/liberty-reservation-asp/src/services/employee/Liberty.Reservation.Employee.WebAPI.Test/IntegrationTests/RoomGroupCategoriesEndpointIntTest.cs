using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Employee.WebAPI.Test.IntegrationTests;

public class RoomGroupCategoriesEndpointIntTest(
    string categoryUrl = "/api/room-group-categories"
) : BaseCategoriesEndpointIntTest(categoryUrl);
