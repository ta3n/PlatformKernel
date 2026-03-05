using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Application.Utils;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.EntityFrameworkCore;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Application.Domains.Services;

public class BookingAuditCompareService(
    IUnitOfWork unitOfWork
) : IBookingAuditCompareService
{
    public async Task<List<BookingTreeModel>> GetAllBookingTreeAsync(
        long bookingId,
        CancellationToken cancellationToken
    )
    {
        var reservationProp = unitOfWork.GetEntityProperty<ReservationEntity>();

        var bookingTreeQuery = $"""
                WITH RECURSIVE BookingTree AS (
                    SELECT
                        r1.{reservationProp.ColumnName(nameof(ReservationEntity.Id))},
                        r1.{reservationProp.ColumnName(nameof(ReservationEntity.ParentId))}
                    FROM {reservationProp.TableName} r1
                    WHERE r1.id = '{bookingId}'

                    UNION ALL

                    SELECT
                        r2.{reservationProp.ColumnName(nameof(ReservationEntity.Id))},
                        r2.{reservationProp.ColumnName(nameof(ReservationEntity.ParentId))}
                    FROM {reservationProp.TableName} r2
                    JOIN BookingTree t ON r2.{reservationProp.ColumnName(nameof(ReservationEntity.ParentId))} = t.{reservationProp.ColumnName(nameof(ReservationEntity.Id))}
                )
                SELECT * FROM BookingTree
            """;

        var result = await unitOfWork.GetDbContext()
            .Database
            .SqlQueryRaw<BookingTreeModel>(bookingTreeQuery)
            .ToListAsync(cancellationToken);

        return result;
    }

    public List<ChangeOfBookingAuditLogResponse> GetAllBookingAuditLogs(
        BookingHistoryDataModel bookingNew,
        BookingHistoryDataModel bookingOld
    )
    {
        var changeItems = new List<ChangeOfBookingAuditLogResponse>();

        var bookingAuditChangeLog = GetBookingAuditChangeLog(
            bookingNew,
            bookingOld
        );

        changeItems.AddRange(
            GetBasicAuditLogs(
                bookingNew,
                bookingOld,
                [
                    nameof(BookingHistoryDataModel.Basic.Memo)
                ]
            )
        );
        changeItems.AddRange(GetPersonAgeTypeAuditLogs(bookingAuditChangeLog.PersonChanges));
        changeItems.AddRange(GetOptionAuditLogs(bookingAuditChangeLog.OptionChanges));
        changeItems.AddRange(
            GetBasicAuditLogs(
                bookingNew,
                bookingOld,
                [
                    nameof(BookingHistoryDataModel.Basic.CheckInTime),
                    nameof(BookingHistoryDataModel.Basic.NumberOfNights),
                    nameof(BookingHistoryDataModel.Basic.NumberOfRooms)
                ]
            )
        );
        changeItems.AddRange(GetQuestionAuditLogs(bookingNew, bookingOld));
        changeItems.AddRange(GetReserverAuditLogs(bookingNew, bookingOld));
        changeItems.AddRange(GetMainUserAuditLogs(bookingNew, bookingOld));
        changeItems.AddRange(
            GetRoomRepresentativeAuditLogs(
                bookingAuditChangeLog.RoomRepresentativeNameChanges,
                bookingAuditChangeLog.RoomRepresentativeKanaChanges
            )
        );

        return changeItems;
    }

    private static List<ChangeOfBookingAuditLogResponse> GetBasicAuditLogs(
        BookingHistoryDataModel bookingHistoryNew,
        BookingHistoryDataModel bookingHistoryOld,
        string[] ignoreProperties
    )
    {
        const string changeItem = "";

        var differences = CompareUtil.GetDifferences(
            bookingHistoryOld.Basic,
            bookingHistoryNew.Basic,
            ignoreProperties
            // isCheckMemo ? [] : [nameof(BookingHistoryDataModel.Basic.Memo)]
        );

        var auditLogs = new List<ChangeOfBookingAuditLogResponse>();

        foreach (var (propName, beforeValue, afterValue) in differences)
        {
            var auditLog = new ChangeOfBookingAuditLogResponse(
                $"{changeItem}{propName}",
                $"{beforeValue}",
                $"{afterValue}"
            );

            auditLogs.Add(auditLog);
        }

        return auditLogs;
    }

    private static List<ChangeOfBookingAuditLogResponse> GetReserverAuditLogs(
        BookingHistoryDataModel bookingHistoryNew,
        BookingHistoryDataModel bookingHistoryOld
    )
    {
        const string changeItem = "Reserver";
        var differences = CompareUtil.GetDifferences(bookingHistoryOld.Reserver, bookingHistoryNew.Reserver);

        var auditLogs = new List<ChangeOfBookingAuditLogResponse>();

        foreach (var (propName, beforeValue, afterValue) in differences)
        {
            var auditLog = new ChangeOfBookingAuditLogResponse(
                $"{changeItem}{propName}",
                $"{beforeValue}",
                $"{afterValue}"
            );

            auditLogs.Add(auditLog);
        }

        return auditLogs;
    }

    private static List<ChangeOfBookingAuditLogResponse> GetMainUserAuditLogs(
        BookingHistoryDataModel bookingHistoryNew,
        BookingHistoryDataModel bookingHistoryOld
    )
    {
        const string changeItem = "MainUser";
        var differences = CompareUtil.GetDifferences(bookingHistoryOld.MainUser, bookingHistoryNew.MainUser);

        var auditLogs = new List<ChangeOfBookingAuditLogResponse>();

        foreach (var (propName, beforeValue, afterValue) in differences)
        {
            var auditLog = new ChangeOfBookingAuditLogResponse(
                $"{changeItem}{propName}",
                $"{beforeValue}",
                $"{afterValue}"
            );

            auditLogs.Add(auditLog);
        }

        return auditLogs;
    }

    private static List<ChangeOfBookingAuditLogResponse> GetQuestionAuditLogs(
        BookingHistoryDataModel bookingHistoryNew,
        BookingHistoryDataModel bookingHistoryOld
    )
    {
        const string changeItem = "Question";

        var newValue = bookingHistoryNew.BookingData?.GetAllQuestions() ?? [];
        var oldValue = bookingHistoryOld.BookingData?.GetAllQuestions() ?? [];

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
                            ChangeType = ChangeType.Added,
                            appDateChange.NewValue?.Name,
                            appDateChange.NewValue?.Description
                        },
                        ChangeType.Removed => new
                        {
                            ChangeType = ChangeType.Removed,
                            appDateChange.OldValue?.Name,
                            appDateChange.OldValue?.Description
                        },
                        _ => new
                        {
                            ChangeType = ChangeType.Modified,
                            appDateChange.NewValue?.Name,
                            appDateChange.NewValue?.Description
                        }
                    }
                }
            )
        ];
    }

    private static List<ChangeOfBookingAuditLogResponse> GetPersonAgeTypeAuditLogs(
        List<ChangeLog<AppDatePersonFlatOfBookingAuditLogResponse>> personChanges
    )
    {
        const string changeItem = "PersonAgeType";

        if (personChanges is not { Count: > 0 })
        {
            return [];
        }

        return
        [
            .. personChanges.Select(
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
                            ChangeType = ChangeType.Added,
                            appDateChange.NewValue?.AppDateId,
                            appDateChange.NewValue?.RestIndex,
                            appDateChange.NewValue?.RoomIndex,
                            appDateChange.NewValue?.PersonAgeTypeName,
                            appDateChange.NewValue?.Gender
                        },
                        ChangeType.Removed => new
                        {
                            ChangeType = ChangeType.Removed,
                            appDateChange.OldValue?.AppDateId,
                            appDateChange.OldValue?.RestIndex,
                            appDateChange.OldValue?.RoomIndex,
                            appDateChange.OldValue?.PersonAgeTypeName,
                            appDateChange.OldValue?.Gender
                        },
                        _ => new
                        {
                            ChangeType = ChangeType.Modified,
                            appDateChange.NewValue?.AppDateId,
                            appDateChange.NewValue?.RestIndex,
                            appDateChange.NewValue?.RoomIndex,
                            appDateChange.NewValue?.PersonAgeTypeName,
                            appDateChange.NewValue?.Gender
                        }
                    }
                }
            )
        ];
    }

    private static List<ChangeOfBookingAuditLogResponse> GetOptionAuditLogs(
        List<ChangeLog<AppDateOptionFlatOfBookingAuditLogResponse>> optionChanges
    )
    {
        const string changeItem = "Option";

        if (optionChanges is not { Count: > 0 })
        {
            return [];
        }

        return
        [
            .. optionChanges.Select(
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
                            ChangeType = ChangeType.Added,
                            appDateChange.NewValue?.AppDateId,
                            appDateChange.NewValue?.RestIndex,
                            appDateChange.NewValue?.RoomIndex,
                            appDateChange.NewValue?.OptionName
                        },
                        ChangeType.Removed => new
                        {
                            ChangeType = ChangeType.Removed,
                            appDateChange.OldValue?.AppDateId,
                            appDateChange.OldValue?.RestIndex,
                            appDateChange.OldValue?.RoomIndex,
                            appDateChange.OldValue?.OptionName
                        },
                        _ => new
                        {
                            ChangeType = ChangeType.Modified,
                            appDateChange.NewValue?.AppDateId,
                            appDateChange.NewValue?.RestIndex,
                            appDateChange.NewValue?.RoomIndex,
                            appDateChange.NewValue?.OptionName
                        }
                    }
                }
            )
        ];
    }

    private static List<ChangeOfBookingAuditLogResponse> GetRoomRepresentativeAuditLogs(
        List<ChangeLog<AppDateRoomRepresentativeNameFlatOfBookingAuditLogResponse>> roomNameRepresentativeChanges,
        List<ChangeLog<AppDateRoomRepresentativeKanaFlatOfBookingAuditLogResponse>> roomKanaRepresentativeChanges
    )
    {
        const string changeItem = "RoomRepresentative";

        var roomIndexes = GetRoomRepresentativeIndexes(
            roomNameRepresentativeChanges,
            roomKanaRepresentativeChanges
        );

        if (roomIndexes is not { Count: > 0 })
        {
            return [];
        }

        var auditLogs = new List<ChangeOfBookingAuditLogResponse>();

        foreach (var roomIndex in roomIndexes)
        {
            var index = roomIndex;
            var roomRepresentativeName = roomNameRepresentativeChanges.Find(
                x => x.NewValue?.RoomIndex == index || x.OldValue?.RoomIndex == index
            );
            var roomRepresentativeKana = roomKanaRepresentativeChanges.Find(
                x => x.NewValue?.RoomIndex == roomIndex || x.OldValue?.RoomIndex == roomIndex
            );

            if (roomRepresentativeName is not null)
            {
                auditLogs.Add(
                    new ChangeOfBookingAuditLogResponse(
                        $"{changeItem}Name",
                        roomRepresentativeName.OldValue?.Name,
                        roomRepresentativeName.NewValue?.Name
                    )
                    {
                        ChangeItemData = roomRepresentativeName.Type switch
                        {
                            ChangeType.Added => new
                            {
                                ChangeType = ChangeType.Added,
                                roomRepresentativeName.NewValue?.RoomIndex
                            },
                            ChangeType.Removed => new
                            {
                                ChangeType = ChangeType.Removed,
                                roomRepresentativeName.OldValue?.RoomIndex
                            },
                            _ => new
                            {
                                ChangeType = ChangeType.Modified,
                                roomRepresentativeName.NewValue?.RoomIndex
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
                                ChangeType = ChangeType.Added,
                                roomRepresentativeKana.NewValue?.RoomIndex
                            },
                            ChangeType.Removed => new
                            {
                                ChangeType = ChangeType.Removed,
                                roomRepresentativeKana.OldValue?.RoomIndex
                            },
                            _ => new
                            {
                                ChangeType = ChangeType.Modified,
                                roomRepresentativeKana.NewValue?.RoomIndex
                            }
                        }
                    }
                );
            }
        }

        return auditLogs;
    }

    private static List<int> GetRoomRepresentativeIndexes(
        List<ChangeLog<AppDateRoomRepresentativeNameFlatOfBookingAuditLogResponse>> roomNameRepresentativeChanges,
        List<ChangeLog<AppDateRoomRepresentativeKanaFlatOfBookingAuditLogResponse>> roomKanaRepresentativeChanges
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

    private static BookingAuditChangeLog GetBookingAuditChangeLog(
        BookingHistoryDataModel bookingHistoryNew,
        BookingHistoryDataModel bookingHistoryOld
    )
    {
        var newValue = bookingHistoryNew.BookingData?.AppDates?.Select(
                    appDate => new AppDateOfBookingAuditLogResponse(
                        appDate.AppDateId,
                        appDate.RestIndex + 1,
                        appDate.Rooms.Select(
                            room => new RoomOfBookingAuditLogResponse(
                                room.RoomIndex + 1,
                                room.PricePeoples.Select(
                                    people => new PersonOfBookingAuditLogResponse(
                                        people.PersonAgeType.Name,
                                        people.Persons,
                                        people.MalePersons ?? 0,
                                        people.FemalePersons ?? 0,
                                        people.NonePersons ?? 0
                                    )
                                ),
                                room.OptionItems?.Select(
                                    option => new OptionOfBookingAuditLogResponse(
                                        option.Name,
                                        option.Number
                                    )
                                )
                                ?? [],
                                new RoomRepresentativeOfBookingAuditLogResponse(
                                    room.CustomerInfo?.Name,
                                    room.CustomerInfo?.Kana
                                )
                            )
                        )
                    )
                )
                .ToList()
            ?? [];

        var oldValue = bookingHistoryOld.BookingData?.AppDates?.Select(
                    appDate => new AppDateOfBookingAuditLogResponse(
                        appDate.AppDateId,
                        appDate.RestIndex + 1,
                        appDate.Rooms.Select(
                            room => new RoomOfBookingAuditLogResponse(
                                room.RoomIndex + 1,
                                room.PricePeoples.Select(
                                    people => new PersonOfBookingAuditLogResponse(
                                        people.PersonAgeType.Name,
                                        people.Persons,
                                        people.MalePersons ?? 0,
                                        people.FemalePersons ?? 0,
                                        people.NonePersons ?? 0
                                    )
                                ),
                                room.OptionItems?.Select(
                                    option => new OptionOfBookingAuditLogResponse(
                                        option.Name,
                                        option.Number
                                    )
                                )
                                ?? [],
                                new RoomRepresentativeOfBookingAuditLogResponse(
                                    room.CustomerInfo?.Name,
                                    room.CustomerInfo?.Kana
                                )
                            )
                        )
                    )
                )
                .ToList()
            ?? [];

        var compareAuditChangeLog = GenerateBookingAuditChangeLog(
            oldValue,
            newValue
        );

        return compareAuditChangeLog;
    }

    private static BookingAuditChangeLog GenerateBookingAuditChangeLog(
        List<AppDateOfBookingAuditLogResponse> oldList,
        List<AppDateOfBookingAuditLogResponse> newList
    )
    {
        var oldPersons = oldList.SelectMany(o => o.GetFlatPersons()).ToList();
        var newPersons = newList.SelectMany(n => n.GetFlatPersons()).ToList();

        var oldOptions = oldList.SelectMany(o => o.GetFlatOptions()).ToList();
        var newOptions = newList.SelectMany(n => n.GetFlatOptions()).ToList();

        var oldNameReps = oldList.SelectMany(o => o.GetFlatRoomNameRepresentatives()).Distinct().ToList();
        var newNameReps = newList.SelectMany(n => n.GetFlatRoomNameRepresentatives()).Distinct().ToList();

        var oldKanaReps = oldList.SelectMany(o => o.GetFlatRoomNameKanaRepresentatives()).Distinct().ToList();
        var newKanaReps = newList.SelectMany(n => n.GetFlatRoomNameKanaRepresentatives()).Distinct().ToList();

        var personChanges = CompareUtil.Diff(
                oldPersons,
                newPersons,
                x => (x.AppDateId, x.RestIndex, x.RoomIndex, x.PersonAgeTypeName, x.Gender),
                (
                    a,
                    b
                ) => a.NumberOfPersons == b.NumberOfPersons
            )
            .FindAll(
                x => (x.OldValue?.NumberOfPersons ?? 0) != (x.NewValue?.NumberOfPersons ?? 0)
            );

        var optionChanges = CompareUtil.Diff(
                oldOptions,
                newOptions,
                x => (x.AppDateId, x.RestIndex, x.RoomIndex, x.OptionName),
                (
                    a,
                    b
                ) => a.NumberOfOptions == b.NumberOfOptions
            )
            .FindAll(
                x => (x.OldValue?.NumberOfOptions ?? 0) != (x.NewValue?.NumberOfOptions ?? 0)
            );

        var repNameChanges = CompareUtil.Diff(
                oldNameReps,
                newNameReps,
                x => x.RoomIndex,
                (
                    a,
                    b
                ) => (a.Name ?? string.Empty) == (b.Name ?? string.Empty)
            )
            .FindAll(
                x => (x.OldValue?.Name ?? string.Empty) != (x.NewValue?.Name ?? string.Empty)
            );

        var repKanaChanges = CompareUtil.Diff(
                oldKanaReps,
                newKanaReps,
                x => x.RoomIndex,
                (
                    a,
                    b
                ) => (a.Kana ?? string.Empty) == (b.Kana ?? string.Empty)
            )
            .FindAll(
                x => (x.OldValue?.Kana ?? string.Empty) != (x.NewValue?.Kana ?? string.Empty)
            );

        return new BookingAuditChangeLog(
            personChanges,
            optionChanges,
            repNameChanges,
            repKanaChanges
        );
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
}
