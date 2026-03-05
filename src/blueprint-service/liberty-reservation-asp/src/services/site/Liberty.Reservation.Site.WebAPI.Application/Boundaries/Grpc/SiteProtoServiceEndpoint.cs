using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Liberty.Protobuf.Site.V1;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Site.WebAPI.Application.Boundaries.Grpc.Base;
using Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Booking;
using Microsoft.Extensions.Logging;
using Pageable = Liberty.Pagination.Pageable;

namespace Liberty.Reservation.Site.WebAPI.Application.Boundaries.Grpc;

public class SiteProtoServiceEndpoint(
    ILogger<SiteProtoServiceEndpoint> logger,
    IMediator mediator,
    IFacilityService facilityService
) : SiteProtoEndpoint.SiteProtoEndpointBase
{
    public override Task<PongProtoReply> Ping(
        Empty request,
        ServerCallContext context
    )
    {
        var dateNowUtc = DateTimeOffset.UtcNow;
        var message = $"Pong from Site Service at {dateNowUtc:O}";

        logger.LogInformation("Ping request received. {Message}", message);

        return new Task<PongProtoReply>(() => new PongProtoReply { Message = message });
    }

    public override async Task<GetFacilitySiteProtoReply> GetFacilitySite(
        GetFacilitySiteProtoRequest request,
        ServerCallContext context
    )
    {
        return await GrpcErrorHandler.ExecuteAsync(
            logger,
            context,
            async () =>
            {
                await SetFacilitySiteToHeaderAsync(
                    context,
                    request.FacilityCode,
                    request.SiteCode
                );

                var (_, response) = await mediator.Send(
                    new BookingGetFacilityQuery()
                );

                var reply = new GetFacilitySiteProtoReply
                {
                    Code = response.Code,
                    Name = response.Name,
                    Description = response.Description ?? string.Empty,
                    IsOnLinePayment = response.IsOnLinePayment,
                    IsOnSidePayment = response.IsOnSidePayment,
                    UseDailyPerson = response.UseDailyPerson,
                    CanAddRoomOnModify = response.CanAddRoomOnModify,
                    Postcode = response.Postcode,
                    Url = response.Url ?? string.Empty,
                    File =
                        new()
                        {
                            Code = response.File?.Code ?? string.Empty,
                            ContentType = response.File?.ContentType ?? string.Empty
                        },
                    Site =
                        new()
                        {
                            Code = response.Site?.Code,
                            Name = response.Site?.Name,
                            MaxPeople = response.Site?.MaxPeople ?? 0
                        },
                    Heading = response.Heading,
                    IsBarrierFree = response.IsBarrierFree,
                    BarrierFreeInfoComment = response.BarrierFreeInfoComment ?? string.Empty,
                    Email = response.Email ?? string.Empty,
                    UseSpaTax = response.UseSpaTax ?? false,
                    SpaTaxComment = response.SpaTaxComment ?? string.Empty,
                    SpaTaxTable = response.SpaTaxTable ?? string.Empty,
                    Logo = response.Logo ?? string.Empty,
                    Address1 = response.Address1 ?? string.Empty,
                    Address2 = response.Address2 ?? string.Empty,
                    Address3 = response.Address3 ?? string.Empty,
                    Address4 = response.Address4 ?? string.Empty
                };

                if (response.PersonAgeTypes is not null)
                {
                    reply.PersonAgeTypes.AddRange(
                        response.PersonAgeTypes.Select(
                            x => new PersonAgeTypeOfBookingFacilityProtoModel
                            {
                                Id = x.Id,
                                IsMain = x.IsMain,
                                AgeMin = x.AgeMin ?? 0,
                                AgeMax = x.AgeMax ?? 0,
                                Name = x.Name,
                                LastUpdatedAt = x.LastUpdatedAt ?? 0
                            }
                        )
                    );
                }

                return reply;
            }
        );
    }

    public override async Task<SearchBookingByPlanProtoReply> SearchBookingByPlan(
        SearchBookingByPlanProtoRequest request,
        ServerCallContext context
    )
    {
        return await GrpcErrorHandler.ExecuteAsync(
            logger,
            context,
            async () =>
            {
                var model = request.BookingSearch;

                await SetFacilitySiteToHeaderAsync(
                    context,
                    model.FacilityCode,
                    model.SiteCode
                );

                var protoPageable = request.Pageable;

                var pageable = Pageable.Of(
                    protoPageable.PageNumber,
                    protoPageable.PageSize,
                    protoPageable.IsEnabled
                );

                var commandReq = new BookingSearchPlanRequest
                {
                    CheckInDate = model.CheckInDate!.Value,
                    CheckOutDate = model.CheckOutDate!.Value,
                    DisplayCheckInDate = model.DisplayCheckInDate,
                    DisplayCheckOutDate = model.DisplayCheckOutDate,
                    RestNumber = model.RestNumber!.Value,
                    RoomNumber = model.RoomNumber!.Value,
                    MinPrice = model.MinPrice,
                    MaxPrice = model.MaxPrice,
                    Secret = model.Secret,
                    DayUse = model.DayUse,
                    GuestsPerRoom = model.GuestsPerRoom?.Select(
                        x => new PersonOfBookingSearchModel
                        {
                            AppDateId = x.AppDateId,
                            RestIndex = x.RestIndex,
                            RoomGroupIndex = x.RoomGroupIndex,
                            PersonAgeTypeId = x.PersonAgeTypeId,
                            Persons = x.Persons,
                            MalePersons = x.MalePersons,
                            FemalePersons = x.FemalePersons
                        }
                    ),
                    OptionItems = model.OptionItems?.Select(
                        x => new OptionOfBookingSearchModel
                        {
                            AppDateId = x.AppDateId,
                            RoomGroupIndex = x.RoomGroupIndex,
                            OptionItemId = x.OptionItemId,
                            Number = x.Number
                        }
                    )
                };

                var (headers, commandRes) = await mediator.Send(
                    new BookingSearchByPlanQuery(commandReq, pageable)
                );

                var reply = new SearchBookingByPlanProtoReply();

                reply.Headers.AddRange(
                    headers.Select(
                        header => new HeaderValueProtoModel
                        {
                            Key = header.Key,
                            Value = header.Value.ToString()
                        }
                    )
                );

                reply.Data.AddRange(
                    commandRes.Select(
                        response => new BookingSearchByPlanProtoModel
                        {
                            Code = response.Code,
                            Name = response.Name,
                            Tag = response.Tag,
                            IsOnLinePayment = response.IsOnLinePayment,
                            IsOnSidePayment = response.IsOnSidePayment,
                            Summary = response.Summary,
                            UseDisplayDate = response.UseDisplayDate,
                            DisplayDateStart = response.DisplayDateStart,
                            DisplayDateEnd = response.DisplayDateEnd,
                            UseAcceptDate = response.UseAcceptDate,
                            AcceptDateStart = response.AcceptDateStart,
                            AcceptDateEnd = response.AcceptDateEnd,
                            PlanType = $"{response.PlanType}",
                            DisplayOrder = response.DisplayOrder,
                            Description = response.Description,
                            DayUse = response.DayUse,
                            FacilityId = response.FacilityId,
                            MinTotalPrice = response.MinTotalPrice != null ? (double)response.MinTotalPrice : 0,
                            BasePrice = response.BasePrice != null ? (double)response.BasePrice : 0,
                            CheckInStart = Duration.FromTimeSpan(response.CheckInStart ?? TimeSpan.MinValue),
                            CheckInEnd = Duration.FromTimeSpan(response.CheckInEnd ?? TimeSpan.MinValue),
                            CheckOut = Duration.FromTimeSpan(response.CheckOut ?? TimeSpan.MinValue),
                            Categories =
                            {
                                (response.Categories ?? Enumerable.Empty<CategoryOfPlanResponse>()).Select(
                                    category =>
                                        new CategoryOfPlanProtoModel
                                        {
                                            Code = category.Code,
                                            Name = category.Name
                                        }
                                )
                            },
                            Files =
                            {
                                (response.Files ?? Enumerable.Empty<FileOfPlanResponse>()).Select(
                                    file => new FileOfPlanProtoModel
                                    {
                                        Code = file.Code,
                                        ContentType = file.ContentType,
                                        Index = file.Index,
                                        IsEnabled = file.IsEnabled
                                    }
                                )
                            },
                            Meals =
                            {
                                (response.Meals ?? Enumerable.Empty<MealOfPlanResponse>()).Select(
                                    meal => new MealOfPlanProtoModel
                                    {
                                        Code = meal.Code,
                                        MealTypeEatType = $"{meal.MealTypeEatType}",
                                        Name = meal.Name
                                    }
                                )
                            },
                            Rooms =
                            {
                                response.Rooms.Select(
                                    room => new RoomOfPlanProtoModel
                                    {
                                        Code = room.Code,
                                        Name = room.Name,
                                        Tag = room.Tag,
                                        IsEnabledSmoking = room.IsEnabledSmoking,
                                        DisplayOrder = room.DisplayOrder,
                                        Files =
                                        {
                                            room.Files?.Select(
                                                file => new FileOfPlanProtoModel
                                                {
                                                    Code = file.Code,
                                                    ContentType = file.ContentType,
                                                    IsEnabled = file.IsEnabled
                                                }
                                            )
                                        },
                                        AppDatePrices =
                                        {
                                            room.AppDatePrices.Select(
                                                appDatePrice => new AppDatePriceOfPlanProtoModel
                                                {
                                                    AppDateId = appDatePrice.AppDateId,
                                                    RemainNumber = appDatePrice.RemainNumber ?? 0,
                                                    BasePrice = (double)(appDatePrice.BasePrice ?? 0),
                                                    Price = (double)(appDatePrice.Price ?? 0),
                                                    TotalSpaTax = (double)(appDatePrice.TotalSpaTax ?? 0),
                                                    TotalPrice = (double)(appDatePrice.TotalPrice ?? 0),
                                                    Status = new AppDatePriceStatusSearchProtoModel
                                                    {
                                                        IsRoomAvailable = appDatePrice.Status.IsRoomAvailable,
                                                        IsRoomUnderRequested = appDatePrice.Status.IsRoomUnderRequested,
                                                        IsAcceptDate = appDatePrice.Status.IsAcceptDate,
                                                        IsDayBookable = appDatePrice.Status.IsDayBookable,
                                                        IsNight = appDatePrice.Status.IsNight,
                                                        IsAvailable = appDatePrice.Status.IsAvailable
                                                    }
                                                }
                                            )
                                        }
                                    }
                                )
                            }
                        }
                    )
                );

                return reply;
            }
        );
    }

    private async Task<(long facilityId, long siteId)> GetFacilitySiteAsync(
        string facilityCode,
        string siteCode
    )
    {
        var (_, facilityId, siteId) = await facilityService.CheckSiteCodeAlreadyInFacilityAsync(
            facilityCode,
            siteCode
        );

        return (facilityId, siteId);
    }

    private async Task SetFacilitySiteToHeaderAsync(
        ServerCallContext context,
        string facilityCode,
        string siteCode
    )
    {
        var (facilityId, siteId) = await GetFacilitySiteAsync(facilityCode, siteCode);

        context.GetHttpContext()
            .Request.Headers.Append(
                SecurityContextAccessor.FacilityIdHeaderKey,
                $"{facilityId}"
            );

        context.GetHttpContext()
            .Request.Headers.Append(
                SecurityContextAccessor.SiteIdOfFacilityHeaderKey,
                $"{siteId}"
            );
    }
}
