using System.Net;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Facility;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Facility;
using Liberty.Reservation.Manager.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests;

public class FacilityEndpointUnitTest : BaseUnitTest
{
    protected override void InitData()
    {
        var mockMediator = new Mock<IMediator>();

        mockMediator
            .Setup(mediator => mediator.Send(It.IsAny<FacilityUpdateAccessCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        mockMediator
            .Setup(mediator => mediator.Send(It.IsAny<FacilityUpdateAcceptCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        mockMediator
            .Setup(mediator => mediator.Send(It.IsAny<FacilityUpdateBasicSettingCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        mockMediator
            .Setup(mediator => mediator.Send(It.IsAny<FacilityUpdateBathCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        mockMediator
            .Setup(mediator => mediator.Send(It.IsAny<FacilityUpdateClassificationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        mockMediator
            .Setup(mediator => mediator.Send(It.IsAny<FacilityUpdatePaymentMethodCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        mockMediator
            .Setup(mediator => mediator.Send(It.IsAny<FacilityUpdatePublicationInformationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        mockMediator
            .Setup(mediator => mediator.Send(It.IsAny<FacilityUpdateReservationChangeCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        mockMediator
            .Setup(mediator => mediator.Send(It.IsAny<FacilityUpdateReservationSettingCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        MockMediator = mockMediator.Object;
    }

    [Fact]
    public async Task GetBasicSetting_ReturnsCorrectResult()
    {
        // Arrange
        var facility = new Facility
        {
            Code = "Test",
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test Name" } },
            Kana = "Test Kana",
            Description = "Test Description",
            Postcode = "1111",
            Address1 = new MultilingualText { { TestUtil.DefaultLanguageCode, "Address 1" } },
            Address2 = new MultilingualText { { TestUtil.DefaultLanguageCode, "Address 2" } },
            Address3 = new MultilingualText { { TestUtil.DefaultLanguageCode, "Address 3" } },
            Address4 = new MultilingualText { { TestUtil.DefaultLanguageCode, "Address 4" } },
            Phone = "123456789",
            Fax = "123456789",
            Url = "https://test.com",
            AreaId = 1,
            CategoryId = 1,
            Meta = new FacilityMeta
            {
                RoomNumberWesternStyle = 1,
                RoomNumberJapaneseStyle = 1,
                RoomNumberJapaneseWesternStyle = 1,
                RoomNumberOtherStyle = 1
            }
        };

        var mockMediator = new Mock<IMediator>();

        mockMediator
            .Setup(mediator => mediator.Send(It.IsAny<FacilityGetBasicSettingQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (
                    FacilityGetBasicSettingQuery _,
                    CancellationToken _
                ) =>
                {
                    var basicSettingResponse = new FacilityDetailBasicSettingResponse(
                        "Test",
                        "Test Name",
                        "Test Kana",
                        "Test Description",
                        "1111",
                        "Address 1",
                        "Address 2",
                        "Address 3",
                        "Address 4",
                        "123456789",
                        "123456789",
                        "https://test.com",
                        1,
                        1,
                        1,
                        1,
                        1,
                        1,
                        new FileOfFacilityResponse("test"),
                        TimeSpan.FromHours(9),
                        "Asia/Tokyo"
                    );

                    return (new HeaderDictionary(), basicSettingResponse);
                }
            );

        var mockMapper = MockServices.MockMapper();
        var controller = new FacilityEndpoint(mockMapper, mockMediator.Object);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetBasicSetting(cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<FacilityDetailBasicSettingResponse>(okResult.Value, false);
        Assert.Equal(facility.Code, response.Code);
        Assert.Equal(facility.Kana, response.Kana);
        Assert.Equal(facility.Description, response.Description);
        Assert.Equal(facility.Postcode, response.PostCode);
        Assert.Equal(facility.Phone, response.Phone);
        Assert.Equal(facility.Fax, response.Fax);
        Assert.Equal(facility.Url, response.Url);
        Assert.Equal(facility.AreaId, response.AreaId);
        Assert.Equal(facility.CategoryId, response.FacilityTypeId);
        Assert.Equal(facility.Meta!.RoomNumberWesternStyle, response.RoomNumberWesternStyle);
        Assert.Equal(facility.Meta!.RoomNumberJapaneseWesternStyle, response.RoomNumberJapaneseWesternStyle);
        Assert.Equal(facility.Meta!.RoomNumberJapaneseStyle, response.RoomNumberJapaneseStyle);
        Assert.Equal(facility.Meta!.RoomNumberOtherStyle, response.RoomNumberOtherStyle);

        Assert.NotNull(headers);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }

    [Fact]
    public async Task GetAccept_ReturnsCorrectResult()
    {
        // Arrange
        var facility = new Facility
        {
            Code = "Test",
            BarrierFreeInfoComment = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
            Meta = new FacilityMeta
            {
                IsAcceptChildren = true,
                AcceptChildrenInfoComment = "Comment",
                IsAcceptPet = true,
                AcceptPetInfoComment = "Comment",
                IsBarrierFree = true
            }
        };

        var mockMediator = new Mock<IMediator>();

        mockMediator
            .Setup(mediator => mediator.Send(It.IsAny<FacilityGetGroupDetailsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (
                    FacilityGetGroupDetailsQuery _,
                    CancellationToken _
                ) =>
                {
                    var acceptResponse = new FacilityDetailAcceptResponse
                    {
                        Code = "Test",
                        IsAcceptChildren = true,
                        AcceptChildrenInfoComment = "Comment",
                        IsAcceptPet = true,
                        AcceptPetInfoComment = "Comment",
                        IsBarrierFree = true
                    };

                    return (new HeaderDictionary(), acceptResponse);
                }
            );

        var mockMapper = MockServices.MockMapper();
        var controller = new FacilityEndpoint(mockMapper, mockMediator.Object);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetAccept(cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<FacilityDetailAcceptResponse>(okResult.Value, false);
        Assert.Equal(facility.Code, response.Code);
        Assert.Equal(facility.Meta!.IsAcceptChildren, response.IsAcceptChildren);
        Assert.Equal(facility.Meta!.AcceptChildrenInfoComment, response.AcceptChildrenInfoComment);
        Assert.Equal(facility.Meta!.IsAcceptPet, response.IsAcceptPet);
        Assert.Equal(facility.Meta!.AcceptPetInfoComment, response.AcceptPetInfoComment);
        Assert.Equal(facility.Meta!.IsBarrierFree, response.IsBarrierFree);

        Assert.NotNull(headers);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }

    [Fact]
    public async Task GetAccess_ReturnsCorrectResult()
    {
        // Arrange
        var facility = new Facility
        {
            Code = "Test",
            Longitude = 111,
            Latitude = 111,
            Meta = new FacilityMeta
            {
                ExistsParking = true,
                CanTransfer = true
            },
            AccessInfoComment = new MultilingualText { { TestUtil.DefaultLanguageCode, "Comment" } },
            ParkingInfoComment = new MultilingualText { { TestUtil.DefaultLanguageCode, "Comment" } },
            TransferComment = new MultilingualText { { TestUtil.DefaultLanguageCode, "Comment" } },
            NearStationInfoComment = new MultilingualText { { TestUtil.DefaultLanguageCode, "Comment" } }
        };

        var mockMediator = new Mock<IMediator>();

        mockMediator
            .Setup(mediator => mediator.Send(It.IsAny<FacilityGetGroupDetailsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (
                    FacilityGetGroupDetailsQuery _,
                    CancellationToken _
                ) =>
                {
                    var acceptResponse = new FacilityDetailAccessResponse
                    {
                        Code = "Test",
                        Latitude = 111,
                        Longitude = 111,
                        AccessInfoComment = "Comment",
                        ParkingInfoComment = "Comment",
                        ExistsParking = true,
                        TransferComment = "Comment",
                        CanTransfer = true,
                        NearStationInfoComment = "Comment"
                    };

                    return (new HeaderDictionary(), acceptResponse);
                }
            );

        var mockMapper = MockServices.MockMapper();
        var controller = new FacilityEndpoint(mockMapper, mockMediator.Object);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetAccess(cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<FacilityDetailAccessResponse>(okResult.Value, false);
        Assert.Equal(facility.Code, response.Code);
        Assert.Equal(facility.AccessInfoComment.GetValueByHeader(), response.AccessInfoComment);
        Assert.Equal(facility.ParkingInfoComment.GetValueByHeader(), response.ParkingInfoComment);
        Assert.Equal(facility.TransferComment.GetValueByHeader(), response.TransferComment);
        Assert.Equal(facility.NearStationInfoComment.GetValueByHeader(), response.NearStationInfoComment);
        Assert.Equal(facility.Meta!.ExistsParking, response.ExistsParking);
        Assert.Equal(facility.Meta.CanTransfer, response.CanTransfer);
        Assert.Equal(facility.Latitude, response.Latitude);
        Assert.Equal(facility.Longitude, response.Longitude);

        Assert.NotNull(headers);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }

    [Fact]
    public async Task GetBath_ReturnsCorrectResult()
    {
        // Arrange
        var facility = new Facility
        {
            Code = "Test",
            Meta = new FacilityMeta
            {
                SpaType = "Type",
                SpaName = "Name",
                SpaDescription = "Description",
                SpaInfoComment = "Comment"
            }
        };

        var mockMediator = new Mock<IMediator>();

        mockMediator
            .Setup(mediator => mediator.Send(It.IsAny<FacilityGetGroupDetailsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (
                    FacilityGetGroupDetailsQuery _,
                    CancellationToken _
                ) =>
                {
                    var acceptResponse = new FacilityDetailBathResponse
                    {
                        Code = "Test",
                        SpaType = "Type",
                        SpaName = "Name",
                        SpaDescription = "Description",
                        SpaInfoComment = "Comment"
                    };

                    return (new HeaderDictionary(), acceptResponse);
                }
            );

        var mockMapper = MockServices.MockMapper();
        var controller = new FacilityEndpoint(mockMapper, mockMediator.Object);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetBath(cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<FacilityDetailBathResponse>(okResult.Value, false);
        Assert.Equal(facility.Code, response.Code);
        Assert.Equal(facility.Meta!.SpaType, response.SpaType);
        Assert.Equal(facility.Meta!.SpaName, response.SpaName);
        Assert.Equal(facility.Meta!.SpaDescription, response.SpaDescription);
        Assert.Equal(facility.Meta!.SpaInfoComment, response.SpaInfoComment);

        Assert.NotNull(headers);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }

    [Fact]
    public async Task GetClassification_ReturnsCorrectResult()
    {
        // Arrange
        var facility = new Facility { Code = "Test" };

        var mockMediator = new Mock<IMediator>();

        mockMediator
            .Setup(mediator => mediator.Send(It.IsAny<FacilityGetGroupDetailsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (
                    FacilityGetGroupDetailsQuery _,
                    CancellationToken _
                ) =>
                {
                    var acceptResponse = new FacilityDetailClassificationResponse
                    {
                        Code = "Test",
                        Allergens = [],
                        Amenities = [],
                        Baths = [],
                        Equipments = [],
                        Features = [],
                        Meals = [],
                        Sceneries = [],
                        Services = []
                    };

                    return (new HeaderDictionary(), acceptResponse);
                }
            );

        var mockMapper = MockServices.MockMapper();
        var controller = new FacilityEndpoint(mockMapper, mockMediator.Object);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetClassification(cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<FacilityDetailClassificationResponse>(okResult.Value, false);
        Assert.Equal(facility.Code, response.Code);
        Assert.NotNull(response.Allergens);
        Assert.NotNull(response.Amenities);
        Assert.NotNull(response.Baths);
        Assert.NotNull(response.Equipments);
        Assert.NotNull(response.Features);
        Assert.NotNull(response.Meals);
        Assert.NotNull(response.Sceneries);
        Assert.NotNull(response.Services);

        Assert.NotNull(headers);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }

    [Fact]
    public async Task GetPaymentMethod_ReturnsCorrectResult()
    {
        // Arrange
        var facility = new Facility
        {
            Code = "Test",
            IsOnLinePayment = true,
            IsOnSidePayment = true,
            CanOnLinePayment = true,
            Meta = new FacilityMeta
            {
                OnLinePaymentComment = "Comment",
                OnSidePaymentComment = "Comment",
                PaymentComment = "Comment"
            }
        };

        var mockMediator = new Mock<IMediator>();

        mockMediator
            .Setup(mediator => mediator.Send(It.IsAny<FacilityGetGroupDetailsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (
                    FacilityGetGroupDetailsQuery _,
                    CancellationToken _
                ) =>
                {
                    var acceptResponse = new FacilityDetailPaymentMethodResponse
                    {
                        Code = "Test",
                        IsOnLinePayment = true,
                        IsOnSidePayment = true,
                        CanOnLinePayment = true,
                        OnLinePaymentComment = "Comment",
                        OnSidePaymentComment = "Comment",
                        PaymentComment = "Comment"
                    };

                    return (new HeaderDictionary(), acceptResponse);
                }
            );

        var mockMapper = MockServices.MockMapper();
        var controller = new FacilityEndpoint(mockMapper, mockMediator.Object);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetPaymentMethod(cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<FacilityDetailPaymentMethodResponse>(okResult.Value, false);
        Assert.Equal(facility.Code, response.Code);
        Assert.Equal(facility.Meta!.OnLinePaymentComment, response.OnLinePaymentComment);
        Assert.Equal(facility.Meta!.OnSidePaymentComment, response.OnSidePaymentComment);
        Assert.Equal(facility.Meta!.PaymentComment, response.PaymentComment);
        Assert.Equal(facility.IsOnLinePayment, response.IsOnLinePayment);
        Assert.Equal(facility.IsOnSidePayment, response.IsOnSidePayment);
        Assert.Equal(facility.CanOnLinePayment, response.CanOnLinePayment);

        Assert.NotNull(headers);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }

    [Fact]
    public async Task GetDetailPublish_ReturnsCorrectResult()
    {
        // Arrange
        var facility = new Facility
        {
            Code = "Test",
            IsOnLinePayment = true,
            IsOnSidePayment = true,
            CanOnLinePayment = true,
            Heading1 = new MultilingualText { { TestUtil.DefaultLanguageCode, "Heading" } },
            Meta = new FacilityMeta
            {
                EquipmentInfoComment = "Comment",
                RoomInfoComment = "Comment",
                PRPointComment = "Comment",
                AmenityInfoComment = "Comment",
                LeisureInfoComment = "Comment",
                FAQInfoComment = "Comment",
                OtherInfoComment = "Comment",
                MapUrl = "Url"
            }
        };

        var mockMediator = new Mock<IMediator>();

        mockMediator
            .Setup(mediator => mediator.Send(It.IsAny<FacilityGetGroupDetailsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (
                    FacilityGetGroupDetailsQuery _,
                    CancellationToken _
                ) =>
                {
                    var acceptResponse = new FacilityDetailPublishResponse
                    {
                        Code = "Test",
                        EquipmentInfoComment = "Comment",
                        RoomInfoComment = "Comment",
                        PrPointComment = "Comment",
                        AmenityInfoComment = "Comment",
                        LeisureInfoComment = "Comment",
                        FaqInfoComment = "Comment",
                        OtherInfoComment = "Comment",
                        MapUrl = "Url"
                    };

                    return (new HeaderDictionary(), acceptResponse);
                }
            );

        var mockMapper = MockServices.MockMapper();
        var controller = new FacilityEndpoint(mockMapper, mockMediator.Object);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetAcceptPublish(cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<FacilityDetailPublishResponse>(okResult.Value, false);
        Assert.Equal(facility.Code, response.Code);
        Assert.Equal(facility.Meta!.EquipmentInfoComment, response.EquipmentInfoComment);
        Assert.Equal(facility.Meta!.RoomInfoComment, response.RoomInfoComment);
        Assert.Equal(facility.Meta!.PRPointComment, response.PrPointComment);
        Assert.Equal(facility.Meta!.AmenityInfoComment, response.AmenityInfoComment);
        Assert.Equal(facility.Meta!.LeisureInfoComment, response.LeisureInfoComment);
        Assert.Equal(facility.Meta!.FAQInfoComment, response.FaqInfoComment);
        Assert.Equal(facility.Meta!.OtherInfoComment, response.OtherInfoComment);
        Assert.Equal(facility.Meta!.MapUrl, response.MapUrl);

        Assert.NotNull(headers);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }

    [Fact]
    public async Task GetReservationChange_ReturnsCorrectResult()
    {
        // Arrange
        var facility = new Facility
        {
            Code = "Test",
            CanAddRoomOnModify = true
        };

        var mockMediator = new Mock<IMediator>();

        mockMediator
            .Setup(mediator => mediator.Send(It.IsAny<FacilityGetGroupDetailsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (
                    FacilityGetGroupDetailsQuery _,
                    CancellationToken _
                ) =>
                {
                    var acceptResponse = new FacilityDetailReservationChangeResponse
                    {
                        Code = "Test",
                        CanAddRoomOnModify = true
                    };

                    return (new HeaderDictionary(), acceptResponse);
                }
            );

        var mockMapper = MockServices.MockMapper();
        var controller = new FacilityEndpoint(mockMapper, mockMediator.Object);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetPaymentMethod(cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<FacilityDetailReservationChangeResponse>(okResult.Value, false);
        Assert.Equal(facility.Code, response.Code);
        Assert.Equal(facility.CanAddRoomOnModify, response.CanAddRoomOnModify);

        Assert.NotNull(headers);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }

    [Fact]
    public async Task GetReservationSetting_ReturnsCorrectResult()
    {
        // Arrange
        var facility = new Facility
        {
            Code = "Test",
            UseDailyPerson = true
        };

        var mockMediator = new Mock<IMediator>();

        mockMediator
            .Setup(mediator => mediator.Send(It.IsAny<FacilityGetGroupDetailsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (
                    FacilityGetGroupDetailsQuery _,
                    CancellationToken _
                ) =>
                {
                    var acceptResponse = new FacilityDetailReservationSettingResponse
                    {
                        Code = "Test",
                        UseDailyPerson = true
                    };

                    return (new HeaderDictionary(), acceptResponse);
                }
            );

        var mockMapper = MockServices.MockMapper();
        var controller = new FacilityEndpoint(mockMapper, mockMediator.Object);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetPaymentMethod(cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<FacilityDetailReservationSettingResponse>(okResult.Value, false);
        Assert.Equal(facility.Code, response.Code);
        Assert.Equal(facility.UseDailyPerson, response.UseDailyPerson);

        Assert.NotNull(headers);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }

    [Fact]
    public async Task UpdateAccess_ReturnsCorrectResult()
    {
        // Arrange
        var facility = new Facility
        {
            Id = 1,
            Code = "Test"
        };

        var mockMapper = MockServices.MockMapper();
        var controller = new FacilityEndpoint(mockMapper, MockMediator);
        var cancellationToken = CancellationToken.None;

        var request = new FacilityUpdateAccessRequest(
            111,
            111,
            "Comment",
            true,
            "Comment",
            true,
            "Comment",
            "Comment"
        );

        // Act
        var result = await controller.UpdateFacilityAccessAsync(request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);

        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Facility.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(facility.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UpdateAccept_ReturnsCorrectResult()
    {
        // Arrange
        var facility = new Facility
        {
            Id = 1,
            Code = "Test",
            BarrierFreeInfoComment = new MultilingualText { { TestUtil.DefaultLanguageCode, "Comment" } },
            Meta = new FacilityMeta
            {
                IsAcceptChildren = true,
                AcceptChildrenInfoComment = "Comment",
                IsAcceptPet = true,
                AcceptPetInfoComment = "Comment",
                IsBarrierFree = true
            }
        };

        var mockMapper = MockServices.MockMapper();
        var controller = new FacilityEndpoint(mockMapper, MockMediator);
        var cancellationToken = CancellationToken.None;

        var request = new FacilityUpdateAcceptRequest(
            true,
            "Comment",
            true,
            "Comment",
            true,
            "Comment"
        );

        // Act
        var result = await controller.UpdateFacilityAcceptanceAsync(request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);

        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Facility.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(facility.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UpdateBasicSetting_ReturnsCorrectResult()
    {
        // Arrange
        var facility = new Facility
        {
            Id = 1,
            Code = "Test",
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test Name" } },
            Kana = "Test Kana",
            Description = "Test Description",
            Postcode = "1111",
            Address1 = new MultilingualText { { TestUtil.DefaultLanguageCode, "Address 1" } },
            Address2 = new MultilingualText { { TestUtil.DefaultLanguageCode, "Address 2" } },
            Address3 = new MultilingualText { { TestUtil.DefaultLanguageCode, "Address 3" } },
            Address4 = new MultilingualText { { TestUtil.DefaultLanguageCode, "Address 4" } },
            Phone = "123456789",
            Fax = "123456789",
            Url = "https://test.com",
            AreaId = 1,
            CategoryId = 1,
            Meta = new FacilityMeta
            {
                RoomNumberWesternStyle = 1,
                RoomNumberJapaneseStyle = 1,
                RoomNumberJapaneseWesternStyle = 1,
                RoomNumberOtherStyle = 1
            }
        };

        var mockMapper = MockServices.MockMapper();
        var controller = new FacilityEndpoint(mockMapper, MockMediator);
        var cancellationToken = CancellationToken.None;

        var request = new FacilityUpdateBasicSettingRequest(
            "Test Description",
            "123456789",
            "https://test.com",
            "Test Name",
            "TestKana",
            "1111",
            "Address 1",
            "Address 2",
            "Address 3",
            "Address 4",
            "123456789",
            1,
            1,
            1,
            1,
            1,
            1,
            null,
            TimeSpan.FromHours(9),
            "Asia/Tokyo"
        );

        // Act
        var result = await controller.UpdateFacilityBasicSettingAsync(request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);

        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Facility.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(facility.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UpdateBath_ReturnsCorrectResult()
    {
        // Arrange
        var facility = new Facility
        {
            Id = 1,
            Code = "Test",
            Meta = new FacilityMeta
            {
                SpaType = "Type",
                SpaName = "Name",
                SpaDescription = "Description",
                SpaInfoComment = "Comment"
            }
        };

        var mockMapper = MockServices.MockMapper();
        var controller = new FacilityEndpoint(mockMapper, MockMediator);
        var cancellationToken = CancellationToken.None;

        var request = new FacilityUpdateBathRequest(
            "Type",
            "Name",
            "Comment",
            "Description"
        );

        // Act
        var result = await controller.UpdateFacilityBathAsync(request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);

        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Facility.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(facility.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UpdateClassification_ReturnsCorrectResult()
    {
        // Arrange
        var facility = new Facility
        {
            Id = 1,
            Code = "Test"
        };

        var mockMapper = MockServices.MockMapper();
        var controller = new FacilityEndpoint(mockMapper, MockMediator);
        var cancellationToken = CancellationToken.None;

        var request = new FacilityUpdateClassificationRequest(
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            []
        );

        // Act
        var result = await controller.UpdateFacilityClassificationAsync(request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);

        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Facility.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(facility.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UpdatePaymentMethod_ReturnsCorrectResult()
    {
        // Arrange
        var facility = new Facility
        {
            Id = 1,
            Code = "Test",
            IsOnLinePayment = true,
            IsOnSidePayment = true,
            CanOnLinePayment = true,
            Meta = new FacilityMeta
            {
                OnLinePaymentComment = "Comment",
                OnSidePaymentComment = "Comment",
                PaymentComment = "Comment"
            }
        };

        var mockMapper = MockServices.MockMapper();
        var controller = new FacilityEndpoint(mockMapper, MockMediator);
        var cancellationToken = CancellationToken.None;

        var request = new FacilityUpdatePaymentMethodRequest(
            true,
            true,
            "Comment",
            "Comment",
            "Comment"
        );

        // Act
        var result = await controller.UpdateFacilityPaymentMethodAsync(request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);

        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Facility.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(facility.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UpdatePublish_ReturnsCorrectResult()
    {
        // Arrange
        var facility = new Facility
        {
            Id = 1,
            Code = "Test",
            Heading1 = new MultilingualText { { TestUtil.DefaultLanguageCode, "Heading" } },
            Meta = new FacilityMeta
            {
                EquipmentInfoComment = "Comment",
                RoomInfoComment = "Comment",
                PRPointComment = "Comment",
                AmenityInfoComment = "Comment",
                LeisureInfoComment = "Comment",
                FAQInfoComment = "Comment",
                OtherInfoComment = "Comment",
                MapUrl = "Url"
            }
        };

        var mockMapper = MockServices.MockMapper();
        var controller = new FacilityEndpoint(mockMapper, MockMediator);
        var cancellationToken = CancellationToken.None;

        var request = new FacilityUpdatePublicationInformationRequest(
            "Heading",
            "Comment",
            "Comment",
            "Comment",
            "Comment",
            "Comment",
            "Comment",
            "Comment",
            "Url"
        );

        // Act
        var result = await controller.UpdateFacilityPublishingAsync(request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);

        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Facility.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(facility.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UpdateReservationChange_ReturnsCorrectResult()
    {
        // Arrange
        var facility = new Facility
        {
            Id = 1,
            Code = "Test",
            CanAddRoomOnModify = true
        };

        var mockMapper = MockServices.MockMapper();
        var controller = new FacilityEndpoint(mockMapper, MockMediator);
        var cancellationToken = CancellationToken.None;

        var request = new FacilityUpdateReservationChangeRequest(
            true,
            true
        );

        // Act
        var result = await controller.UpdateFacilityReservationChangeAsync(request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);

        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Facility.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(facility.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UpdateReservationSetting_ReturnsCorrectResult()
    {
        // Arrange
        var facility = new Facility
        {
            Id = 1,
            Code = "Test",
            UseDailyPerson = true
        };

        var mockMapper = MockServices.MockMapper();
        var controller = new FacilityEndpoint(mockMapper, MockMediator);
        var cancellationToken = CancellationToken.None;

        var request = new FacilityUpdateReservationSettingRequest
        (
            true
        );

        // Act
        var result = await controller.UpdateFacilityReservationSettingAsync(request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);

        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.Facility.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(facility.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }
}
