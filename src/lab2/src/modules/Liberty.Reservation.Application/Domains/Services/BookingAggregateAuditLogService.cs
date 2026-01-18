using System.Globalization;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.AggregateLogs;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Application.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Application.Domains.Services;

public class BookingAggregateAuditLogService(
    ILogger<BookingAggregateAuditLogService> logger,
    IBookingAggregateAuditLogRepository bookingAggregateAuditLogRepository,
    IBookingQuestionRepository bookingQuestionRepository,
    IBookingOptionItemRepository bookingOptionItemRepository,
    IBookingPersonAgeTypeRepository bookingPersonAgeTypeRepository
) : BaseService<BookingAggregateAuditLog>(logger, bookingAggregateAuditLogRepository, new BookingAuditLogNotfoundException()),
    IBookingAggregateAuditLogService
{
    private static readonly string[] BookingAuditLogEventTypes =
    [
        "BookingCreatedEvent",
        "BookingUpdatedEvent"
    ];

    public async Task<BookingAggregateAuditLog?> FindLastestReservationAsync(
        long? bookingId,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = bookingAggregateAuditLogRepository.GetQueryableWithAsNoTracking()
            .Where(x => x.AggregateId == bookingId)
            .Where(x => BookingAuditLogEventTypes.Contains(x.EventType))
            .OrderByDescending(x => x.Id);

        return await queryable.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<ChangeOfBookingAuditLogResponse>> DiffRequestBookingAuditLogsAsync(
        BookingAdjustRequest requestNew,
        BookingAdjustRequest requestOld,
        string? languageCode = "ja",
        CancellationToken cancellationToken = default
    )
    {
        var changeItems = new List<ChangeOfBookingAuditLogResponse>();
        changeItems.AddRange(
            ChangeBasicBooking(
                requestNew,
                requestOld,
                [
                    nameof(BookingAdjustRequest.Id),
                    nameof(BookingAdjustRequest.Reserver),
                    nameof(BookingAdjustRequest.MainUser),
                    nameof(BookingAdjustRequest.NightOptions),
                    nameof(BookingAdjustRequest.NightPeoples),
                    nameof(BookingAdjustRequest.RoomRepresentatives),
                    nameof(BookingAdjustRequest.PlanQuestions),
                    nameof(BookingAdjustRequest.OptionsQuestions)
                ]
            )
        );

        changeItems.AddRange(
            await GetNightPeopleAuditLogsAsync(
                requestNew,
                requestOld,
                languageCode!,
                cancellationToken
            )
        );

        changeItems.AddRange(
            await GetNightOptionAuditLogsAsync(
                requestNew,
                requestOld,
                languageCode!,
                cancellationToken
            )
        );

        changeItems.AddRange(
            await GetPlanQuestionAuditLogsAsync(
                requestNew,
                requestOld,
                languageCode!,
                cancellationToken
            )
        );
        changeItems.AddRange(
            await GetOptionQuestionAuditLogsAsync(
                requestNew,
                requestOld,
                languageCode!,
                cancellationToken
            )
        );
        changeItems.AddRange(
            GetReserverAuditLogs(
                requestNew,
                requestOld
            )
        );

        changeItems.AddRange(
            GetMainUserAuditLogs(
                requestNew,
                requestOld
            )
        );

        changeItems.AddRange(
            GetRoomRepresentativeAuditLogs(
                requestNew,
                requestOld
            )
        );

        return changeItems;
    }

    private static List<ChangeOfBookingAuditLogResponse> ChangeBasicBooking(
        BookingAdjustRequest requestNew,
        BookingAdjustRequest requestOld,
        string[] ignoreProperties
    )
    {
        const string changeItem = "";

        var differences = CompareUtil.GetDifferences(
            requestOld,
            requestNew,
            ignoreProperties
        );

        var auditLogs = new List<ChangeOfBookingAuditLogResponse>();

        foreach (var (propName, beforeValue, afterValue) in differences)
        {
            var name = FormatName($"{changeItem}{propName}");
            var auditLog = new ChangeOfBookingAuditLogResponse(
                name,
                $"{FormatToHHmmIfPossible(beforeValue)}",
                $"{FormatToHHmmIfPossible(afterValue)}"
            );

            auditLogs.Add(auditLog);
        }

        return auditLogs;
    }

    private static List<ChangeOfBookingAuditLogResponse> GetReserverAuditLogs(
        BookingAdjustRequest requestNew,
        BookingAdjustRequest requestOld
    )
    {
        const string changeItem = "Reserver";
        var differences = CompareUtil.GetDifferences(
            requestOld.ConvertToCustomerOfBookingData(),
            requestNew.ConvertToCustomerOfBookingData()
        );

        var auditLogs = new List<ChangeOfBookingAuditLogResponse>();

        foreach (var (propName, beforeValue, afterValue) in differences)
        {
            var name = FormatName($"{changeItem}{propName}");
            var auditLog = new ChangeOfBookingAuditLogResponse(
                name,
                $"{beforeValue}",
                $"{afterValue}"
            );

            auditLogs.Add(auditLog);
        }

        return auditLogs;
    }

    private static List<ChangeOfBookingAuditLogResponse> GetMainUserAuditLogs(
        BookingAdjustRequest requestNew,
        BookingAdjustRequest requestOld
    )
    {
        const string changeItem = "MainUser";
        var differences = CompareUtil.GetDifferences(requestOld.ConvertToGuestOfBookingData(), requestNew.ConvertToGuestOfBookingData());

        var auditLogs = new List<ChangeOfBookingAuditLogResponse>();

        foreach (var (propName, beforeValue, afterValue) in differences)
        {
            var name = FormatName($"{changeItem}{propName}");
            var auditLog = new ChangeOfBookingAuditLogResponse(
                name,
                $"{beforeValue}",
                $"{afterValue}"
            );

            auditLogs.Add(auditLog);
        }

        return auditLogs;
    }

    private async Task<List<ChangeOfBookingAuditLogResponse>> GetPlanQuestionAuditLogsAsync(
        BookingAdjustRequest requestNew,
        BookingAdjustRequest requestOld,
        string languageCode,
        CancellationToken cancellationToken = default
    )
    {
        const string changeItem = "Question";

        var newQuestionId = requestNew.PlanQuestions?.Select(x => x.QuestionId).ToList() ?? [];
        var oldQuestionId = requestOld.PlanQuestions?.Select(x => x.QuestionId).ToList() ?? [];
        var questionNew = await bookingQuestionRepository.GetQueryableWithAsNoTracking()
            .Where(x => newQuestionId.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var questionOld = await bookingQuestionRepository.GetQueryableWithAsNoTracking()
            .Where(x => oldQuestionId.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var newValue = requestNew.PlanQuestions?.Select(
                    x => new QuestionData
                    {
                        Id = x.QuestionId,
                        AnswerData = x.AnswerData,
                        Name = questionNew.Find(a => a.Id == x.QuestionId)?.Name?.GetValueByCode(languageCode),
                        Description = questionNew.Find(a => a.Id == x.QuestionId)?.Description?.GetValueByCode(languageCode),
                        FormData = questionNew.Find(a => a.Id == x.QuestionId)?.FormData?.GetValueByCode(languageCode)
                    }
                )
                .ToList()
            ?? [];
        var oldValue = requestOld.PlanQuestions?.Select(
                    x => new QuestionData
                    {
                        Id = x.QuestionId,
                        AnswerData = x.AnswerData,
                        Name = questionOld.Find(a => a.Id == x.QuestionId)?.Name?.GetValueByCode(languageCode),
                        Description = questionOld.Find(a => a.Id == x.QuestionId)?.Description?.GetValueByCode(languageCode),
                        FormData = questionOld.Find(a => a.Id == x.QuestionId)?.FormData?.GetValueByCode(languageCode)
                    }
                )
                .ToList()
            ?? [];

        var questionChanges = GenerateBookingQuestionAuditChangeLog(
            oldValue,
            newValue
        );

        if (questionChanges is not { Count: > 0 })
        {
            return [];
        }

        return
        [
            .. questionChanges.Select(
                appDateChange => new ChangeOfBookingAuditLogResponse(
                    $"{changeItem}",
                    appDateChange.OldValue?.AnswerData,
                    appDateChange.NewValue?.AnswerData
                )
                {
                    ChangeItemData = appDateChange.Type switch
                    {
                        ChangeType.Added => new
                        {
                            changeType = nameof(ChangeType.Added),
                            name = appDateChange.NewValue?.Name,
                            description = appDateChange.NewValue?.Description
                        },
                        ChangeType.Removed => new
                        {
                            changeType = nameof(ChangeType.Removed),
                            name = appDateChange.OldValue?.Name,
                            description = appDateChange.OldValue?.Description
                        },
                        _ => new
                        {
                            changeType = nameof(ChangeType.Modified),
                            name = appDateChange.NewValue?.Name,
                            description = appDateChange.NewValue?.Description
                        }
                    }
                }
            )
        ];
    }

    private async Task<List<ChangeOfBookingAuditLogResponse>> GetOptionQuestionAuditLogsAsync(
        BookingAdjustRequest requestNew,
        BookingAdjustRequest requestOld,
        string languageCode,
        CancellationToken cancellationToken = default
    )
    {
        const string changeItem = "Question";

        var newQuestionId = requestNew.OptionsQuestions?.Select(x => x.QuestionId).ToList() ?? [];
        var oldQuestionId = requestOld.OptionsQuestions?.Select(x => x.QuestionId).ToList() ?? [];
        var questionNew = await bookingQuestionRepository.GetQueryableWithAsNoTracking()
            .Where(x => newQuestionId.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var questionOld = await bookingQuestionRepository.GetQueryableWithAsNoTracking()
            .Where(x => oldQuestionId.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var newValue = requestNew.OptionsQuestions?.Select(
                    x => new QuestionData
                    {
                        Id = x.QuestionId,
                        AnswerData = x.AnswerData,
                        Name = questionNew.Find(a => a.Id == x.QuestionId)?.Name?.GetValueByCode(languageCode),
                        Description = questionNew.Find(a => a.Id == x.QuestionId)?.Description?.GetValueByCode(languageCode),
                        FormData = questionNew.Find(a => a.Id == x.QuestionId)?.FormData?.GetValueByCode(languageCode)
                    }
                )
                .ToList()
            ?? [];
        var oldValue = requestOld.OptionsQuestions?.Select(
                    x => new QuestionData
                    {
                        Id = x.QuestionId,
                        AnswerData = x.AnswerData,
                        Name = questionOld.Find(a => a.Id == x.QuestionId)?.Name?.GetValueByCode(languageCode),
                        Description = questionOld.Find(a => a.Id == x.QuestionId)?.Description?.GetValueByCode(languageCode),
                        FormData = questionOld.Find(a => a.Id == x.QuestionId)?.FormData?.GetValueByCode(languageCode)
                    }
                )
                .ToList()
            ?? [];

        var questionChanges = GenerateBookingQuestionAuditChangeLog(
            oldValue,
            newValue
        );

        if (questionChanges is not { Count: > 0 })
        {
            return [];
        }

        return
        [
            .. questionChanges.Select(
                appDateChange => new ChangeOfBookingAuditLogResponse(
                    $"{changeItem}",
                    appDateChange.OldValue?.AnswerData,
                    appDateChange.NewValue?.AnswerData
                )
                {
                    ChangeItemData = appDateChange.Type switch
                    {
                        ChangeType.Added => new
                        {
                            changeType = nameof(ChangeType.Added),
                            name = appDateChange.NewValue?.Name,
                            description = appDateChange.NewValue?.Description
                        },
                        ChangeType.Removed => new
                        {
                            changeType = nameof(ChangeType.Removed),
                            name = appDateChange.OldValue?.Name,
                            description = appDateChange.OldValue?.Description
                        },
                        _ => new
                        {
                            changeType = nameof(ChangeType.Modified),
                            name = appDateChange.NewValue?.Name,
                            description = appDateChange.NewValue?.Description
                        }
                    }
                }
            )
        ];
    }

    private static List<ChangeOfBookingAuditLogResponse> GetRoomRepresentativeAuditLogs(
        BookingAdjustRequest requestNew,
        BookingAdjustRequest requestOld
    )
    {
        const string changeItem = "RoomRepresentative";

        var newValue = requestNew.RoomRepresentatives?.ToList() ?? [];
        var oldValue = requestOld.RoomRepresentatives?.ToList() ?? [];

        var repNameChanges = CompareUtil.Diff(
                oldValue,
                newValue,
                x => x.RoomIndex,
                (
                    a,
                    b
                ) => a.FullName == b.FullName
            )
            .FindAll(
                x => (x.OldValue?.FullName ?? string.Empty) != (x.NewValue?.FullName ?? string.Empty)
            );

        var repKanaChanges = CompareUtil.Diff(
                oldValue,
                newValue,
                x => x.RoomIndex,
                (
                    a,
                    b
                ) => (a.Kana ?? string.Empty) == (b.Kana ?? string.Empty)
            )
            .FindAll(
                x => (x.OldValue?.Kana ?? string.Empty) != (x.NewValue?.Kana ?? string.Empty)
            );

        var roomIndexes = GetRoomRepresentativeIndexes(
            repNameChanges,
            repKanaChanges
        );

        if (roomIndexes is not { Count: > 0 })
        {
            return [];
        }

        var auditLogs = new List<ChangeOfBookingAuditLogResponse>();

        foreach (var roomIndex in roomIndexes)
        {
            var index = roomIndex;
            var roomRepresentativeName = repNameChanges.Find(
                x => x.NewValue?.RoomIndex == index || x.OldValue?.RoomIndex == index
            );
            var roomRepresentativeKana = repKanaChanges.Find(
                x => x.NewValue?.RoomIndex == roomIndex || x.OldValue?.RoomIndex == roomIndex
            );

            if (roomRepresentativeName is not null)
            {
                auditLogs.Add(
                    new ChangeOfBookingAuditLogResponse(
                        $"{changeItem}Name",
                        roomRepresentativeName.OldValue?.FullName,
                        roomRepresentativeName.NewValue?.FullName
                    )
                    {
                        ChangeItemData = roomRepresentativeName.Type switch
                        {
                            ChangeType.Added => new
                            {
                                changeType = nameof(ChangeType.Added),
                                roomIndex = roomRepresentativeName.NewValue?.RoomIndex + 1
                            },
                            ChangeType.Removed => new
                            {
                                changeType = nameof(ChangeType.Removed),
                                roomIndex = roomRepresentativeName.OldValue?.RoomIndex + 1
                            },
                            _ => new
                            {
                                changeType = nameof(ChangeType.Modified),
                                roomIndex = roomRepresentativeName.NewValue?.RoomIndex + 1
                            }
                        }
                    }
                );
            }

            if (roomRepresentativeKana is not null)
            {
                auditLogs.Add(
                    new ChangeOfBookingAuditLogResponse(
                        $"{changeItem}Kana",
                        roomRepresentativeKana.OldValue?.Kana,
                        roomRepresentativeKana.NewValue?.Kana
                    )
                    {
                        ChangeItemData = roomRepresentativeKana.Type switch
                        {
                            ChangeType.Added => new
                            {
                                changeType = nameof(ChangeType.Added),
                                roomIndex = roomRepresentativeKana.NewValue?.RoomIndex + 1
                            },
                            ChangeType.Removed => new
                            {
                                changeType = nameof(ChangeType.Removed),
                                roomIndex = roomRepresentativeKana.OldValue?.RoomIndex + 1
                            },
                            _ => new
                            {
                                changeType = nameof(ChangeType.Modified),
                                roomIndex = roomRepresentativeKana.NewValue?.RoomIndex + 1
                            }
                        }
                    }
                );
            }
        }

        return auditLogs;
    }

    private async Task<List<ChangeOfBookingAuditLogResponse>> GetNightPeopleAuditLogsAsync(
        BookingAdjustRequest requestNew,
        BookingAdjustRequest requestOld,
        string languageCode,
        CancellationToken cancellationToken = default
    )
    {
        const string changeItem = "PersonAgeType";

        var newValue = requestNew.NightPeoplesToTable()?.ToList() ?? [];
        var oldValue = requestOld.NightPeoplesToTable()?.ToList() ?? [];

        var personAgeTypeNewIds = newValue.Select(x => x.PersonAgeTypeId).ToList();
        var personAgeTypeOldIds = oldValue.Select(x => x.PersonAgeTypeId).ToList();

        var personAgeTypesNew = await bookingPersonAgeTypeRepository.GetQueryableWithAsNoTracking()
            .Where(x => personAgeTypeNewIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var personAgeTypesOld = await bookingPersonAgeTypeRepository.GetQueryableWithAsNoTracking()
            .Where(x => personAgeTypeOldIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var appDatePersonFlatOfBookingNew = newValue.Select(
                x => new AppDatePersonFlatOfBookingAuditLogResponse(
                    x.AppDateId,
                    x.RestIndex,
                    x.RoomIndex,
                    personAgeTypesNew.Find(a => a.Id == x.PersonAgeTypeId)?.Name?.GetValueByCode(languageCode),
                    x.Gender,
                    x.NumberOfPeoples,
                    0,
                    0,
                    0
                )
            )
            .ToList();

        var appDatePersonFlatOfBookingOld = oldValue.Select(
                x => new AppDatePersonFlatOfBookingAuditLogResponse(
                    x.AppDateId,
                    x.RestIndex,
                    x.RoomIndex,
                    personAgeTypesOld.Find(a => a.Id == x.PersonAgeTypeId)?.Name?.GetValueByCode(languageCode),
                    x.Gender,
                    x.NumberOfPeoples,
                    0,
                    0,
                    0
                )
            )
            .ToList();

        var nightPeopleChanges = GenerateNightPeopleAuditChangeLog(
            appDatePersonFlatOfBookingOld,
            appDatePersonFlatOfBookingNew
        );

        if (nightPeopleChanges is not { Count: > 0 })
        {
            return [];
        }

        return
        [
            .. nightPeopleChanges.Select(
                appDateChange => new ChangeOfBookingAuditLogResponse(
                    $"{changeItem}",
                    appDateChange.OldValue?.NumberOfPersons,
                    appDateChange.NewValue?.NumberOfPersons
                )
                {
                    ChangeItemData = appDateChange.Type switch
                    {
                        ChangeType.Added => new
                        {
                            changeType = nameof(ChangeType.Added),
                            appDateId = appDateChange.NewValue?.AppDateId,
                            restIndex = appDateChange.NewValue?.RestIndex + 1,
                            roomIndex = appDateChange.NewValue?.RoomIndex + 1,
                            personAgeTypeName = appDateChange.NewValue?.PersonAgeTypeName,
                            gender = appDateChange.NewValue?.Gender.ToString()
                        },
                        ChangeType.Removed => new
                        {
                            changeType = nameof(ChangeType.Removed),
                            appDateId = appDateChange.OldValue?.AppDateId,
                            restIndex = appDateChange.OldValue?.RestIndex + 1,
                            roomIndex = appDateChange.OldValue?.RoomIndex + 1,
                            personAgeTypeName = appDateChange.OldValue?.PersonAgeTypeName,
                            gender = appDateChange.OldValue?.Gender.ToString()
                        },
                        _ => new
                        {
                            changeType = nameof(ChangeType.Modified),
                            appDateId = appDateChange.NewValue?.AppDateId,
                            restIndex = appDateChange.NewValue?.RestIndex + 1,
                            roomIndex = appDateChange.NewValue?.RoomIndex + 1,
                            personAgeTypeName = appDateChange.NewValue?.PersonAgeTypeName,
                            gender = appDateChange.NewValue?.Gender.ToString()
                        }
                    }
                }
            )
        ];
    }

    private async Task<List<ChangeOfBookingAuditLogResponse>> GetNightOptionAuditLogsAsync(
        BookingAdjustRequest requestNew,
        BookingAdjustRequest requestOld,
        string languageCode,
        CancellationToken cancellationToken = default
    )
    {
        const string changeItem = "Option";

        var newValue = requestNew.NightOptionItemsToTable()?.ToList() ?? [];
        var oldValue = requestOld.NightOptionItemsToTable()?.ToList() ?? [];

        var newOptionId = newValue.Select(x => x.OptionItemId).ToList();
        var oldOptionId = oldValue.Select(x => x.OptionItemId).ToList();

        var optionsNew = await bookingOptionItemRepository.GetQueryableWithAsNoTracking()
            .Where(x => newOptionId.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var optionsOld = await bookingOptionItemRepository.GetQueryableWithAsNoTracking()
            .Where(x => oldOptionId.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var optionPersonFlatOfBookingNew = newValue.Select(
                x => new AppDateOptionFlatOfBookingAuditLogResponse(
                    x.AppDateId,
                    x.RestIndex,
                    x.RoomIndex,
                    optionsNew.Find(a => a.Id == x.OptionItemId)?.Name?.GetValueByCode(languageCode),
                    x.Number
                )
            )
            .ToList();

        var optionPersonFlatOfBookingOld = oldValue.Select(
                x => new AppDateOptionFlatOfBookingAuditLogResponse(
                    x.AppDateId,
                    x.RestIndex,
                    x.RoomIndex,
                    optionsOld.Find(a => a.Id == x.OptionItemId)?.Name?.GetValueByCode(languageCode),
                    x.Number
                )
            )
            .ToList();

        var nightOptionChanges = GenerateNightOptionAuditChangeLog(
            optionPersonFlatOfBookingOld,
            optionPersonFlatOfBookingNew
        );

        if (nightOptionChanges is not { Count: > 0 })
        {
            return [];
        }

        return
        [
            .. nightOptionChanges.Select(
                appDateChange => new ChangeOfBookingAuditLogResponse(
                    $"{changeItem}",
                    appDateChange.OldValue?.NumberOfOptions,
                    appDateChange.NewValue?.NumberOfOptions
                )
                {
                    ChangeItemData = appDateChange.Type switch
                    {
                        ChangeType.Added => new
                        {
                            changeType = nameof(ChangeType.Added),
                            appDateId = appDateChange.NewValue?.AppDateId,
                            restIndex = appDateChange.NewValue?.RestIndex + 1,
                            roomIndex = appDateChange.NewValue?.RoomIndex + 1,
                            optionName = appDateChange.NewValue?.OptionName
                        },
                        ChangeType.Removed => new
                        {
                            changeType = nameof(ChangeType.Removed),
                            appDateId = appDateChange.OldValue?.AppDateId,
                            restIndex = appDateChange.OldValue?.RestIndex + 1,
                            roomIndex = appDateChange.OldValue?.RoomIndex + 1,
                            optionName = appDateChange.OldValue?.OptionName
                        },
                        _ => new
                        {
                            changeType = nameof(ChangeType.Modified),
                            appDateId = appDateChange.NewValue?.AppDateId,
                            restIndex = appDateChange.NewValue?.RestIndex + 1,
                            roomIndex = appDateChange.NewValue?.RoomIndex + 1,
                            optionName = appDateChange.NewValue?.OptionName
                        }
                    }
                }
            )
        ];
    }

    private static List<ChangeLog<QuestionData>> GenerateBookingQuestionAuditChangeLog(
        List<QuestionData> oldList,
        List<QuestionData> newList
    )
    {
        var questionChanges = CompareUtil.Diff(
            oldList,
            newList,
            x => (x.Id, x.Name, x.Description, x.FormData),
            (
                    a,
                    b
                ) => a.Id == b.Id
                && a.Name == b.Name
                && a.Description == b.Description
                && a.FormData == b.FormData
                && a.AnswerData == b.AnswerData
        );

        return questionChanges;
    }

    private static List<ChangeLog<AppDatePersonFlatOfBookingAuditLogResponse>> GenerateNightPeopleAuditChangeLog(
        List<AppDatePersonFlatOfBookingAuditLogResponse> oldList,
        List<AppDatePersonFlatOfBookingAuditLogResponse> newList
    )
    {
        var nightPeople = CompareUtil.Diff(
                oldList,
                newList,
                x => (x.AppDateId, x.RestIndex, x.RoomIndex, x.PersonAgeTypeName, x.Gender),
                (
                    a,
                    b
                ) => a.NumberOfPersons == b.NumberOfPersons
            )
            .FindAll(
                x => (x.OldValue?.NumberOfPersons ?? 0) != (x.NewValue?.NumberOfPersons ?? 0)
            );

        return nightPeople;
    }

    private static List<ChangeLog<AppDateOptionFlatOfBookingAuditLogResponse>> GenerateNightOptionAuditChangeLog(
        List<AppDateOptionFlatOfBookingAuditLogResponse> oldList,
        List<AppDateOptionFlatOfBookingAuditLogResponse> newList
    )
    {
        var optionPeople = CompareUtil.Diff(
                oldList,
                newList,
                x => (x.AppDateId, x.RestIndex, x.RoomIndex, x.OptionName),
                (
                    a,
                    b
                ) => a.NumberOfOptions == b.NumberOfOptions
            )
            .FindAll(
                x => (x.OldValue?.NumberOfOptions ?? 0) != (x.NewValue?.NumberOfOptions ?? 0)
            );

        return optionPeople;
    }

    public static object? FormatToHHmmIfPossible(
        object? value
    )
    {
        switch (value)
        {
            case null: return null;
            case string s:
                {
                    s = s.Trim();
                    return TimeSpan.TryParseExact(s, @"hh\:mm\:ss", CultureInfo.InvariantCulture, out var parsed)
                        ? parsed.ToString(@"hh\:mm", CultureInfo.InvariantCulture)
                        : s;
                }
            default: return value;
        }
    }

    public static string? FormatName(
        string? name
    )
    {
        if (name is null)
        {
            return name;
        }

        return name switch
        {
            "FreeInput" => "Memo",
            "ReserverFullName" => "ReserverName",
            "ReserverEmail" => "ReserverEMail",
            "ReserverPhoneNumber" => "ReserverPhone",
            "MainUserFullName" => "MainUserName",
            "MainUserPhoneNumber" => "MainUserPhone",
            _ => name
        };
    }

    private static List<int> GetRoomRepresentativeIndexes(
        List<ChangeLog<RoomRepresentativeOfReservationAdjustRequest>> roomNameRepresentativeChanges,
        List<ChangeLog<RoomRepresentativeOfReservationAdjustRequest>> roomKanaRepresentativeChanges
    )
    {
        var ints = new List<int>();

        roomNameRepresentativeChanges.ForEach(
            x =>
            {
                if (x.NewValue?.RoomIndex is not null)
                {
                    ints.Add(x.NewValue?.RoomIndex ?? 0);
                }

                if (x.OldValue?.RoomIndex is not null)
                {
                    ints.Add(x.OldValue?.RoomIndex ?? 0);
                }
            }
        );

        roomKanaRepresentativeChanges.ForEach(
            x =>
            {
                if (x.NewValue?.RoomIndex is not null)
                {
                    ints.Add(x.NewValue?.RoomIndex ?? 0);
                }

                if (x.OldValue?.RoomIndex is not null)
                {
                    ints.Add(x.OldValue?.RoomIndex ?? 0);
                }
            }
        );

        ints = [.. ints.Distinct().OrderBy(x => x)];

        return ints;
    }
}
