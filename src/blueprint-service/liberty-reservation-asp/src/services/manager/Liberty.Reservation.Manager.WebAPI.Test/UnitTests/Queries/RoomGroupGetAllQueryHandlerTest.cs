using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroup;
using Liberty.Reservation.Manager.Application.Auth;
using Moq;
using Liberty.Pagination;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using MockQueryable;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class RoomGroupGetAllQueryHandlerTest
{
    private readonly IMapper _mapper;

    public RoomGroupGetAllQueryHandlerTest()
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<RoomGroup, RoomGroupResponse>()
                    .ConstructUsing(
                        src => new RoomGroupResponse(
                            src.Id,
                            src.Code ?? string.Empty,
                            src.Name!.GetValueByHeader(),
                            src.GroupName,
                            src.CapacityMin ?? 0,
                            src.CapacityMax ?? 0,
                            src.BaseNumber,
                            src.Size ?? 0,
                            src.RoomGroupSizeUnitType,
                            src.DisplayOrder,
                            src.IsEnabledSmoking,
                            src.IsEnabled,
                            src.FileRoomGroups!.Select(
                                    x => new ImageOfRoomGroupDetailMediaSettingResponse(
                                        x.FileId,
                                        x.File!.Code ?? string.Empty,
                                        x.Index,
                                        true,
                                        "Description"
                                    )
                                )
                                .ToArray(),
                            src.Tag
                        )
                    );
            }
        );
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPagedRoomGroups_WhenQueryValid()
    {
        var facilityId = 100;
        var pageable = PageableBinderConfig.DefaultPageable;

        // Mocks
        var mockCacheService = new Mock<ICacheService>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockRoomGroupRepository = new Mock<IRoomGroupRepository>();

        mockSecurityContextAccessor.Setup(x => x.FacilityKey).Returns(facilityId);
        var roomGroups = new List<RoomGroup>
        {
            new()
            {
                Id = 1,
                Code = "Code1",
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Room A" } },
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
                }
            }
        };
        mockRoomGroupRepository.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(roomGroups.AsQueryable().BuildMock());

        var query = new RoomGroupGetAllQuery(pageable);
        var handler = new RoomGroupGetAllQueryHandler(
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
