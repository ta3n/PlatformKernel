using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Entity.ValueObjects;
using Liberty.Pagination;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Site.Application.Auth;
using Liberty.Reservation.Site.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Site.Application.Exceptions;
using Liberty.Reservation.Site.WebAPI.Application.ExternalServices.Membership.Facility.Repositories;
using Liberty.Reservation.Site.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Site.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Booking;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest.Utilities;
using MockQueryable;
using Moq;

namespace Liberty.Reservation.Site.WebAPI.Test.UnitTests.Queries;

public class BookingGetAllOptionItemsQueryHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnOptionItems_WhenAllValid()
    {
        // Arrange
        const string facilityCode = "FAC123";
        const int planId = 1;
        var appDateId = AppDate.GetId(DateTime.UtcNow);

        var securityContextMock = new Mock<ISecurityContextAccessor>();
        var facilityExternalRepositoryMock = new Mock<IFacilityExternalRepository>();
        var optionItemRepositoryMock = new Mock<IOptionItemRepository>();
        var mapperMock = new Mock<IMapper>();
        var cacheServiceMock = new Mock<ICacheService>();

        securityContextMock.Setup(x => x.GetFacilityCodeSelected()).Returns(facilityCode);

        facilityExternalRepositoryMock
            .Setup(x => x.CheckFacilityAvailableAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var data = new List<OptionItem>
            {
                new()
                {
                    Id = 1,
                    Price = 100,
                    IsEnabled = true,
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test Option" } },
                    Description = "Test Description",
                    PlanOptionItems = new List<PlanOptionItem>
                    {
                        new()
                        {
                            PlanId = planId,
                            Plan = new Plan { UseFixedOptionItem = true }
                        }
                    },
                    OptionItemAppDates = new List<OptionItemAppDate>
                    {
                        new()
                        {
                            AppDateId = appDateId,
                            IsNotSelled = false,
                            SellNumber = 10,
                            ReservationRoomGroupAppDateOptionItems = new List<ReservationRoomGroupAppDateOptionItem>
                            {
                                new()
                                {
                                    ReservationId = 1,
                                    Reservation = new()
                                    {
                                        Id = 1,
                                        ReservationState = ReservationStatus.Reserved
                                    },
                                    Number = 1
                                }
                            }
                        }
                    },
                    FileOptionItems = new List<FileOptionItem>
                    {
                        new()
                        {
                            File = new()
                            {
                                Code = "FILE456",
                                ContentType = "application/pdf"
                            }
                        }
                    },
                    OptionItemQuestions = new List<OptionItemQuestion>
                    {
                        new()
                        {
                            Question = new Question
                            {
                                Id = 102,
                                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Sample Question 2" } },
                                Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Another test question." } },
                                FormData = new MultilingualText { { TestUtil.DefaultLanguageCode, "{}" } },
                                QuestionType = QuestionTypes.Unknown
                            }
                        }
                    },
                    ReservationRoomGroupAppDateOptionItems = new List<ReservationRoomGroupAppDateOptionItem>
                    {
                        new()
                        {
                            ReservationId = 2,
                            Reservation = new()
                            {
                                Id = 2,
                                ReservationState = ReservationStatus.Confirmed
                            },
                            Number = 2
                        }
                    }
                }
            }
            .AsQueryable()
            .BuildMock();

        optionItemRepositoryMock
            .Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(data);

        var payload = new BookingOptionRequest(appDateId);
        var pageable = PageableBinderConfig.DefaultPageable;
        var query = new BookingGetAllOptionItemsQuery(planId, payload, pageable)
        {
            PlanId = planId,
            Payload = payload,
            Pageable = pageable
        };

        mapperMock
            .Setup(m => m.ConfigurationProvider)
            .Returns(
                new MapperConfiguration(
                    cfg =>
                    {
                        cfg.CreateMap<MultilingualText, string>().ConvertUsing(src => src == null ? string.Empty : src.GetValueByHeader());
                        cfg.CreateMap<OptionItem, OptionItemOfBookingResponse>()
                            .ForMember(
                                dest => dest.Id,
                                opt => opt.MapFrom(
                                    src => src.Id
                                )
                            )
                            .ForMember(
                                dest => dest.Name,
                                opt => opt.MapFrom(
                                    src => src.Name!.GetValueByHeader()
                                )
                            )
                            .ForMember(
                                dest => dest.Description,
                                opt => opt.MapFrom(
                                    src => src.Description
                                )
                            )
                            .ForMember(
                                dest => dest.SellNumber,
                                opt => opt.MapFrom(
                                    src => src.OptionItemAppDates != null && src.OptionItemAppDates.Count > 0
                                        ? src.OptionItemAppDates.First().SellNumber
                                        : 0
                                )
                            )
                            .ForMember(
                                dest => dest.ReservedNumber,
                                opt => opt.MapFrom(
                                    src => src.ReservationRoomGroupAppDateOptionItems!
                                        .Where(
                                            x =>
                                                x.Reservation!.ReservationState == ReservationStatus.Confirmed
                                                || x.Reservation!.ReservationState == ReservationStatus.Reserved
                                                || x.Reservation!.ReservationState == ReservationStatus.Modified
                                        )
                                        .Sum(x => x.Number)
                                )
                            )
                            .ForMember(
                                dest => dest.Price,
                                opt => opt.MapFrom(
                                    src => src.Price
                                )
                            );
                    }
                ) { }
            );

        var handler = new BookingGetAllOptionItemsQueryHandler(
            mapperMock.Object,
            cacheServiceMock.Object,
            securityContextMock.Object,
            optionItemRepositoryMock.Object,
            facilityExternalRepositoryMock.Object
        );
        var result = await handler.Handle(query, CancellationToken.None);
        var optionItems = Assert.IsType<IEnumerable<OptionItemOfBookingResponse>>(result.Item2, false).ToList();

        Assert.NotNull(optionItems);
        var firstOptionItem = optionItems[0];
        Assert.Equal(100, firstOptionItem.Price);
        Assert.Equal("Test Option", firstOptionItem.Name);
        Assert.Equal("Test Description", firstOptionItem.Description);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFacilityNotfoundException()
    {
        // Arrange
        const string facilityCode = "FAC123";
        const int appDateId = 1;
        const int planId = 1;

        var securityContextMock = new Mock<ISecurityContextAccessor>();
        var facilityExternalRepositoryMock = new Mock<IFacilityExternalRepository>();
        var optionItemRepositoryMock = new Mock<IOptionItemRepository>();
        var mapperMock = new Mock<IMapper>();
        var cacheServiceMock = new Mock<ICacheService>();

        securityContextMock.Setup(x => x.GetFacilityCodeSelected()).Returns(facilityCode);

        facilityExternalRepositoryMock
            .Setup(x => x.CheckFacilityAvailableAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var mockQueryable = new List<OptionItem>
            {
                new()
                {
                    IsEnabled = true,
                    PlanOptionItems = new List<PlanOptionItem>
                    {
                        new()
                        {
                            PlanId = planId,
                            Plan = new Plan { UseFixedOptionItem = true }
                        }
                    },
                    OptionItemAppDates = new List<OptionItemAppDate>
                    {
                        new()
                        {
                            AppDateId = appDateId,
                            IsNotSelled = false,
                            SellNumber = 10
                        }
                    }
                }
            }.AsQueryable()
            .BuildMock();

        optionItemRepositoryMock
            .Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(mockQueryable);

        var payload = new BookingOptionRequest(appDateId);
        var pageable = PageableBinderConfig.DefaultPageable;
        var query = new BookingGetAllOptionItemsQuery(1, payload, pageable)
        {
            PlanId = planId,
            Payload = payload,
            Pageable = pageable
        };

        mapperMock
            .Setup(m => m.ConfigurationProvider)
            .Returns(
                new MapperConfiguration(
                    cfg =>
                    {
                        cfg.CreateMap<OptionItem, OptionItemOfBookingResponse>();
                    }
                )
            );

        var handler = new BookingGetAllOptionItemsQueryHandler(
            mapperMock.Object,
            cacheServiceMock.Object,
            securityContextMock.Object,
            optionItemRepositoryMock.Object,
            facilityExternalRepositoryMock.Object
        );

        await Assert.ThrowsAsync<FacilityNotfoundException>(
            () => handler.Handle(query, CancellationToken.None)
        );
    }
}
