using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Employee.WebAPI.Test.IntegrationTests;

public class FacilityEquipmentCategoriesEndpointIntTest(
    string categoryUrl = "/api/facility-equipment-categories"
) : BaseCategoriesEndpointIntTest(categoryUrl);
