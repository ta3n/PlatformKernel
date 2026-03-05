using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Employee.WebAPI.Test.IntegrationTests;

public class RoomGroupFeatureCategoriesEndpointIntTest(
    string categoryUrl = "/api/room-group-feature-categories"
) : BaseCategoriesEndpointIntTest(categoryUrl);
