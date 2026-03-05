using System.Text;
using AutoMapper;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Site.Application.Auth;
using Liberty.Reservation.Site.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Site.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Site.Application.Exceptions;
using Liberty.Reservation.Site.Application.Models;
using Liberty.Reservation.Site.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Site.WebAPI.Application.UserCases.Commands.Booking;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;
using Liberty.UnitOfWork.Abstractions;
using MockQueryable;
using Moq;
using Newtonsoft.Json;
using SiteEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Site;

namespace Liberty.Reservation.Site.WebAPI.Test.UnitTests.Commands;

public class CheckChangedCommandHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldThrowException_WhenDataHasChanged()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var facilityRepositoryMock = new Mock<IFacilityRepository>();
        var planRepositoryMock = new Mock<IPlanRepository>();
        var roomGroupRepositoryMock = new Mock<IRoomGroupRepository>();
        var siteRepositoryMock = new Mock<ISiteRepository>();
        var optionItemRepositoryMock = new Mock<IOptionItemRepository>();
        var checkChangedServiceMock = new Mock<ICheckChangedService>();

        var lastUpdateObject = new LastUpdatedTimeOfBoookingModel
        {
            FacilityUpdatedAt = 20250101,
            RoomGroupUpdatedAt = 20250101,
            PlanUpdatedAt = 20250101,
            CancellationUpdatedAt = 20250101,
            SiteUpdatedAt = 20250101
        };

        var json = JsonConvert.SerializeObject(lastUpdateObject);
        var inputBytes = Encoding.UTF8.GetBytes(json);
        var lastUpdateString = Convert.ToBase64String(inputBytes);

        checkChangedServiceMock.Setup(x => x.GetLastUpdatedAtAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(lastUpdateObject);

        var facilityId = 1;
        var siteCode = "SITE123";
        var planId = 2;
        var roomGroupId = 3;

        var command = new CheckChangedCommand(1, 1) { Payload = new CheckChangedRequest(lastUpdateString, []) };

        securityContextAccessorMock.Setup(x => x.GetFacilityIdSelected()).Returns(facilityId);
        securityContextAccessorMock.Setup(x => x.GetSiteCodeSelected()).Returns(siteCode);

        facilityRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(
                new List<Facility>
                    {
                        new()
                        {
                            Id = facilityId,
                            FacilityPersonAgeTypes =
                            [
                                new() { PersonAgeType = new PersonAgeType { Id = 1 } }
                            ]
                        }
                    }.AsQueryable()
                    .BuildMock()
            );

        planRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(
                new List<Plan>
                    {
                        new()
                        {
                            Id = planId,
                            Cancellation = new Cancellation()
                        }
                    }.AsQueryable()
                    .BuildMock()
            );

        siteRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(new List<SiteEntity> { new() { Code = siteCode } }.AsQueryable().BuildMock());

        roomGroupRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(new List<RoomGroup> { new() { Id = roomGroupId } }.AsQueryable().BuildMock());

        optionItemRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(new List<OptionItem> { new() { Id = 1 } }.AsQueryable().BuildMock());

        var handler = new CheckChangedCommandHandler(
            unitOfWorkMock.Object,
            mapperMock.Object,
            optionItemRepositoryMock.Object,
            checkChangedServiceMock.Object
        );

        var res = await handler.Handle(command, CancellationToken.None);

        Assert.True(res);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowPlanHasChangesException()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var facilityRepositoryMock = new Mock<IFacilityRepository>();
        var planRepositoryMock = new Mock<IPlanRepository>();
        var roomGroupRepositoryMock = new Mock<IRoomGroupRepository>();
        var siteRepositoryMock = new Mock<ISiteRepository>();
        var optionItemRepositoryMock = new Mock<IOptionItemRepository>();
        var checkChangedServiceMock = new Mock<ICheckChangedService>();

        var lastUpdateObject = new LastUpdatedTimeOfBoookingModel
        {
            FacilityUpdatedAt = 20250101,
            RoomGroupUpdatedAt = 20250101,
            PlanUpdatedAt = 20250101,
            CancellationUpdatedAt = 20250101,
            SiteUpdatedAt = 20250101
        };

        var lastUpdatePayload = new LastUpdatedTimeOfBoookingModel
        {
            FacilityUpdatedAt = 20250101,
            RoomGroupUpdatedAt = 20250101,
            PlanUpdatedAt = 20250102,
            CancellationUpdatedAt = 20250101,
            SiteUpdatedAt = 20250101
        };

        var json = JsonConvert.SerializeObject(lastUpdatePayload);
        var inputBytes = Encoding.UTF8.GetBytes(json);
        var lastUpdateString = Convert.ToBase64String(inputBytes);

        checkChangedServiceMock.Setup(x => x.GetLastUpdatedAtAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(lastUpdateObject);

        var facilityId = 1;
        var siteCode = "SITE123";
        var planId = 2;
        var roomGroupId = 3;

        var command = new CheckChangedCommand(1, 1) { Payload = new CheckChangedRequest(lastUpdateString, []) };

        securityContextAccessorMock.Setup(x => x.GetFacilityIdSelected()).Returns(facilityId);
        securityContextAccessorMock.Setup(x => x.GetSiteCodeSelected()).Returns(siteCode);

        // InfrastructureOfTest repository mocks with different UpdatedAt values to simulate changes
        facilityRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(
                new List<Facility>
                    {
                        new()
                        {
                            Id = facilityId,
                            UpdatedAt = 20250101000000000,
                            FacilityPersonAgeTypes = new List<FacilityPersonAgeType>
                                {
                                    new() { PersonAgeType = new PersonAgeType { Id = 1 } }
                                }
                        }
                    }.AsQueryable()
                    .BuildMock()
            );

        planRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(
                new List<Plan>
                    {
                        new()
                        {
                            Id = planId,
                            UpdatedAt = 20250102000000000,
                            Cancellation = new Cancellation { UpdatedAt = 20250103000000000 }
                        } // Fake date 2
                    }.AsQueryable()
                    .BuildMock()
            );

        siteRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(
                new List<SiteEntity>
                    {
                        new()
                        {
                            Code = siteCode,
                            UpdatedAt = 20250104000000000
                        }
                    }.AsQueryable()
                    .BuildMock()
            );

        roomGroupRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(
                new List<RoomGroup>
                    {
                        new()
                        {
                            Id = roomGroupId,
                            UpdatedAt = 20250105000000000
                        }
                    }.AsQueryable()
                    .BuildMock()
            );

        optionItemRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(new List<OptionItem> { new() { Id = 1 } }.AsQueryable().BuildMock());

        // InfrastructureOfTest the handler
        var handler = new CheckChangedCommandHandler(
            unitOfWorkMock.Object,
            mapperMock.Object,
            optionItemRepositoryMock.Object,
            checkChangedServiceMock.Object
        );

        await Assert.ThrowsAsync<PlanHasChangesException>(
            () => handler.Handle(command, CancellationToken.None)
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowFacilityHasChangesException()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var facilityRepositoryMock = new Mock<IFacilityRepository>();
        var planRepositoryMock = new Mock<IPlanRepository>();
        var roomGroupRepositoryMock = new Mock<IRoomGroupRepository>();
        var siteRepositoryMock = new Mock<ISiteRepository>();
        var optionItemRepositoryMock = new Mock<IOptionItemRepository>();
        var checkChangedServiceMock = new Mock<ICheckChangedService>();

        var lastUpdateObject = new LastUpdatedTimeOfBoookingModel
        {
            FacilityUpdatedAt = 20250101,
            RoomGroupUpdatedAt = 20250101,
            PlanUpdatedAt = 20250101,
            CancellationUpdatedAt = 20250101,
            SiteUpdatedAt = 20250101
        };

        var lastUpdatePayload = new LastUpdatedTimeOfBoookingModel
        {
            FacilityUpdatedAt = 20250102,
            RoomGroupUpdatedAt = 20250101,
            PlanUpdatedAt = 20250101,
            CancellationUpdatedAt = 20250101,
            SiteUpdatedAt = 20250101
        };

        var json = JsonConvert.SerializeObject(lastUpdatePayload);
        var inputBytes = Encoding.UTF8.GetBytes(json);
        var lastUpdateString = Convert.ToBase64String(inputBytes);

        checkChangedServiceMock.Setup(x => x.GetLastUpdatedAtAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(lastUpdateObject);

        var facilityId = 1;
        var siteCode = "SITE123";
        var planId = 2;
        var roomGroupId = 3;

        var command = new CheckChangedCommand(1, 1) { Payload = new CheckChangedRequest(lastUpdateString, []) };

        securityContextAccessorMock.Setup(x => x.GetFacilityIdSelected()).Returns(facilityId);
        securityContextAccessorMock.Setup(x => x.GetSiteCodeSelected()).Returns(siteCode);

        // InfrastructureOfTest repository mocks with different UpdatedAt values to simulate changes
        facilityRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(
                new List<Facility>
                    {
                        new()
                        {
                            Id = facilityId,
                            UpdatedAt = 20250101000000000,
                            FacilityPersonAgeTypes = new List<FacilityPersonAgeType>
                                {
                                    new() { PersonAgeType = new PersonAgeType { Id = 1 } }
                                }
                        }
                    }.AsQueryable()
                    .BuildMock()
            );

        planRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(
                new List<Plan>
                    {
                        new()
                        {
                            Id = planId,
                            UpdatedAt = 20250102000000000,
                            Cancellation = new Cancellation { UpdatedAt = 20250103000000000 }
                        } // Fake date 2
                    }.AsQueryable()
                    .BuildMock()
            );

        siteRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(
                new List<SiteEntity>
                    {
                        new()
                        {
                            Code = siteCode,
                            UpdatedAt = 20250104000000000
                        }
                    }.AsQueryable()
                    .BuildMock()
            );

        roomGroupRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(
                new List<RoomGroup>
                    {
                        new()
                        {
                            Id = roomGroupId,
                            UpdatedAt = 20250105000000000
                        }
                    }.AsQueryable()
                    .BuildMock()
            );

        optionItemRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(new List<OptionItem> { new() { Id = 1 } }.AsQueryable().BuildMock());

        // InfrastructureOfTest the handler
        var handler = new CheckChangedCommandHandler(
            unitOfWorkMock.Object,
            mapperMock.Object,
            optionItemRepositoryMock.Object,
            checkChangedServiceMock.Object
        );

        await Assert.ThrowsAsync<FacilityHasChangesException>(
            () => handler.Handle(command, CancellationToken.None)
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowSiteHasChangesException()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var facilityRepositoryMock = new Mock<IFacilityRepository>();
        var planRepositoryMock = new Mock<IPlanRepository>();
        var roomGroupRepositoryMock = new Mock<IRoomGroupRepository>();
        var siteRepositoryMock = new Mock<ISiteRepository>();
        var optionItemRepositoryMock = new Mock<IOptionItemRepository>();
        var checkChangedServiceMock = new Mock<ICheckChangedService>();

        var lastUpdateObject = new LastUpdatedTimeOfBoookingModel
        {
            FacilityUpdatedAt = 20250101,
            RoomGroupUpdatedAt = 20250101,
            PlanUpdatedAt = 20250101,
            CancellationUpdatedAt = 20250101,
            SiteUpdatedAt = 20250101
        };

        var lastUpdatePayload = new LastUpdatedTimeOfBoookingModel
        {
            FacilityUpdatedAt = 20250101,
            RoomGroupUpdatedAt = 20250101,
            PlanUpdatedAt = 20250101,
            CancellationUpdatedAt = 20250101,
            SiteUpdatedAt = 20250102
        };

        var json = JsonConvert.SerializeObject(lastUpdatePayload);
        var inputBytes = Encoding.UTF8.GetBytes(json);
        var lastUpdateString = Convert.ToBase64String(inputBytes);

        checkChangedServiceMock.Setup(x => x.GetLastUpdatedAtAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(lastUpdateObject);

        var facilityId = 1;
        var siteCode = "SITE123";
        var planId = 2;
        var roomGroupId = 3;

        var command = new CheckChangedCommand(planId, roomGroupId)
        {
            Payload = new CheckChangedRequest(
                lastUpdateString,
                []
            )
        };

        securityContextAccessorMock.Setup(x => x.GetFacilityIdSelected()).Returns(facilityId);
        securityContextAccessorMock.Setup(x => x.GetSiteCodeSelected()).Returns(siteCode);

        facilityRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(
                new List<Facility>
                    {
                        new()
                        {
                            Id = facilityId,
                            UpdatedAt = 20250101000000000,
                            FacilityPersonAgeTypes = new List<FacilityPersonAgeType>
                                {
                                    new() { PersonAgeType = new PersonAgeType { Id = 1 } }
                                }
                        }
                    }.AsQueryable()
                    .BuildMock()
            );

        planRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(
                new List<Plan>
                    {
                        new()
                        {
                            Id = planId,
                            UpdatedAt = 20250102000000000,
                            Cancellation = new Cancellation { UpdatedAt = 20250104000000000 }
                        }
                    }.AsQueryable()
                    .BuildMock()
            );

        siteRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(
                new List<SiteEntity>
                    {
                        new()
                        {
                            Code = siteCode,
                            UpdatedAt = 20250107000000000
                        }
                    }.AsQueryable()
                    .BuildMock()
            );

        roomGroupRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(
                new List<RoomGroup>
                    {
                        new()
                        {
                            Id = roomGroupId,
                            UpdatedAt = 20250105000000000
                        }
                    }.AsQueryable()
                    .BuildMock()
            );

        optionItemRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(new List<OptionItem> { new() { Id = 1 } }.AsQueryable().BuildMock());

        var handler = new CheckChangedCommandHandler(
            unitOfWorkMock.Object,
            mapperMock.Object,
            optionItemRepositoryMock.Object,
            checkChangedServiceMock.Object
        );
        await Assert.ThrowsAsync<SiteHasChangesException>(
            () => handler.Handle(command, CancellationToken.None)
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowCancellationHasChangesException()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var facilityRepositoryMock = new Mock<IFacilityRepository>();
        var planRepositoryMock = new Mock<IPlanRepository>();
        var roomGroupRepositoryMock = new Mock<IRoomGroupRepository>();
        var siteRepositoryMock = new Mock<ISiteRepository>();
        var optionItemRepositoryMock = new Mock<IOptionItemRepository>();
        var checkChangedServiceMock = new Mock<ICheckChangedService>();

        var lastUpdateObject = new LastUpdatedTimeOfBoookingModel
        {
            FacilityUpdatedAt = 20250101,
            RoomGroupUpdatedAt = 20250101,
            PlanUpdatedAt = 20250101,
            CancellationUpdatedAt = 20250101,
            SiteUpdatedAt = 20250101
        };

        var lastUpdatePayload = new LastUpdatedTimeOfBoookingModel
        {
            FacilityUpdatedAt = 20250101,
            RoomGroupUpdatedAt = 20250101,
            PlanUpdatedAt = 20250101,
            CancellationUpdatedAt = 20250102,
            SiteUpdatedAt = 20250101
        };

        var json = JsonConvert.SerializeObject(lastUpdatePayload);
        var inputBytes = Encoding.UTF8.GetBytes(json);
        var lastUpdateString = Convert.ToBase64String(inputBytes);

        checkChangedServiceMock.Setup(x => x.GetLastUpdatedAtAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(lastUpdateObject);

        var facilityId = 1;
        var siteCode = "SITE123";
        var planId = 2;
        var roomGroupId = 3;

        var command = new CheckChangedCommand(planId, roomGroupId)
        {
            Payload = new CheckChangedRequest(
                lastUpdateString,
                [
                    new(1, "20250101")
                ]
            )
        };

        securityContextAccessorMock.Setup(x => x.GetFacilityIdSelected()).Returns(facilityId);
        securityContextAccessorMock.Setup(x => x.GetSiteCodeSelected()).Returns(siteCode);

        facilityRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(
                new List<Facility>
                    {
                        new()
                        {
                            Id = facilityId,
                            UpdatedAt = 20250101000000000,
                            FacilityPersonAgeTypes = new List<FacilityPersonAgeType>
                                {
                                    new() { PersonAgeType = new PersonAgeType { Id = 1 } }
                                }
                        }
                    }.AsQueryable()
                    .BuildMock()
            );

        planRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(
                new List<Plan>
                    {
                        new()
                        {
                            Id = planId,
                            UpdatedAt = 20250102000000000,
                            Cancellation = new Cancellation { UpdatedAt = 20250107000000000 }
                        }
                    }.AsQueryable()
                    .BuildMock()
            );

        siteRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(
                new List<SiteEntity>
                    {
                        new()
                        {
                            Code = siteCode,
                            UpdatedAt = 20250103000000000
                        }
                    }.AsQueryable()
                    .BuildMock()
            );

        roomGroupRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(
                new List<RoomGroup>
                    {
                        new()
                        {
                            Id = roomGroupId,
                            UpdatedAt = 20250105000000000
                        }
                    }.AsQueryable()
                    .BuildMock()
            );

        optionItemRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(
                new List<OptionItem>
                    {
                        new()
                        {
                            Id = 1,
                            UpdatedAt = 20250101
                        }
                    }.AsQueryable()
                    .BuildMock()
            );

        var handler = new CheckChangedCommandHandler(
            unitOfWorkMock.Object,
            mapperMock.Object,
            optionItemRepositoryMock.Object,
            checkChangedServiceMock.Object
        );
        await Assert.ThrowsAsync<CancellationHasChangesException>(
            () => handler.Handle(command, CancellationToken.None)
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowRoomGroupHasChangesException()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var facilityRepositoryMock = new Mock<IFacilityRepository>();
        var planRepositoryMock = new Mock<IPlanRepository>();
        var roomGroupRepositoryMock = new Mock<IRoomGroupRepository>();
        var siteRepositoryMock = new Mock<ISiteRepository>();
        var optionItemRepositoryMock = new Mock<IOptionItemRepository>();
        var checkChangedServiceMock = new Mock<ICheckChangedService>();

        var lastUpdateObject = new LastUpdatedTimeOfBoookingModel
        {
            FacilityUpdatedAt = 20250101,
            RoomGroupUpdatedAt = 20250101,
            PlanUpdatedAt = 20250101,
            CancellationUpdatedAt = 20250101,
            SiteUpdatedAt = 20250101
        };

        var lastUpdatePayload = new LastUpdatedTimeOfBoookingModel
        {
            FacilityUpdatedAt = 20250101,
            RoomGroupUpdatedAt = 20250102,
            PlanUpdatedAt = 20250101,
            CancellationUpdatedAt = 20250101,
            SiteUpdatedAt = 20250101
        };

        var json = JsonConvert.SerializeObject(lastUpdatePayload);
        var inputBytes = Encoding.UTF8.GetBytes(json);
        var lastUpdateString = Convert.ToBase64String(inputBytes);

        checkChangedServiceMock.Setup(x => x.GetLastUpdatedAtAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(lastUpdateObject);

        var facilityId = 1;
        var siteCode = "SITE123";
        var planId = 2;
        var roomGroupId = 3;

        var command = new CheckChangedCommand(planId, roomGroupId)
        {
            Payload = new CheckChangedRequest(
                lastUpdateString,
                []
            )
        };

        securityContextAccessorMock.Setup(x => x.GetFacilityIdSelected()).Returns(facilityId);
        securityContextAccessorMock.Setup(x => x.GetSiteCodeSelected()).Returns(siteCode);

        facilityRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(
                new List<Facility>
                    {
                        new()
                        {
                            Id = facilityId,
                            UpdatedAt = 20250101000000000,
                            FacilityPersonAgeTypes = new List<FacilityPersonAgeType>
                                {
                                    new() { PersonAgeType = new PersonAgeType { Id = 1 } }
                                }
                        }
                    }.AsQueryable()
                    .BuildMock()
            );

        planRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(
                new List<Plan>
                    {
                        new()
                        {
                            Id = planId,
                            UpdatedAt = 20250102000000000,
                            Cancellation = new Cancellation { UpdatedAt = 20250104000000000 }
                        }
                    }.AsQueryable()
                    .BuildMock()
            );

        siteRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(
                new List<SiteEntity>
                    {
                        new()
                        {
                            Code = siteCode,
                            UpdatedAt = 20250103000000000
                        }
                    }.AsQueryable()
                    .BuildMock()
            );

        roomGroupRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(
                new List<RoomGroup>
                    {
                        new()
                        {
                            Id = roomGroupId,
                            UpdatedAt = 20250107000000000
                        }
                    }.AsQueryable()
                    .BuildMock()
            );

        optionItemRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(new List<OptionItem> { new() { Id = 1 } }.AsQueryable().BuildMock());

        var handler = new CheckChangedCommandHandler(
            unitOfWorkMock.Object,
            mapperMock.Object,
            optionItemRepositoryMock.Object,
            checkChangedServiceMock.Object
        );

        await Assert.ThrowsAsync<RoomGroupHasChangesException>(
            () => handler.Handle(command, CancellationToken.None)
        );
    }
}
