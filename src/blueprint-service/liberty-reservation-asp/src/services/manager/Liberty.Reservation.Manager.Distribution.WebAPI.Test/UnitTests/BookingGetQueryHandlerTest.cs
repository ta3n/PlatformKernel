using System.ServiceModel.Syndication;
using System.Xml.Linq;
using AutoMapper;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Pagination;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Queries.Rss;
using Liberty.Reservation.Manager.Distribution.WebAPI.Test.InfrastructureOfTest;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Test.UnitTests;

public class BookingGetQueryHandlerTest : BaseUnitTest
{
    private readonly RssEndpoint _controller;

    public BookingGetQueryHandlerTest()
    {
        var mockMediator = new Mock<IMediator>();
        var mapperMock = new Mock<IMapper>();
        var planOfBookingResponse = new BookingSearchByPlanResponse(
            1,
            "Plan A",
            "TagA",
            true,
            false,
            "This is Plan A",
            false,
            null,
            null,
            false,
            null,
            null,
            PlanTypes.Combo,
            1,
            "des",
            false
        )
        {
            BasePrice = 100,
            Categories = [new(1, "Category 1")],
            Files = [new("FileCode1", "application/pdf", 0, true)],
            Meals = [new(1, MealTypeEatTypes.Box, "Meal 1")],
            Rooms =
            [
                new(
                    1,
                    "Room 1",
                    "Tag1",
                    false,
                    "des",
                    1
                )
                {
                    Files = [new("RoomFileCode1", "image/jpeg", 0, true)],
                    AppDatePrices =
                    [
                        new(1, 5)
                        {
                            BasePrice = 50,
                            Price = 60,
                            TotalSpaTax = 5
                        }
                    ]
                }
            ]
        };

        var planOfBookingResponses = new[] { planOfBookingResponse };

        var dict = new Dictionary<string, string> { { "ja", "Facility Name" } };

        var facilityMock = new Facility
        {
            Id = 1,
            Name = new MultilingualText(dict),
            Address1 = new MultilingualText(dict),
            Address2 = new MultilingualText(dict),
            Address3 = new MultilingualText(dict),
            Address4 = new MultilingualText(dict),
            Code = "Code1",
            Phone = "123456",
            Url = "https://www.facility.com",
            Heading1 = new MultilingualText(dict)
        };
        var items = new List<SyndicationItem>();
        var item = BuildBookingXmlStructure(planOfBookingResponses, facilityMock);
        items.Add(item);
        var feed = new SyndicationFeed("Feed Title", "Feed Description", new Uri("https://example.com")) { Items = items };
        mockMediator
            .Setup(mediator => mediator.Send(It.IsAny<BookingGetAllQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new HeaderDictionary(), feed));
        _controller = new RssEndpoint(mapperMock.Object, mockMediator.Object);
    }

    [Fact]
    public async Task SearchBooking_ReturnsResult()
    {
        // Arrange
        var pageable = new Mock<IPageable>();
        var request = new GetBookingSearchRequest(2, 1, 1, ["1", "2"], 20250623);
        // Act
        var result = await _controller.GetAllBooking(request, pageable.Object, CancellationToken.None);
        var contentResult = Assert.IsType<ContentResult>(result); // Ép kiểu

        Assert.NotNull(contentResult.Content);
        Assert.Equal("application/rss+xml; charset=utf-8", contentResult.ContentType);
    }

    private static SyndicationItem BuildBookingXmlStructure(
        IEnumerable<BookingSearchByPlanResponse> bookingSearchResponse,
        Facility facilityDto
    )
    {
        var bookingDataElement = new XElement(
            "bookingData",
            bookingSearchResponse.Select(
                booking =>
                    new XElement(
                        "booking",
                        new XElement("id", booking.Id),
                        new XElement("name", booking.Name),
                        new XElement("tag", booking.Tag),
                        new XElement("summary", booking.Summary),
                        new XElement("isOnlinePayment", booking.IsOnLinePayment),
                        new XElement("isOnSidePayment", booking.IsOnSidePayment),
                        new XElement("planType", booking.PlanType.ToString()),
                        new XElement("displayOrder", booking.DisplayOrder),
                        new XElement("description", booking.Description),
                        new XElement("useDisplayDate", booking.UseDisplayDate),
                        new XElement("displayDateStart", booking.DisplayDateStart),
                        new XElement("displayDateEnd", booking.DisplayDateEnd),
                        new XElement("useAcceptDate", booking.UseAcceptDate),
                        new XElement("acceptDateStart", booking.AcceptDateStart),
                        new XElement("acceptDateEnd", booking.DisplayDateStart),
                        new XElement("minTotalPrice", booking.MinTotalPrice),
                        new XElement("basePrice", booking.BasePrice),
                        new XElement("checkInEnd", booking.CheckInEnd),
                        new XElement(
                            "rooms",
                            booking.Rooms.Select(
                                room =>
                                    new XElement(
                                        "room",
                                        new XElement("id", room.Id),
                                        new XElement("name", room.Name),
                                        new XElement("tag", room.Tag),
                                        new XElement("isEnabledSmoking", room.IsEnabledSmoking),
                                        new XElement("overview", room.Overview),
                                        new XElement("displayOrder", room.DisplayOrder),
                                        new XElement(
                                            "files",
                                            room.Files?.Select(
                                                file =>
                                                    new XElement(
                                                        "file",
                                                        new XElement("code", file.Code),
                                                        new XElement("contentType", file.ContentType),
                                                        new XElement("index", file.Index),
                                                        new XElement("isEnabled", file.IsEnabled)
                                                    )
                                            )
                                            ?? []
                                        ),
                                        new XElement(
                                            "appDatePrices",
                                            room.AppDatePrices.Select(
                                                price =>
                                                    new XElement(
                                                        "price",
                                                        new XElement("appDateId", price.AppDateId),
                                                        new XElement("remainNumber", price.RemainNumber),
                                                        new XElement("basePrice", price.BasePrice),
                                                        new XElement("price", price.Price),
                                                        new XElement("totalSpaTax", price.TotalSpaTax),
                                                        new XElement("totalPrice", price.TotalPrice)
                                                    )
                                            )
                                        )
                                    )
                            )
                        ),
                        new XElement(
                            "meals",
                            booking.Meals?.Select(
                                m =>
                                    new XElement(
                                        "meal",
                                        new XElement("id", m.Id),
                                        new XElement("type", m.MealTypeEatType.ToString()),
                                        new XElement("name", m.Name)
                                    )
                            )
                            ?? []
                        ),
                        new XElement(
                            "files",
                            booking.Files?.Select(
                                file =>
                                    new XElement(
                                        "file",
                                        new XElement("code", file.Code),
                                        new XElement("contentType", file.ContentType),
                                        new XElement("index", file.Index),
                                        new XElement("isEnabled", file.IsEnabled)
                                    )
                            )
                            ?? []
                        ),
                        new XElement(
                            "categories",
                            booking.Categories?.Select(
                                category =>
                                    new XElement(
                                        "category",
                                        new XElement("id", category.Id),
                                        new XElement("name", category.Name)
                                    )
                            )
                            ?? []
                        )
                    )
            )
        );

        var facilityData = new XElement(
            "facility",
            new XElement("id", facilityDto.Id),
            new XElement("heading", facilityDto.Heading1!.GetValueByCode(LanguageHeaderUtil.DefaultLanguageCode)),
            new XElement("name", facilityDto.Name!.GetValueByCode(LanguageHeaderUtil.DefaultLanguageCode)),
            new XElement(
                "address",
                $"{facilityDto.Address1!.GetValueByCode(LanguageHeaderUtil.DefaultLanguageCode)}"
                + $" {facilityDto.Address2!.GetValueByCode(LanguageHeaderUtil.DefaultLanguageCode)}"
                + $" {facilityDto.Address3!.GetValueByCode(LanguageHeaderUtil.DefaultLanguageCode)}"
            ),
            new XElement("code", facilityDto.Code),
            new XElement("phone", facilityDto.Phone),
            new XElement("url", facilityDto.Url),
            bookingDataElement
        );
        var item = new SyndicationItem
        {
            Id = facilityDto.Code,
            Title = new TextSyndicationContent(facilityDto.Name!.GetValueByCode(LanguageHeaderUtil.DefaultLanguageCode)),
            BaseUri = new Uri(facilityDto.Url ?? string.Empty),
            PublishDate = DateTimeOffset.Now
        };
        item.ElementExtensions.Add(facilityData);

        return item;
    }
}
