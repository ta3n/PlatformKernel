namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record OptionItemWithAppDatesResponse(
    long OptionItemId,
    string OptionItemName,
    int? BaseNumber,
    List<OptionItemAppDateDetailResponse> AppDates
);

public record OptionItemAppDateDetailResponse(
    long AppDateId,
    int? SellNumber,
    int? ReservedNumber,
    int? RemainNumber,
    bool IsNotSold
);
