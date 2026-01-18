using AutoMapper;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.OptionItemInventory;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Moq;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Pagination;
using MockQueryable;
using Liberty.Reservation.Application.Constants;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class OptionItemInventoryGetAllQueryHandlerTest
{
    private readonly IMapper _mapper;

    public OptionItemInventoryGetAllQueryHandlerTest()
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<OptionItemAppDate, OptionItemAppDateResponse>()
                    .ConstructUsing(
                        src => new OptionItemAppDateResponse(
                            src.AppDateId,
                            src.OptionItemId,
                            src.OptionItem!.Name!.GetValueByHeader(),
                            src.SellNumber ?? 0,
                            src.IsNotSelled
                        )
                    )
                    .ForMember(
                        des => des.ReservedNumber,
                        opt =>
                            opt.MapFrom(
                                src =>
                                    src.OptionItem!.ReservationRoomGroupAppDateOptionItems!
                                        .Where(x => x.Reservation!.CheckInDate == src.AppDateId)
                                        .Count(
                                            x =>
                                                x.Reservation!.ReservationState == ReservationStatus.Confirmed
                                                || x.Reservation!.ReservationState == ReservationStatus.Reserved
                                                || x.Reservation!.ReservationState == ReservationStatus.Modified
                                        )
                            )
                    );
            }
        );

        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnOptionItemAppDateResponses_WhenDataExists()
    {
        // Arrange
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockOptionItemAppDateRepository = new Mock<IOptionItemAppDateRepository>();

        var facilityId = 1;
        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        var optionItemAppDates = new List<OptionItemAppDate>
        {
            new()
            {
                OptionItemId = 10,
                SellNumber = 100,
                AppDateId = 20240218,
                OptionItem = new OptionItem
                {
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Option 10" } },
                    FacilityOptionItems = new List<FacilityOptionItem> { new() { FacilityId = facilityId } },
                    ReservationRoomGroupAppDateOptionItems =
                        new List<ReservationRoomGroupAppDateOptionItem>
                        {
                            new()
                            {
                                Reservation = new()
                                {
                                    CheckInDate = 20240218,
                                    ReservationState = ReservationStatus.Confirmed
                                }
                            },
                            new()
                            {
                                Reservation = new()
                                {
                                    CheckInDate = 20240218,
                                    ReservationState = ReservationStatus.Reserved
                                }
                            }
                        }
                }
            },
            new()
            {
                OptionItemId = 20,
                SellNumber = 50,
                AppDateId = 20240219,
                OptionItem = new OptionItem
                {
                    Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Option 20" } },
                    FacilityOptionItems = new List<FacilityOptionItem> { new() { FacilityId = facilityId } },
                    ReservationRoomGroupAppDateOptionItems = new List<ReservationRoomGroupAppDateOptionItem>
                    {
                        new()
                        {
                            Reservation = new()
                            {
                                CheckInDate = 20240219,
                                ReservationState = ReservationStatus.Confirmed
                            }
                        }
                    }
                }
            }
        };

        var mockDbSet = optionItemAppDates.AsQueryable().AsEnumerable().BuildMock();

        mockOptionItemAppDateRepository.Setup(repo => repo.GetQueryableWithAsNoTracking())
            .Returns(mockDbSet);

        var pageable = PageableBinderConfig.DefaultPageable;
        var query = new OptionItemInventoryGetAllQuery(pageable, 20240218, 20240219);

        var handler = new OptionItemInventoryGetAllQueryHandler(
            _mapper,
            mockSecurityContextAccessor.Object,
            mockOptionItemAppDateRepository.Object
        );

        // Act
        var (_, responses) = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(responses);
        Assert.Equal(2, responses.Count());
    }
}
