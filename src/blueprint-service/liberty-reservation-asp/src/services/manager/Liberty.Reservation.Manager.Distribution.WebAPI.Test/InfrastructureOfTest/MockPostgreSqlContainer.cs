namespace Liberty.Reservation.Manager.Distribution.WebAPI.Test.InfrastructureOfTest;

public static class MockPostgreSqlContainer
{
    public const string Host = "localhost";
    public const int ContainerPort = 5432;
    public const string ReservationDatabase = "reservation_test";
    public const string FacilityDatabase = "facility_test";

    public static int HostPort { get; set; } = 54328;

    public static string ReservationConnectionString =>
        $"Server={Host};Port={HostPort};Database={ReservationDatabase};User id=postgres;Pwd=postgres;";
}
