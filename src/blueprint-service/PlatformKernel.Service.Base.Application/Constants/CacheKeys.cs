namespace PlatformKernel.Service.Base.Application.Constants;

/// <summary>
/// Provides a collection of constant keys used for caching various items in the application.
/// </summary>
public static class CacheKeys
{
    /// <summary>
    /// Represents a cache key for searching reservations across all facilities.
    /// This key is intended for global reservation search scenarios.
    /// </summary>
    /// <remarks>
    /// The value is minimized for Redis optimization.
    /// </remarks>
    /// <example>
    /// <code>
    /// var key = CacheKeys.AllFacBkgSrchKey;
    /// </code>
    /// </example>
    public const string AllFacilityBookingSearchPrefixKey = "AllFacBkgSearch";

    /// <summary>
    /// Represents a cache key template for managing detailed booking information
    /// across all facilities.
    /// </summary>
    /// <remarks>
    /// This key includes a placeholder "{0}" that can be substituted with a specific
    /// identifier to dynamically generate unique cache keys for booking details.
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// var key = string.Format(CacheKeys.AllFacilityBookingDetailPerfixKey, facilityId);
    /// </code>
    /// </example>
    public const string AllFacilityBookingDetailPrefixKey = "AllFacBkgDetail{0}";

    /// <summary>
    /// Represents a cache key template specific to a facility entity.
    /// The placeholder "{0}" in this key allows for interpolation with a facility-specific identifier,
    /// enabling the creation of unique cache keys for individual facilities.
    /// </summary>
    public const string FacilityPrefixKey = "Facility{0}";

    /// <summary>
    /// Represents a cache key prefix used to store or retrieve user-specific location data.
    /// The placeholder "{0}" in the key string is replaced with the user's unique identifier (user code).
    /// This key is typically used by caching mechanisms to manage and access user location information.
    /// </summary>
    public const string UserLocationPrefixKey = "Loc:{0}";

    public const string BulkRoomGroupAppDate = "FacilityBulk{0}:RoomGroup{1}:User{2}:RoomAdjust{3}";

    /// <summary>
    /// A constant key used as a prefix for caching booking search data.
    /// The key is formatted to include the facility ID and site ID, enabling
    /// segregation of cached data specific to each facility and site combination.
    /// </summary>
    public const string BookingSearchPrefixKey = "Facility{0}:Site{1}:BkgSearch";

    /// <summary>
    /// Represents a cache key template for storing or retrieving standard price data
    /// in booking searches. The key is formatted with placeholders for facility ID,
    /// site ID, plan ID, and room group ID.
    /// </summary>
    /// <remarks>
    /// This key is used to cache standard price data specific to a facility, site,
    /// plan, and room group combination.
    /// </remarks>
    /// <example>
    /// <code>
    /// var key = string.Format(CacheKeys.BookingSearchStandardPricePrefixKey, facilityId, siteId, planId, roomGroupId);
    /// </code>
    /// </example>
    public const string BookingSearchStandardPricePrefixKey = "Facility{0}:Site{1}:BkgSearch:StandardPrice:P{2}:R{3}";

    /// <summary>
    /// Represents a cache key to search booking details based on a specified range of person count.
    /// This key is designed to optimize searches that involve filtering bookings by the number of persons within a specified range.
    /// </summary>
    /// <remarks>
    /// The key structure incorporates parameters for the facility, site, and specific range filters.
    /// It is optimized for Redis caching to ensure optimal retrieval performance for person-range-based booking searches.
    /// </remarks>
    /// <example>
    /// <code>
    /// var key = string.Format(CacheKeys.BookingSearchRangePersonPrefixKey, facilityId, siteId, planId, roomGroupId);
    /// </code>
    /// </example>
    public const string BookingSearchRangePersonPrefixKey = "Facility{0}:Site{1}:BkgSearch:RangePerson:P{2}:R{3}";

    /// <summary>
    /// Represents a cache key template for storing or retrieving price data
    /// in booking searches. The key is formatted with placeholders for facility ID,
    /// site ID, plan ID, room group ID, and date.
    /// </summary>
    /// <remarks>
    /// This key is used to cache price data specific to a facility, site, plan,
    /// room group, and date combination.
    /// </remarks>
    /// <example>
    /// <code>
    /// var key = string.Format(CacheKeys.BookingSearchPriceDataPrefixKey, facilityId, siteId, planId, roomGroupId, date);
    /// </code>
    /// </example>
    public const string BookingSearchPriceDataPrefixKey = "Facility{0}:Site{1}:BkgSearch:PriceData:P{2}:R{3}:D{4}";

    /// <summary>
    /// Represents a cache key template for storing or retrieving discount data
    /// in booking searches. The key is formatted with placeholders for facility ID,
    /// site ID, plan ID, and room group ID.
    /// </summary>
    /// <remarks>
    /// This key is used to cache discount data specific to a facility, site, plan,
    /// and room group combination.
    /// </remarks>
    /// <example>
    /// <code>
    /// var key = string.Format(CacheKeys.BookingSearchDiscountDataPrefixKey, facilityId, siteId, planId, roomGroupId);
    /// </code>
    /// </example>
    public const string BookingSearchDiscountDataPrefixKey = "Facility{0}:Site{1}:BkgSearch:DiscountData:P{2}:R{3}";

    /// <summary>
    /// Represents a cache key template for storing or retrieving reservation data
    /// in booking searches. The key is formatted with placeholders for facility ID,
    /// site ID, and plan ID.
    /// </summary>
    /// <remarks>
    /// This key is used to cache reservation data specific to a facility, site,
    /// and plan combination.
    /// </remarks>
    /// <example>
    /// <code>
    /// var key = string.Format(CacheKeys.BookingSearchDataReservationPrefixKey, facilityId, siteId, planId);
    /// </code>
    /// </example>
    public const string BookingSearchDataReservationPrefixKey = "Facility{0}:Site{1}:BkgSearch:ResData:P{2}:D{3}";

    /// <summary>
    /// Represents a cache key template for storing or retrieving person type data
    /// in booking searches. The key is formatted with placeholders for facility ID,
    /// site ID, plan ID, and room group ID.
    /// </summary>
    /// <remarks>
    /// This key is used to cache person type data specific to a facility, site,
    /// plan, and room group combination.
    /// </remarks>
    /// <example>
    /// <code>
    /// var key = string.Format(CacheKeys.BookingSearchPersonTypePrefixKey, facilityId, siteId, planId, roomGroupId);
    /// </code>
    /// </example>
    public const string BookingSearchPersonTypePrefixKey = "Facility{0}:Site{1}:BkgSearch:PersonType:P{2}:R{3}";

    /// <summary>
    /// Represents a cache key template for storing or retrieving option item data
    /// in booking searches. The key is formatted with placeholders for facility ID,
    /// site ID, plan ID, and date.
    /// </summary>
    /// <remarks>
    /// This key is used to cache option item data specific to a facility, site,
    /// plan, and date combination.
    /// </remarks>
    /// <example>
    /// <code>
    /// var key = string.Format(CacheKeys.BookingSearchDataOptionItemPrefixKey, facilityId, siteId, planId, date);
    /// </code>
    /// </example>
    public const string BookingSearchDataOptionItemPrefixKey = "Facility{0}:Site{1}:BkgSearch:Option:P{2}:D{3}";

    /// <summary>
    /// Represents a cache key template for storing or retrieving room application date data
    /// in booking searches. The key is formatted with placeholders for facility ID,
    /// site ID, room group ID, and date.
    /// </summary>
    /// <remarks>
    /// This key is used to cache room application date data specific to a facility, site,
    /// room group, and date combination.
    /// </remarks>
    /// <example>
    /// <code>
    /// var key = string.Format(CacheKeys.BookingSearchRoomAppDatePrefixKey, facilityId, siteId, roomGroupId, date);
    /// </code>
    /// </example>
    public const string BookingSearchRoomAppDatePrefixKey = "Facility{0}:Site{1}:BkgSearch:RoomDate:R{2}:D{3}";

    /// <summary>
    /// Represents a cache key template for storing or retrieving plan application date data
    /// in booking searches. The key is formatted with placeholders for facility ID,
    /// site ID, plan ID, room group ID, and date.
    /// </summary>
    /// <remarks>
    /// This key is used to cache plan application date data specific to a facility, site,
    /// plan, room group, and date combination.
    /// </remarks>
    /// <example>
    /// <code>
    /// var key = string.Format(CacheKeys.BookingSearchPlanAppDatePrefixKey, facilityId, siteId, planId, roomGroupId, date);
    /// </code>
    /// </example>
    public const string BookingSearchPlanAppDatePrefixKey = "Facility{0}:Site{1}:BkgSearch:PlanDate:P{2}:R{3}:D{4}";

    /// <summary>
    /// Represents the cache key pattern used for retrieving booking details.
    /// The key is formatted using specific parameters to uniquely identify
    /// booking details within a facility, site, plan, and room context.
    /// </summary>
    /// <remarks>
    /// The format of this key is: "Facility{0}:Bkg:Site{1}:BkgDetails:Plan{2}:Room{3}",
    /// where:
    /// - {0} is the facility ID.
    /// - {1} is the site ID.
    /// - {2} is the plan ID.
    /// - {3} is the room ID.
    /// This key is used in caching various booking details, ensuring uniqueness and
    /// efficient cache management across different handlers and use cases.
    /// </remarks>
    public const string BookingDetailPrefixKey = "Facility{0}:Site{1}:Plan{2}:Room{3}:BkgDetail";

    /// <summary>
    /// Represents a cache key template used for managing booking holds within the system.
    /// The key format includes placeholders for facility, site, plan, room, and a date identifier.
    /// These placeholders allow for creating unique cache keys specific to booking holds
    /// associated with particular facilities, sites, plans, rooms, and dates.
    /// </summary>
    public const string BookingHoldPrefixKey = "FacHold{0}:Site{1}:Plan{2}:Room{3}:D{4}:BkgHold";

    /// <summary>
    /// Represents a cache key template used for uniquely identifying booking hold entries
    /// associated with a specific user. This key includes placeholders for Facility ID,
    /// Site ID, Plan ID, Room ID, Date, User Code, and a temporary booking code.
    /// These placeholders allow for dynamic interpolation, enabling the generation of
    /// unique cache keys for managing user-specific booking holds.
    /// </summary>
    public const string BookingHoldWithUserPrefixKey = "FacHold{0}:Site{1}:Plan{2}:Room{3}:D{4}:BkgHold:Usr{5}:{6}";

    /// <summary>
    /// A constant string pattern used as a key for resetting cached data related to facilities.
    /// This key is typically used with cache invalidation mechanisms when facility-related data
    /// is modified or updated in the system.
    /// </summary>
    public const string ResetPatternManagerFacility = "*Facility*";

    /// <summary>
    /// Represents a cache key pattern used to reset cache entries related to facility management
    /// in the application. This key pattern allows targeting of all cache entries associated with
    /// a specific facility by substituting the facility ID into the formatting pattern.
    /// </summary>
    /// <remarks>
    /// This constant is commonly used in scenarios where cache invalidation is required
    /// for all resources linked to a specific facility. It is employed across various
    /// command handlers to ensure consistency in cache reset operations.
    /// </remarks>
    public const string ResetPatternManagerFacilitySetting = "*:Facility{0}:*";

    /// <summary>
    /// Represents a cache pattern key used to reset booking details across facilities and sites.
    /// This key is commonly utilized for clearing or invalidating cache entries related to
    /// booking details, particularly when updates or changes occur within entities like
    /// room groups, categories, or facility-related configurations.
    /// </summary>
    public const string ResetPatternBookingDetails = "*BkgDetails*";

    /// <summary>
    /// Represents the cache key pattern used for resetting site booking search entries
    /// in the caching layer. This constant allows dynamic substitution of the facility identifier
    /// in the key format "*:Facility{0}:Bkg:*".
    /// </summary>
    /// <remarks>
    /// This key is commonly utilized in operations that require invalidation or reset
    /// of cached site booking search data when updates or deletions are performed
    /// to avoid stale data retrieval.
    /// </remarks>
    public const string ResetPatternSiteBookingSearch = "*:Facility{0}:Bkg:*";

    /// <summary>
    /// Represents the cache key pattern used to reset cached question-related data for a specific facility.
    /// The placeholder "{0}" in the key allows for dynamic replacement with a facility identifier.
    /// </summary>
    /// <remarks>
    /// This key is utilized in operations where cached question data needs to be invalidated for a specific facility,
    /// such as updating or enabling/disabling option items related to questions.
    /// </remarks>
    public const string ResetPatternQuestionByFacilityId = "*:Question:Facility{0}:*";

    /// <summary>
    /// Represents the cache key prefix format for identifying hotel distribution entries within a facility context.
    /// This key is utilized in cache-related operations to store or retrieve data pertaining to hotels
    /// uniquely associated with a specific facility.
    /// </summary>
    /// <remarks>
    /// The prefix follows a specific formatting pattern where `{0}` is replaced dynamically with the facility identifier.
    /// It serves as a part of patterns in cache-targeted operations, ensuring precise identification or invalidation of
    /// relevant cached entries.
    /// </remarks>
    public const string DistributionHotelsPrefixKey = "FacDis{0}";

    /// <summary>
    /// Represents the cache key prefix that is used for identifying cached data related to the "Get All Booking" query
    /// in the Kakusan context.
    /// </summary>
    /// <remarks>
    /// This cache key prefix is formatted with placeholders for the user-specific application key and additional query-specific elements,
    /// enabling efficient storage and retrieval of cached booking data.
    /// </remarks>
    /// <example>
    /// Placeholder {0} corresponds to the compact application user key, and {1} corresponds to other query-specific identifiers,
    /// making it customizable for caching multiple users and query combinations in the Kakusan domain.
    /// </example>
    public const string KakusanGetAllBookingQueryPrefixKey = "User:{0}:KakusanBookings:{1}";

    /// <summary>
    /// Represents the cache key prefix for retrieving all room queries
    /// associated with a specific user and hotel context. This key is used
    /// to cache or identify queries related to fetching room data for a given
    /// user and target hotel(s).
    /// </summary>
    /// <remarks>
    /// This prefix key should be formatted with specific parameters:
    /// - {0}: The compact user key representing the user's identity.
    /// - {1}: Additional context or identifiers for target hotel(s), typically a combination
    /// of hotel IDs or related metadata.
    /// This key enables efficient cache management, ensuring that data related
    /// to room queries is grouped and accessible for specific users and their
    /// associated hotel context. It is utilized in various application components
    /// to manage caching appropriately for user and hotel-level operations.
    /// </remarks>
    public const string KakusanGetAllRoomQueryPrefixKey = "User:{0}:KakusanRooms:{1}";

    /// <summary>
    /// Represents the cache key prefix used for retrieving all room types for a user within a specific context.
    /// This key is dynamically formatted with placeholders for the user identifier and additional parameters
    /// to distinguish the cached values based on user-specific or contextual variations.
    /// It is used in scenarios where room type data needs to be cached and managed efficiently,
    /// such as retrieval during query handling or invalidation during updates.
    /// </summary>
    public const string KakusanGetAllRoomTypeQueryPrefixKey = "User:{0}:KakusanRoomTypes:{1}";

    /// <summary>
    /// Represents a cache key specific to RSS facility information.
    /// This key is utilized for storing or retrieving data related to a particular RSS facility.
    /// </summary>
    /// <remarks>
    /// The facility identifier should replace the placeholder {0} in the key.
    /// Used primarily to improve the efficiency of accessing RSS facility data within caching operations.
    /// </remarks>
    /// <example>
    /// <code>
    /// var key = string.Format(CacheKeys.RssFacilityPrefixKey, "Facility123");
    /// </code>
    /// </example>
    public const string RssFacilityPrefixKey = "Rss:Facility:{0}";

    /// <summary>
    /// Represents the cache key pattern used for RSS feed-related facility cache entries.
    /// This pattern matches any cached entries related to RSS facilities and is utilized
    /// for efficiently clearing or invalidating associated cache entries.
    /// </summary>
    /// <remarks>
    /// This cache key pattern is primarily employed in scenarios where RSS-related facility
    /// data needs to be cleared or invalidated, such as during save operations in the
    /// <c>CacheSaveChangesInterceptor</c>.
    /// </remarks>
    /// <example>
    /// <code>
    /// _cacheService.RemoveByPatterns(true, CacheKeys.RssFacilityKeyPatternKey);
    /// </code>
    /// </example>
    public const string RssFacilityKeyPatternKey = "*Rss:Facility:";

    /// <summary>
    /// Represents a cache key for storing or retrieving data related to facility site information for RSS feed operations.
    /// This key is designed to include placeholders for dynamically inserting specific identifiers, such as facility IDs,
    /// to uniquely identify cached data for facility sites.
    /// </summary>
    /// <remarks>
    /// Designed to enhance caching performance and reduce redundant data retrieval in scenarios involving facility sites.
    /// The dynamic format of the key ensures flexibility for varying facility site-related operations while maintaining
    /// consistent cache structure for easier management.
    /// </remarks>
    /// <example>
    /// <code>
    /// string key = string.Format(CacheKeys.RssFacilitySitesPrefixKey, "123-456");
    /// </code>
    /// </example>
    public const string RssFacilitySitesPrefixKey = "Rss:Facility:Sites:{0}";

    /// <summary>
    /// Represents a cache key for storing or retrieving person age type data related to facility reservations.
    /// Commonly used in scenarios involving age segmentation or categorization within the reservation workflow.
    /// </summary>
    /// <remarks>
    /// The key is structured with a format placeholder, allowing it to be uniquely identified for specific facilities or contexts.
    /// This dynamic structure enhances compatibility with scenarios requiring granular data segmentation.
    /// </remarks>
    /// <example>
    /// Example usage demonstrates the construction of a uniquely formatted cache key:
    /// <code>
    /// var cacheKey = string.Format(CacheKeys.RssPersonAgeTypesPrefixKey, "facilityId-segment");
    /// </code>
    /// </example>
    public const string RssPersonAgeTypesPrefixKey = "Rss:PersonAgeTypes:{0}";

    /// <summary>
    /// Represents a cache pattern key for identifying and managing cached data related
    /// to person age types in the RSS (Reservation System Schema).
    /// This allows for efficient invalidation of related cache entries.
    /// </summary>
    /// <remarks>
    /// Primarily used for operations where pattern-based cache clearing is required.
    /// The wildcard (*) indicates a pattern match for broader cache management scenarios.
    /// </remarks>
    public const string RssPersonAgeTypesPatternKey = "*Rss:PersonAgeTypes*";

    /// <summary>
    /// Represents a cache key for retrieving external plan details associated with a specific user.
    /// This key is used to identify plans fetched externally for a given user context.
    /// </summary>
    /// <remarks>
    /// The key incorporates user-specific and external plan details, providing a unique identifier for these operations.
    /// </remarks>
    public const string ExternalGetPlanPrefixKey = "User:{0}:ExternalGetPlan:{1}";

    /// <summary>
    /// Represents a cache key for retrieving an external plan associated with a specific user and plan identifier.
    /// This key is used to uniquely identify external plan price data in the caching system.
    /// </summary>
    /// <remarks>
    /// The value contains placeholders for user and plan identifiers to allow dynamic substitution.
    /// </remarks>
    public const string ExternalGetPlanPricePrefixKey = "User:{0}:ExternalGetPlanPrice:{1}";

    public const string AllAlertMessageActivePrefixKey = "AllAlertMessageActive";

    public const string PersonAgeTypeMasterGetAllPrefixKey = "PersonAgeTypeMasterGetAll";
}
