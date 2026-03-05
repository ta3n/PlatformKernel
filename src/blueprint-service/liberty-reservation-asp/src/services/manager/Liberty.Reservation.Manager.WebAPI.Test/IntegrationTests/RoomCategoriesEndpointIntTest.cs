using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Manager.WebAPI.Test.IntegrationTests;

public class RoomCategoriesEndpointIntTest(
    string categoryUrl = "/api/room-group-categories"
) : BaseCategoriesEndpointIntTest(categoryUrl);
