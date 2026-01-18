using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Site.Application.Auth;
using Liberty.Reservation.Site.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Site.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Booking;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest.Utilities;
using MockQueryable;
using Moq;
using SiteEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Site;

namespace Liberty.Reservation.Site.WebAPI.Test.UnitTests.Queries;

public class BookingGetFacilityQueryHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnBookingFacilityResponse_WhenFacilityIsValid()
    {
        var facilityId = 1;
        var siteId = 1;

        var facilityRepositoryMock = new Mock<IFacilityRepository>();
        var facilityRoomGroupRepositoryMock = new Mock<IFacilityRoomGroupRepository>();

        var siteRepositoryMock = new Mock<ISiteRepository>();
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var mapperMock = new Mock<IMapper>();
        var cacheServiceMock = new Mock<ICacheService>();

        securityContextAccessorMock.Setup(x => x.GetFacilityIdSelected()).Returns(facilityId);
        securityContextAccessorMock.Setup(x => x.GetSiteIdSelected()).Returns(siteId);

        var mockSite = new SiteEntity
        {
            Id = siteId,
            Code = "test_site_code",
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test Site" } },
            IsEnabled = true,
            FacilitySites = [new() { FacilityId = facilityId }]
        };

        var mockSiteQuery = new List<SiteEntity> { mockSite }.AsQueryable().BuildMock();

        siteRepositoryMock
            .Setup(repo => repo.GetQueryableWithAsNoTracking())
            .Returns(mockSiteQuery);

        var facilities = new List<Facility>
        {
            new()
            {
                Code = "Test",
                Id = facilityId,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test Facility" } },
                CanAddRoomOnModify = true,
                CanOnLinePayment = true,
                IsOnLinePayment = true,
                IsEnabled = true,
                FacilitySites =
                [
                    new()
                    {
                        SiteId = siteId,
                        Site = new()
                        {
                            Code = "test",
                            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Site" } }
                        }
                    }
                ],
                FacilityPersonAgeTypes =
                    new List<FacilityPersonAgeType>
                    {
                        new()
                        {
                            PersonAgeType = new PersonAgeType
                            {
                                Id = 1,
                                IsMain = true,
                                AgeMin = 10,
                                AgeMax = 50,
                                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Adult" } },
                                UpdatedAt = AppDate.GetId(DateTime.UtcNow)
                            }
                        }
                    },
                Meta = new FacilityMeta
                {
                    IsBarrierFree = true,
                    SystemEMail = "test@example.com"
                },
                Heading1 = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test Heading" } },
                BarrierFreeInfoComment = new MultilingualText { { TestUtil.DefaultLanguageCode, "Accessible" } },
                UseDailyPerson = true,
                Address1 = new MultilingualText { { TestUtil.DefaultLanguageCode, "123 Test St" } },
                Address2 = new MultilingualText { { TestUtil.DefaultLanguageCode, "Suite 4" } },
                Address3 = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test City" } },
                Address4 = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test Country" } },
                Postcode = "12345",
                Url = "https://testfacility.com",
                FacilityFiles =
                [
                    new()
                    {
                        File = new()
                        {
                            Code = "testfile",
                            ContentType = "application/pdf"
                        }
                    }
                ]
            }
        };

        var mockFacilityQuery = facilities.AsQueryable().BuildMock();

        var mockFacilityRoomGroups = new List<FacilityRoomGroup>
            {
                new()
                {
                    FacilityId = facilityId,
                    RoomGroup = new RoomGroup
                    {
                        IsEnabled = true,
                        CapacityMax = 4
                    }
                },
                new()
                {
                    FacilityId = facilityId,
                    RoomGroup = new RoomGroup
                    {
                        IsEnabled = true,
                        CapacityMax = 8
                    }
                },
                new()
                {
                    FacilityId = 999,
                    RoomGroup = new RoomGroup
                    {
                        IsEnabled = true,
                        CapacityMax = 999
                    }
                },
                new()
                {
                    FacilityId = facilityId,
                    RoomGroup = new RoomGroup
                    {
                        IsEnabled = false,
                        CapacityMax = 100
                    }
                }
            }
            .AsQueryable()
            .BuildMock();

        facilityRepositoryMock
            .Setup(repo => repo.GetQueryableWithAsNoTracking())
            .Returns(mockFacilityQuery);

        facilityRoomGroupRepositoryMock
            .Setup(repo => repo.GetQueryableWithAsNoTracking())
            .Returns(mockFacilityRoomGroups);

        mapperMock
            .Setup(m => m.ConfigurationProvider)
            .Returns(
                new MapperConfiguration(
                    cfg =>
                    {
                        cfg.CreateMap<MultilingualText, string>().ConvertUsing(src => src == null ? string.Empty : src.GetValueByHeader());
                        cfg.CreateMap<Facility, BookingFacilityResponse>()
                            .ForMember(
                                dest => dest.Id,
                                opt => opt.MapFrom(
                                    src => src.Id
                                )
                            )
                            .ForMember(
                                dest => dest.Code,
                                opt => opt.MapFrom(
                                    src => src.Code
                                )
                            )
                            .ForMember(
                                dest => dest.Name,
                                opt => opt.MapFrom(
                                    src => src.Name!.GetValueByHeader()
                                )
                            )
                            .ForMember(
                                dest => dest.IsOnLinePayment,
                                opt => opt.MapFrom(
                                    src => src.CanOnLinePayment && src.IsOnLinePayment
                                )
                            )
                            .ForMember(
                                dest => dest.IsOnSidePayment,
                                opt => opt.MapFrom(
                                    src => src.IsOnSidePayment
                                )
                            )
                            .ForMember(
                                dest => dest.Site,
                                opt => opt.MapFrom(
                                    src => src.FacilitySites != null && src.FacilitySites.Count != 0
                                        ? new SiteOfBookingFacilityResponse(
                                            src.FacilitySites.First().Site!.Code,
                                            src.FacilitySites.First().Site!.Name!.GetValueByHeader()
                                        )
                                        : null
                                )
                            )
                            .ForMember(
                                dest => dest.PersonAgeTypes,
                                opt =>
                                    opt.MapFrom(
                                        src => src.FacilityPersonAgeTypes!.Where(x => x.PersonAgeType!.IsEnabled)
                                            .Select(
                                                x => new PersonAgeTypeOfBookingFacilityResponse(
                                                    x.PersonAgeTypeId,
                                                    x.PersonAgeType!.IsMain,
                                                    x.PersonAgeType!.AgeMin,
                                                    x.PersonAgeType!.AgeMax,
                                                    x.PersonAgeType!.Name!.GetValueByHeader(),
                                                    x.PersonAgeType!.UpdatedAt
                                                )
                                            )
                                            .ToList()
                                    )
                            )
                            .ForMember(
                                dest => dest.Heading,
                                opt => opt.MapFrom(
                                    src => src.Heading1!.GetValueByHeader()
                                )
                            )
                            .ForMember(
                                dest => dest.IsBarrierFree,
                                opt => opt.MapFrom(
                                    src => src.Meta!.IsBarrierFree
                                )
                            )
                            .ForMember(
                                dest => dest.BarrierFreeInfoComment,
                                opt => opt.MapFrom(
                                    src => src.BarrierFreeInfoComment!.GetValueByHeader()
                                )
                            )
                            .ForMember(
                                dest => dest.UseDailyPerson,
                                opt => opt.MapFrom(
                                    src => src.UseDailyPerson
                                )
                            )
                            .ForMember(
                                dest => dest.CanAddRoomOnModify,
                                opt => opt.MapFrom(
                                    src => src.CanAddRoomOnModify
                                )
                            )
                            .ForMember(
                                dest => dest.Address1,
                                opt => opt.MapFrom(
                                    src => src.Address1
                                )
                            )
                            .ForMember(
                                dest => dest.Address2,
                                opt => opt.MapFrom(
                                    src => src.Address2
                                )
                            )
                            .ForMember(
                                dest => dest.Address3,
                                opt => opt.MapFrom(
                                    src => src.Address3
                                )
                            )
                            .ForMember(
                                dest => dest.Address4,
                                opt => opt.MapFrom(
                                    src => src.Address4
                                )
                            )
                            .ForMember(
                                dest => dest.Postcode,
                                opt => opt.MapFrom(
                                    src => src.Postcode
                                )
                            )
                            .ForMember(
                                dest => dest.Email,
                                opt => opt.MapFrom(
                                    src => src.Meta!.SystemEMail
                                )
                            )
                            .ForMember(
                                dest => dest.Url,
                                opt => opt.MapFrom(
                                    src => src.Url
                                )
                            )
                            .ForMember(
                                dest => dest.File,
                                opt => opt.MapFrom(
                                    src => src.FacilityFiles != null && src.FacilityFiles.Count != 0
                                        ? new FileOfBookingResponse(
                                            src.FacilityFiles.First().File!.Code,
                                            src.FacilityFiles.First().File!.ContentType
                                        )
                                        : null
                                )
                            );
                    }
                )
            );

        cacheServiceMock
            .Setup(x => x.GetStringAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(string.Empty);

        var query = new BookingGetFacilityQuery();
        var handler = new BookingGetFacilityQueryHandler(
            mapperMock.Object,
            cacheServiceMock.Object,
            securityContextAccessorMock.Object,
            facilityRepositoryMock.Object,
            siteRepositoryMock.Object,
            facilityRoomGroupRepositoryMock.Object
        );

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsType<BookingFacilityResponse>(result.Item2);
        Assert.Equal(facilityId, result.Item2.Id);
        Assert.Equal("Test Facility", result.Item2.Name);
        Assert.Equal("test_site_code", result.Item2.Site?.Code);
    }
}
