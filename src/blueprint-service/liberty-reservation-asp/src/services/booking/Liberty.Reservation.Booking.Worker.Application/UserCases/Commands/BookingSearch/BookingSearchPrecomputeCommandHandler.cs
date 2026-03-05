using System.Text;
using System.Text.Json;
using Liberty.Reservation.Booking.Worker.Application.Settings;
using Liberty.Reservation.Manager.Application.Models;
using Microsoft.Extensions.Options;

namespace Liberty.Reservation.Booking.Worker.Application.UserCases.Commands.BookingSearch;

public class BookingSearchPrecomputeCommandHandler(
    ILogger<BookingSearchPrecomputeCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IHttpClientFactory httpClientFactory,
    IOptions<ServiceSetting> serviceSettingOptions,
    IFacilityService facilityService
) : ActionCommandHandlerBase<BookingSearchPrecomputeCommand, bool>(unitOfWork, mapper)
{
    private const int DefaultPageSize = 20;

    private const string PrecomputeHeader = "X-Precompute";
    private const string AcceptLanguage = "Accept-Language";
    private const string TimeZoneOffsetHeader = "Time-Zone-Offset";
    private const string FacilityCodeHeader = "X-Facility-Code";
    private const string SiteCodeHeader = "X-Site-Code";

    private const int DelayBetweenRequestsMs = 5000;

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private string SiteReservationApiUrl => serviceSettingOptions.Value.ReservationSiteService?.Url
        ?? throw new InvalidOperationException("Reservation Site Service URL is not configured.");

    private string BookingSearchEndpoint => $"{SiteReservationApiUrl}/api/booking/search?api-version=1&page=1&size=2000&enabled=true";

    protected override async Task<bool> HandleAsync(
        BookingSearchPrecomputeCommand request,
        CancellationToken cancellationToken
    )
    {
        var currentPageIndex = 1;

        while (true)
        {
            var facilities = await facilityService.FindAllAvailableFacilitiesOfPrecomputeAsync(
                Pageable.Of(currentPageIndex, DefaultPageSize, true),
                cancellationToken
            );

            await ExecuteFacilityBookingSearchAsync(
                facilities.Content,
                cancellationToken
            );

            if (facilities.HasNext)
            {
                currentPageIndex++;
            }
            else
            {
                break;
            }
        }

        return true;
    }

    private async Task ExecuteFacilityBookingSearchAsync(
        IEnumerable<FacilityAvailableModel> facilities,
        CancellationToken cancellationToken
    )
    {
        var facilityAvailableModels = facilities as FacilityAvailableModel[] ?? [.. facilities];
        if (facilityAvailableModels.Length == 0)
        {
            logger.LogWarning("No facilities found for precomputation.");

            return;
        }

        var facilitySiteFlatAvailableModels = FlattenFacilities(facilityAvailableModels).ToList();

        foreach (var facilitySiteFlatAvailableModel in facilitySiteFlatAvailableModels)
        {
            var personAgeTypes = facilitySiteFlatAvailableModel.PersonAgeTypes;
            if (personAgeTypes == null)
            {
                continue;
            }

            var personAgeTypeOfFacilityAvailableModels = personAgeTypes as PersonAgeTypeOfFacilityAvailableModel[] ?? [.. personAgeTypes];
            if (personAgeTypeOfFacilityAvailableModels is not { Length: > 0 })
            {
                continue;
            }

            var facilityCode = facilitySiteFlatAvailableModel.FacilityCode?.Trim();
            var siteCode = facilitySiteFlatAvailableModel.SiteCode?.Trim();

            await SearchBookingAsync(
                facilityCode,
                siteCode,
                "ja-JP",
                9,
                personAgeTypeOfFacilityAvailableModels,
                cancellationToken
            );

            await Task.Delay(
                TimeSpan.FromMilliseconds(DelayBetweenRequestsMs),
                cancellationToken
            );

            await SearchBookingAsync(
                facilityCode,
                siteCode,
                "ja-JP",
                7,
                personAgeTypeOfFacilityAvailableModels,
                cancellationToken
            );

            await Task.Delay(
                TimeSpan.FromMilliseconds(DelayBetweenRequestsMs),
                cancellationToken
            );

            await SearchBookingAsync(
                facilityCode,
                siteCode,
                "en-US",
                9,
                facilitySiteFlatAvailableModel.PersonAgeTypes,
                cancellationToken
            );

            await Task.Delay(
                TimeSpan.FromMilliseconds(DelayBetweenRequestsMs),
                cancellationToken
            );

            await SearchBookingAsync(
                facilityCode,
                siteCode,
                "en-US",
                7,
                facilitySiteFlatAvailableModel.PersonAgeTypes,
                cancellationToken
            );

            await Task.Delay(
                TimeSpan.FromMilliseconds(DelayBetweenRequestsMs),
                cancellationToken
            );
        }
    }

    private async Task SearchBookingAsync(
        string? facilityCode,
        string? siteCode,
        string acceptLanguage,
        int timeZoneOffset,
        IEnumerable<PersonAgeTypeOfFacilityAvailableModel>? personAgeTypes,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(facilityCode) || string.IsNullOrWhiteSpace(siteCode))
        {
            return;
        }

        using var httpClient = httpClientFactory.CreateClient();
        httpClient.Timeout = TimeSpan.FromSeconds(30);

        try
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, BookingSearchEndpoint);
            httpRequest.Headers.Add(PrecomputeHeader, "true");
            httpRequest.Headers.Add(AcceptLanguage, acceptLanguage);
            httpRequest.Headers.Add(TimeZoneOffsetHeader, $"{timeZoneOffset}");
            httpRequest.Headers.Add(FacilityCodeHeader, facilityCode);
            httpRequest.Headers.Add(SiteCodeHeader, siteCode);

            var request = GetBookingSearchRequest(
                timeZoneOffset,
                personAgeTypes ?? []
            );
            var jsonContent = JsonSerializer.Serialize(
                request,
                JsonOptions
            );
            httpRequest.Content = new StringContent(
                jsonContent,
                Encoding.UTF8,
                "application/json"
            );

            var response = await httpClient.SendAsync(httpRequest, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation(
                    "Booking search completed successfully for facility: {FacilityCode}, site: {SiteCode}, language: {Language}",
                    facilityCode,
                    siteCode,
                    acceptLanguage
                );
            }
            else
            {
                logger.LogWarning(
                    "Booking search failed with status {StatusCode} for facility: {FacilityCode}, site: {SiteCode}",
                    response.StatusCode,
                    facilityCode,
                    siteCode
                );
            }
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(
                ex,
                "HTTP request failed for facility: {FacilityCode}, site: {SiteCode}",
                facilityCode,
                siteCode
            );
        }
        catch (TaskCanceledException ex)
        {
            logger.LogWarning(
                ex,
                "Request timeout for facility: {FacilityCode}, site: {SiteCode}",
                facilityCode,
                siteCode
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error during booking search for facility: {FacilityCode}, site: {SiteCode}",
                facilityCode,
                siteCode
            );
        }
    }

    private static BookingSearchRequest GetBookingSearchRequest(
        int? timeZoneOffset,
        IEnumerable<PersonAgeTypeOfFacilityAvailableModel> personAgeTypes
    )
    {
        var dateTimeNow = DateTime.UtcNow.AddHours(timeZoneOffset ?? DefaultValues.TimeZoneOffset);
        var dateNow = dateTimeNow.Date;

        var checkInDate = AppDate.GetId(dateNow);
        var checkOutDate = AppDate.GetId(
            dateNow.AddDays(
                DefaultValues.BookingSearchCheckOutDayOffset
            )
        );
        var displayCheckOutDate = AppDate.GetId(
            dateNow.AddDays(
                DefaultValues.BookingSearchDisplayCheckOutDayOffset
            )
        );

        var guestPerRooms = new List<GuestPerRoom>();
        foreach (var personAgeTypeOfFacilityAvailableModel in personAgeTypes)
        {
            var guestPerRoom = new GuestPerRoom
            {
                PersonAgeTypeId = personAgeTypeOfFacilityAvailableModel.Id,
                AppDateId = checkInDate,
                RestIndex = 0,
                RoomGroupIndex = 0,
                MalePersons = 0,
                FemalePersons = 0,
                Persons = 0
            };

            if (personAgeTypeOfFacilityAvailableModel.IsMain)
            {
                guestPerRoom.MalePersons = 1;
                guestPerRoom.FemalePersons = 1;
                guestPerRoom.Persons = 2;
            }

            guestPerRooms.Add(guestPerRoom);
        }

        var request = new BookingSearchRequest
        {
            CheckInDate = checkInDate,
            CheckOutDate = checkOutDate,
            DisplayCheckInDate = checkInDate,
            DisplayCheckOutDate = displayCheckOutDate,
            RestNumber = 1,
            RoomNumber = 1,
            GuestsPerRoom = guestPerRooms,
            OptionItems = []
        };

        return request;
    }

    private static IEnumerable<FacilitySiteFlatAvailableModel> FlattenFacilities(
        IEnumerable<FacilityAvailableModel> facilities
    )
    {
        return facilities
            .Where(x => x.Sites != null)
            .SelectMany(
                facility => facility.Sites?
                        .Select(
                            site => new FacilitySiteFlatAvailableModel(
                                facility.Id,
                                facility.Code,
                                site.Id,
                                site.Code,
                                facility.PersonAgeTypes
                            )
                        )
                    ?? []
            );
    }

    private sealed class BookingSearchRequest
    {
        public long CheckInDate { get; set; }
        public long CheckOutDate { get; set; }
        public long DisplayCheckInDate { get; set; }
        public long DisplayCheckOutDate { get; set; }
        public int RestNumber { get; set; }
        public int RoomNumber { get; set; }
        public List<GuestPerRoom> GuestsPerRoom { get; set; } = [];
        public List<object> OptionItems { get; set; } = [];
    }

    private sealed class GuestPerRoom
    {
        public long PersonAgeTypeId { get; set; }
        public long AppDateId { get; set; }
        public int RestIndex { get; set; }
        public int RoomGroupIndex { get; set; }
        public int Persons { get; set; }
        public int FemalePersons { get; set; }
        public int MalePersons { get; set; }
    }
}
