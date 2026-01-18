using System.Net;
using Liberty.Entity.ValueObjects;
using Liberty.Entity.Utils;
using Liberty.Pagination;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroup;
using Liberty.Reservation.Manager.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests;

public class RoomGroupEndpointUnitTest : BaseUnitTest
{
    private IRoomGroupService MockRoomGroupService { get; set; } = null!;
    protected ISecurityContextAccessor MockSecurityContextAccessor { get; set; } = null!;

    protected override void InitData()
    {
        var mockMediator = new Mock<IMediator>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupCreateCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupUpdateBasicConfigurationCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupUpdatePublicationSettingCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupUpdateDisplaySettingCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupUpdateSaleCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupUpdatePaymentMethodCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupUpdateMealCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupUpdateOptionCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupUpdateCancelCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupUpdateSpecialCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupUpdateQuestionCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupUpdateImportantNoteCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupEnableCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupDeleteCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupGetAllQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    RoomGroupGetAllQuery _,
                    CancellationToken _
                ) =>
                {
                    var roomGroupResponse = new List<RoomGroupResponse>
                    {
                        new(
                            1,
                            "RG001",
                            "Deluxe Room",
                            "Luxury",
                            1,
                            4,
                            101,
                            35.5m,
                            RoomGroupSizeUnitTypes.M2,
                            1,
                            false,
                            true,
                            [
                                new(1, "IMG001", 0, true, "Description"),
                                new(2, "IMG002", 1, true, "Description")
                            ],
                            "Tag"
                        ),
                        new(
                            2,
                            "RG002",
                            "Deluxe Room",
                            "Luxury",
                            1,
                            4,
                            101,
                            35.5m,
                            RoomGroupSizeUnitTypes.M2,
                            1,
                            false,
                            true,
                            [
                                new(1, "IMG001", 0, true, "Description"),
                                new(2, "IMG002", 1, true, "Description")
                            ],
                            "Tag"
                        ),
                        new(
                            3,
                            "RG003",
                            "Deluxe Room",
                            "Luxury",
                            1,
                            4,
                            101,
                            35.5m,
                            RoomGroupSizeUnitTypes.M2,
                            1,
                            false,
                            true,
                            [
                                new(1, "IMG001", 0, true, "Description"),
                                new(2, "IMG002", 1, true, "Description")
                            ],
                            "Tag"
                        )
                    };

                    return (new HeaderDictionary(), roomGroupResponse);
                }
            );

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupGetBasicConfigurationQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    RoomGroupGetBasicConfigurationQuery _,
                    CancellationToken _
                ) =>
                {
                    var roomGroupDetailBasicConfigurationResponse = new RoomGroupDetailBasicConfigurationResponse(
                        1,
                        "Deluxe Room",
                        "Luxury",
                        "A luxurious deluxe room with all amenities.",
                        1,
                        4,
                        101,
                        35.5m,
                        RoomGroupSizeUnitTypes.M2,
                        [
                            new(1, "BT001", "King Size Bed", 1),
                            new(2, "BT002", "Single Bed", 2)
                        ],
                        false,
                        true,
                        true,
                        true,
                        true,
                        [
                            new(1, "FILE001", 0, true, "Description"),
                            new(2, "FILE002", 1, true, "Description")
                        ]
                    ) { Overview = "This room offers a comfortable stay with a beautiful view." };

                    return (new HeaderDictionary(), roomGroupDetailBasicConfigurationResponse);
                }
            );

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupGetPublicationSettingQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    RoomGroupGetPublicationSettingQuery _,
                    CancellationToken _
                ) =>
                {
                    var roomGroupDetailPublicationSettingResponse = new RoomGroupDetailPublicationSettingResponse
                    {
                        Id = 1,
                        Sites =
                        [
                            new(1, "Main Site"),
                            new(2, "Secondary Site")
                        ],
                        UseDisplayDate = true,
                        DisplayDateStart = AppDate.GetId(DateTime.UtcNow),
                        DisplayDateEnd = AppDate.GetId(DateTime.UtcNow),
                        UseAcceptDate = true,
                        AcceptDateStart = AppDate.GetId(DateTime.UtcNow),
                        AcceptDateEnd = AppDate.GetId(DateTime.UtcNow),
                        AcceptDays = 30,
                        AcceptMonths = 12,
                        AcceptEndLimitType = PlanAcceptEndLimitTypes.AfterDays, // Replace with actual enum value
                        ReceptionDayLimit = 7,
                        ReceptionLimit = new TimeSpan(18, 0, 0) // 6:00 PM
                    };

                    return (new HeaderDictionary(), roomGroupDetailPublicationSettingResponse);
                }
            );

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupGetDisplaySettingQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    RoomGroupGetDisplaySettingQuery _,
                    CancellationToken _
                ) =>
                {
                    var roomGroupDetailDisplaySettingResponse = new RoomGroupDetailDisplaySettingResponse
                    {
                        Id = 1,
                        RoomGroupMasterCategories =
                        [
                            new(1, "Master Category 1", CategoryTypes.RoomGroup),
                            new(2, "Master Category 2", CategoryTypes.RoomGroup)
                        ],
                        RoomGroupCategories =
                        [
                            new(3, "Category 1", CategoryTypes.RoomGroup),
                            new(4, "Category 2", CategoryTypes.RoomGroup)
                        ],
                        RoomGroupFeatureCategories =
                        [
                            new(5, "Feature 1", CategoryTypes.FacilityFeature),
                            new(6, "Feature 2", CategoryTypes.FacilityFeature)
                        ],
                        RoomGroupEquipmentCategories =
                        [
                            new(7, "Equipment 1", CategoryTypes.FacilityEquipment),
                            new(8, "Equipment 2", CategoryTypes.FacilityEquipment)
                        ],
                        RoomGroupAmenityCategories =
                        [
                            new(9, "Amenity 1", CategoryTypes.Amenity),
                            new(10, "Amenity 2", CategoryTypes.Amenity)
                        ],
                        Tags = ["Tag1", "Tag2", "Tag3"]
                    };

                    return (new HeaderDictionary(), roomGroupDetailDisplaySettingResponse);
                }
            );

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupGetAllPublishedInQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    RoomGroupGetAllPublishedInQuery _,
                    CancellationToken _
                ) =>
                {
                    var roomGroupDetailPublishInResponse = new List<SiteOfRoomGroupDetailPublicationSettingResponse>
                    {
                        new(
                            1,
                            "Main Site"
                        ),
                        new(
                            2,
                            "Secondary Site"
                        )
                    };

                    return (new HeaderDictionary(), roomGroupDetailPublishInResponse);
                }
            );

        MockSecurityContextAccessor = mockSecurityContextAccessor.Object;
        MockMediator = mockMediator.Object;

        var mockRoomGroupService = new Mock<IRoomGroupService>();

        mockRoomGroupService.Setup(
                x => x.ArrangeOrderAsync(
                    It.IsAny<List<long>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.CompletedTask);

        MockRoomGroupService = mockRoomGroupService.Object;
    }

    [Fact]
    public async Task CreateRoomGroup_ReturnCorrectResult()
    {
        var roomGroup = new RoomGroup
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "RoomGroup Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new RoomGroupsEndpoint(
            mockMapper,
            MockMediator,
            MockRoomGroupService,
            new MockCacheService(),
            MockSecurityContextAccessor,
            mockCacheManagementService.Object
        );
        var roomGroupToken = CancellationToken.None;

        var request = new RoomGroupCreateRequest(
            "Test 1",
            "RoomGroup Description",
            "Group Name",
            1,
            1,
            1
        );

        // Act
        var result = await controller.CreateRoomGroup(request, roomGroupToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<long>(okResult.Value, false);
        Assert.Equal(roomGroup.Id, response);

        Assert.NotNull(headers);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }

    [Fact]
    public async Task UpdateBasicSettingRoomGroup_ReturnsCorrectResult()
    {
        var roomGroup = new RoomGroup
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "RoomGroup Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new RoomGroupsEndpoint(
            mockMapper,
            MockMediator,
            MockRoomGroupService,
            new MockCacheService(),
            MockSecurityContextAccessor,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var request = new RoomGroupUpdateBasicConfigurationRequest(
            "Test 1",
            "Test 1",
            "Test 1",
            "RoomGroup Summary",
            "RoomGroup Description",
            1,
            1,
            1,
            1,
            RoomGroupSizeUnitTypes.M2,
            [],
            false,
            true,
            true,
            true,
            true,
            []
        );

        // Act
        var result = await controller.UpdateBasicConfigurationOfRoomGroup(roomGroup.Id, request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.RoomGroup.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(roomGroup.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UpdatePublicSettingRoomGroup_ReturnsCorrectResult()
    {
        var roomGroup = new RoomGroup
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "RoomGroup Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new RoomGroupsEndpoint(
            mockMapper,
            MockMediator,
            MockRoomGroupService,
            new MockCacheService(),
            MockSecurityContextAccessor,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var request = new RoomGroupUpdatePublicationSettingRequest(
            [1, 2],
            true,
            AppDate.GetId(DateTime.UtcNow),
            AppDate.GetId(DateTime.UtcNow),
            true,
            AppDate.GetId(DateTime.UtcNow),
            AppDate.GetId(DateTime.UtcNow),
            30,
            12,
            PlanAcceptEndLimitTypes.AfterMonths,
            7,
            new TimeSpan(18, 0, 0) // 6:00 PM
        ) { Id = 1 };

        // Act
        var result = await controller.UpdatePublicationSettingOfRoomGroup(roomGroup.Id, request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.RoomGroup.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(roomGroup.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UpdateDisplayRoomGroup_ReturnsCorrectResult()
    {
        var roomGroup = new RoomGroup
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "RoomGroup Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new RoomGroupsEndpoint(
            mockMapper,
            MockMediator,
            MockRoomGroupService,
            new MockCacheService(),
            MockSecurityContextAccessor,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var request = new RoomGroupUpdateDisplaySettingRequest(
            [1, 2],
            [3, 4],
            [5, 6],
            [7, 8],
            [9, 10],
            ["Tag1", "Tag2", "Tag3"]
        );

        // Act
        var result = await controller.UpdateDisplaySettingOfRoomGroup(roomGroup.Id, request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.RoomGroup.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(roomGroup.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task GetBasicSettingRoomGroup_ReturnsCorrectResult()
    {
        var roomGroup = new RoomGroup
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "RG1" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new RoomGroupsEndpoint(
            mockMapper,
            MockMediator,
            MockRoomGroupService,
            new MockCacheService(),
            MockSecurityContextAccessor,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetBasicConfigurationOfRoomGroup(roomGroup.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<RoomGroupDetailBasicConfigurationResponse>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(roomGroup.Id, response.Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetPublicationSettingRoomGroup_ReturnsCorrectResult()
    {
        var roomGroup = new RoomGroup
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "RG1" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new RoomGroupsEndpoint(
            mockMapper,
            MockMediator,
            MockRoomGroupService,
            new MockCacheService(),
            MockSecurityContextAccessor,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetPublicationSettingOfRoomGroup(roomGroup.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<RoomGroupDetailPublicationSettingResponse>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(roomGroup.Id, response.Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetDisplaySettingOfRoomGroup_ReturnsCorrectResult()
    {
        var roomGroup = new RoomGroup
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "RG1" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new RoomGroupsEndpoint(
            mockMapper,
            MockMediator,
            MockRoomGroupService,
            new MockCacheService(),
            MockSecurityContextAccessor,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetDisplaySettingOfRoomGroup(roomGroup.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<RoomGroupDetailDisplaySettingResponse>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(roomGroup.Id, response.Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetPublishedInRoomGroup_ReturnsCorrectResult()
    {
        var roomGroup = new RoomGroup
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "RoomGroup Description" } }
        };

        var sites = new List<Site>
        {
            new()
            {
                Id = 1,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Main Site" } }
            },
            new()
            {
                Id = 2,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Secondary Site" } }
            }
        };

        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new RoomGroupsEndpoint(
            mockMapper,
            MockMediator,
            MockRoomGroupService,
            new MockCacheService(),
            MockSecurityContextAccessor,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;
        var mockPagable = new Mock<IPageable>();

        // Act
        var result = await controller.GetAllPublishedInOfRoomGroup(mockPagable.Object, roomGroup.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<IEnumerable<SiteOfRoomGroupDetailPublicationSettingResponse>>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(sites.Count, response.Count());
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetSaleOfRoomGroup_ReturnsCorrectResult()
    {
        var roomGroup = new RoomGroup
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "RoomGroup Description" } }
        };

        var planForRomGroup = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Test 1" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Plan Description" } }
        };

        var mockMapper = MockServices.MockMapper();

        var mockMediator = new Mock<IMediator>();
        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupPlanGetGroupDetailsQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    RoomGroupPlanGetGroupDetailsQuery _,
                    CancellationToken _
                ) =>
                {
                    var basicSetting = new RoomGroupSaleResponse
                    {
                        Id = 1,
                        CheckInStart = new TimeSpan(14, 0, 0), // 2:00 PM
                        CheckInEnd = new TimeSpan(18, 0, 0), // 6:00 PM
                        CheckOut = "11:00 AM",
                        RoomNumberDaySaleLimit = 10,
                        GroupNumberDaySaleLimit = 5,
                        PlanDaySaleLimitType = PlanDaySaleLimitTypes.Pair, // Replace with actual enum value
                        UseDaySaleLimit = true,
                        UseAcceptPersonNumber = true,
                        AcceptPersonNumberMin = 1,
                        AcceptPersonNumberMax = 4,
                        NumberOfStayLimitMin = 1,
                        NumberOfStayLimitMax = 7,
                        PointRate = 0.05f,
                        PointExpire = 365,
                        MaxRoomNumberDaySaleLimit = 20
                    };

                    return (new HeaderDictionary(), basicSetting);
                }
            );
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new RoomGroupsEndpoint(
            mockMapper,
            mockMediator.Object,
            MockRoomGroupService,
            new MockCacheService(),
            MockSecurityContextAccessor,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetRoomGroupSale(roomGroup.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<RoomGroupSaleResponse>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(planForRomGroup.Id, response.Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetPaymentMethodOfRoomGroup_ReturnsCorrectResult()
    {
        var roomGroup = new RoomGroup
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "RoomGroup Description" } }
        };

        var planForRomGroup = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Name" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Description" } }
        };

        var mockMapper = MockServices.MockMapper();

        var mockMediator = new Mock<IMediator>();
        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupPlanGetGroupDetailsQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    RoomGroupPlanGetGroupDetailsQuery _,
                    CancellationToken _
                ) =>
                {
                    var basicSetting = new RoomGroupPaymentMethodResponse
                    {
                        Id = 1,
                        IsOnLinePayment = true,
                        IsOnSidePayment = true
                    };

                    return (new HeaderDictionary(), basicSetting);
                }
            );
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new RoomGroupsEndpoint(
            mockMapper,
            mockMediator.Object,
            MockRoomGroupService,
            new MockCacheService(),
            MockSecurityContextAccessor,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetRoomGroupPaymentMethod(roomGroup.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<RoomGroupPaymentMethodResponse>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(planForRomGroup.Id, response.Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetMealOfRoomGroup_ReturnsCorrectResult()
    {
        var roomGroup = new RoomGroup
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "RoomGroup Description" } }
        };

        var planForRomGroup = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Name" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Description" } }
        };

        var mockMapper = MockServices.MockMapper();

        var mockMediator = new Mock<IMediator>();

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupPlanGetGroupDetailsQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    RoomGroupPlanGetGroupDetailsQuery _,
                    CancellationToken _
                ) =>
                {
                    var basicSetting = new RoomGroupMealResponse
                    {
                        Id = 1,
                        MealTypes =
                        [
                            new(1, MealTypeEatTypes.Box),
                            new(2, MealTypeEatTypes.Room)
                        ]
                    };

                    return (new HeaderDictionary(), basicSetting);
                }
            );
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new RoomGroupsEndpoint(
            mockMapper,
            mockMediator.Object,
            MockRoomGroupService,
            new MockCacheService(),
            MockSecurityContextAccessor,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetRoomGroupMeal(roomGroup.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<RoomGroupMealResponse>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(planForRomGroup.Id, response.Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetOptionOfRoomGroup_ReturnsCorrectResult()
    {
        var roomGroup = new RoomGroup
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "RoomGroup Description" } }
        };

        var planForRomGroup = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Name" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Description" } }
        };

        var mockMapper = MockServices.MockMapper();

        var mockMediator = new Mock<IMediator>();

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupPlanGetGroupDetailsQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    RoomGroupPlanGetGroupDetailsQuery _,
                    CancellationToken _
                ) =>
                {
                    var basicSetting = new RoomGroupOptionResponse
                    {
                        Id = 1,
                        UseFixedOptionItem = true,
                        OptionItems = []
                    };

                    return (new HeaderDictionary(), basicSetting);
                }
            );
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new RoomGroupsEndpoint(
            mockMapper,
            mockMediator.Object,
            MockRoomGroupService,
            new MockCacheService(),
            MockSecurityContextAccessor,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetRoomGroupOption(roomGroup.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<RoomGroupOptionResponse>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(planForRomGroup.Id, response.Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetCancelOfRoomGroup_ReturnsCorrectResult()
    {
        var roomGroup = new RoomGroup
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "RoomGroup Description" } }
        };

        var planForRomGroup = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Name" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Description" } }
        };

        var mockMapper = MockServices.MockMapper();

        var mockMediator = new Mock<IMediator>();

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupPlanGetGroupDetailsQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    RoomGroupPlanGetGroupDetailsQuery _,
                    CancellationToken _
                ) =>
                {
                    var basicSetting = new RoomGroupCancelResponse
                    {
                        Id = 1,
                        IsCancelSameAccept = true,
                        CancelDayLimit = 7,
                        CancelLimit = new TimeSpan(18, 0, 0),
                        CancellationId = 1
                    };

                    return (new HeaderDictionary(), basicSetting);
                }
            );
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new RoomGroupsEndpoint(
            mockMapper,
            mockMediator.Object,
            MockRoomGroupService,
            new MockCacheService(),
            MockSecurityContextAccessor,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetRoomGroupCancel(roomGroup.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<RoomGroupCancelResponse>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(planForRomGroup.Id, response.Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetQuestionOfRoomGroup_ReturnsCorrectResult()
    {
        var roomGroup = new RoomGroup
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "RoomGroup Description" } }
        };

        var planForRomGroup = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Name" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Description" } }
        };

        var mockMapper = MockServices.MockMapper();

        var mockMediator = new Mock<IMediator>();

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupPlanGetGroupDetailsQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    RoomGroupPlanGetGroupDetailsQuery _,
                    CancellationToken _
                ) =>
                {
                    var basicSetting = new RoomGroupQuestionResponse
                    {
                        Id = 1,
                        Questions = []
                    };

                    return (new HeaderDictionary(), basicSetting);
                }
            );
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new RoomGroupsEndpoint(
            mockMapper,
            mockMediator.Object,
            MockRoomGroupService,
            new MockCacheService(),
            MockSecurityContextAccessor,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetRoomGroupQuestion(roomGroup.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<RoomGroupQuestionResponse>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(planForRomGroup.Id, response.Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetSpecialOfRoomGroup_ReturnsCorrectResult()
    {
        var roomGroup = new RoomGroup
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "RoomGroup Description" } }
        };

        var planForRomGroup = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Name" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Description" } }
        };

        var mockMapper = MockServices.MockMapper();

        var mockMediator = new Mock<IMediator>();

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupPlanGetGroupDetailsQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    RoomGroupPlanGetGroupDetailsQuery _,
                    CancellationToken _
                ) =>
                {
                    var basicSetting = new RoomGroupSpecialResponse
                    {
                        Id = 1,
                        IsSecret = true,
                        SecretWord = "Secret"
                    };

                    return (new HeaderDictionary(), basicSetting);
                }
            );
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new RoomGroupsEndpoint(
            mockMapper,
            mockMediator.Object,
            MockRoomGroupService,
            new MockCacheService(),
            MockSecurityContextAccessor,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetRoomGroupOption(roomGroup.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<RoomGroupSpecialResponse>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(planForRomGroup.Id, response.Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetImportantNoteOfRoomGroup_ReturnsCorrectResult()
    {
        var roomGroup = new RoomGroup
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "RoomGroup Description" } }
        };

        var planForRomGroup = new Plan
        {
            Id = 1,
            Name = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Name" } },
            Description = new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), "Description" } }
        };

        var mockMapper = MockServices.MockMapper();

        var mockMediator = new Mock<IMediator>();

        mockMediator.Setup(
                x => x.Send(
                    It.IsAny<RoomGroupPlanGetGroupDetailsQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    RoomGroupPlanGetGroupDetailsQuery _,
                    CancellationToken _
                ) =>
                {
                    var basicSetting = new RoomGroupImportantNoteResponse
                    {
                        Id = 1,
                        Payment = "Payment",
                        Meal = "Meal",
                        Other = "Other"
                    };

                    return (new HeaderDictionary(), basicSetting);
                }
            );
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new RoomGroupsEndpoint(
            mockMapper,
            mockMediator.Object,
            MockRoomGroupService,
            new MockCacheService(),
            MockSecurityContextAccessor,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetRoomGroupOption(roomGroup.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<RoomGroupImportantNoteResponse>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(planForRomGroup.Id, response.Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task EnableRoomGroup_ReturnsCorrectResult()
    {
        var roomGroup = new RoomGroup
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "RoomGroup Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new RoomGroupsEndpoint(
            mockMapper,
            MockMediator,
            MockRoomGroupService,
            new MockCacheService(),
            MockSecurityContextAccessor,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var request = new RoomGroupEnabledRequest(
            true
        );

        // Act
        var result = await controller.EnableRoomGroup(roomGroup.Id, request, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.RoomGroup.Updated", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(roomGroup.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task DeleteRoomGroup_ReturnsCorrectResult()
    {
        var roomGroup = new RoomGroup
        {
            Id = 1,
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test 1" } },
            Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "RoomGroup Description" } }
        };
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new RoomGroupsEndpoint(
            mockMapper,
            MockMediator,
            MockRoomGroupService,
            new MockCacheService(),
            MockSecurityContextAccessor,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.DeleteRoomGroup(roomGroup.Id, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.RoomGroup.Deleted", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(roomGroup.Id.ToString(), headers["X-Liberty-params"]);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task GetAllRoomGroup_ReturnsCorrectResult()
    {
        var roomGroups = new List<RoomGroup>
        {
            new()
            {
                Id = 1,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "RG1" } }
            },
            new()
            {
                Id = 2,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "RG2" } }
            },
            new()
            {
                Id = 3,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "RG3" } }
            }
        };

        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new RoomGroupsEndpoint(
            mockMapper,
            MockMediator,
            MockRoomGroupService,
            new MockCacheService(),
            MockSecurityContextAccessor,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        var mockPagable = new Mock<IPageable>();

        // Act
        var result = await controller.GetAllRoomGroups(mockPagable.Object, cancellationToken);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var response = Assert.IsType<List<RoomGroupResponse>>(okObjectResult.Value, false);
        Assert.NotNull(headers);
        Assert.Equal(roomGroups.Count, response.Count);
        Assert.Equal(roomGroups[0].Id, response[0].Id);
        Assert.Equal(roomGroups[1].Id, response[1].Id);
        Assert.Equal(roomGroups[2].Id, response[2].Id);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task ArrangeOrderOfRoomGroups_ReturnsCorrectResult()
    {
        var request = new ItemUpdateOrderRequest(
            [
                1,
                2,
                3
            ]
        );
        var mockMapper = MockServices.MockMapper();
        var mockCacheManagementService = new Mock<ICacheManagementService>();
        var controller = new RoomGroupsEndpoint(
            mockMapper,
            MockMediator,
            MockRoomGroupService,
            new MockCacheService(),
            MockSecurityContextAccessor,
            mockCacheManagementService.Object
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.ArrangeOrderOfRoomGroups(request, cancellationToken);
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result, false);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }
}
