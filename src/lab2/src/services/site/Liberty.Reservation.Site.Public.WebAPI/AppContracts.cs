namespace Liberty.Reservation.Site.Public.WebAPI;

public static class AppContracts
{
    public const string FacilityCodeHeaderKey = "X-Facility-Code";
    public const string FacilityHeaderRecordCode = "X-Facility-Record-Code";
    public const string SiteCodeOfFacilityHeaderKey = "X-Site-Code";

    public const string TimeZoneHeaderKey = "Time-Zone";
    public const string TimeZoneOffsetHeaderKey = "Time-Zone-Offset";

    public const string FacilityIdHeaderKey = "X-Facility-Id";
    public const string SiteIdOfFacilityHeaderKey = "X-Site-Id";
}
