using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Employee.WebAPI.Test.IntegrationTests;

public class RoomGroupEquipmentCategoriesEndpointIntTest(
    string categoryUrl = "/api/room-group-equipment-categories"
) : BaseCategoriesEndpointIntTest(categoryUrl);
