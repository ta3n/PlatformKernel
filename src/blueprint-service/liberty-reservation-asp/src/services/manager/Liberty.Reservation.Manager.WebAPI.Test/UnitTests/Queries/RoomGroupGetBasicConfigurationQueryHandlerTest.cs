using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroup;
using Liberty.Reservation.Manager.Application.Auth;
using Moq;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using MockQueryable;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class RoomGroupGetBasicConfigurationQueryHandlerTest
{
    private readonly IMapper _mapper;

    public RoomGroupGetBasicConfigurationQueryHandlerTest()
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<MultilingualText, string>().ConvertUsing(src => src == null ? string.Empty : src.GetValueByHeader());
                cfg.CreateMap<RoomGroup, RoomGroupDetailBasicConfigurationResponse>()
                    .ConstructUsing(
                        src => new RoomGroupDetailBasicConfigurationResponse(
                            src.Id,
                            src.Name!.GetValueByHeader(),
                            src.GroupName ?? string.Empty,
                            src.Description != null ? src.Description.GetValueByHeader() : null,
                            src.CapacityMin ?? 0,
                            src.CapacityMax ?? 0,
                            src.BaseNumber,
                            src.Size,
                            src.RoomGroupSizeUnitType,
                            src.RoomGroupBedTypes!.Select(
                                    x => new BedTypeOfRoomGroupUpdateBasicConfigurationResponse(
                                        x.BedTypeId,
                                        x.BedType!.Code ?? string.Empty,
                                        x.BedType!.Name ?? string.Empty,
                                        x.Number ?? 0
                                    )
                                )
                                .ToArray(),
                            src.IsEnabledSmoking,
                            src.IsDescriptionVisible,
                            src.IsOverviewVisible,
                            src.IsRoomSizeVisible,
                            src.IsBedTypeVisible,
                            src.FileRoomGroups!.Select(
                                    x => new FileOfRoomBasicConfigurationResponse(
                                        x.FileId,
                                        x.File!.Code ?? string.Empty,
                                        x.Index,
                                        x.IsEnabled,
                                        "Description"
                                    )
                                )
                                .ToArray()
                        )
                    );
            }
        );
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnRoomGroupDetail_WhenQueryValid()
    {
        var facilityId = 100;
        var roomGroupId = 1;
        var expectedResponse = new RoomGroupDetailBasicConfigurationResponse(
            1,
            "Name",
            "des",
            "des",
            2,
            1,
            1,
            1,
            RoomGroupSizeUnitTypes.JYO,
            [],
            true,
            true,
            true,
            true,
            true,
            []
        )
        {
            Overview = null,
            Summary = null
        };

        // Mocks
        var mockMapper = new Mock<IMapper>();
        var mockCacheService = new Mock<ICacheService>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockRoomGroupRepository = new Mock<IRoomGroupRepository>();

        mockSecurityContextAccessor.Setup(x => x.FacilityKey).Returns(facilityId);
        mockMapper.Setup(m => m.Map<RoomGroupDetailBasicConfigurationResponse>(It.IsAny<RoomGroup>()))
            .Returns(expectedResponse);

        var roomGroups = new List<RoomGroup>
        {
            new()
            {
                Id = 1,
                Code = "Code1",
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Room A" } },
                Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Room A" } },
                GroupName = "Group1",
                CapacityMin = 2,
                CapacityMax = 4,
                BaseNumber = 10,
                Size = 30.5f,
                RoomGroupSizeUnitType = RoomGroupSizeUnitTypes.JYO,
                DisplayOrder = 1,
                IsEnabledSmoking = true,
                IsEnabled = true,
                Tag = "Tag1",
                FacilityRoomGroups = [new() { FacilityId = facilityId }],
                FileRoomGroups = new List<FileRoomGroup>
                {
                    new()
                    {
                        FileId = 101,
                        File = new() { Code = "File1" },
                        Index = 1
                    },
                    new()
                    {
                        FileId = 102,
                        File = new() { Code = "File2" },
                        Index = 2
                    }
                },
                RoomGroupBedTypes =
                [
                    new()
                    {
                        BedTypeId = 1,
                        Number = 1,
                        BedType = new()
                        {
                            Code = "Code",
                            Name = "Name"
                        }
                    }
                ]
            }
        };
        mockRoomGroupRepository.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(roomGroups.AsQueryable().BuildMock());

        var query = new RoomGroupGetBasicConfigurationQuery(roomGroupId);
        var handler = new RoomGroupGetBasicConfigurationQueryHandler(
            _mapper,
            mockCacheService.Object,
            mockSecurityContextAccessor.Object,
            mockRoomGroupRepository.Object
        );

        // Act
        var (_, result) = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }
}
