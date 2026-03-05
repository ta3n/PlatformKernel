using AutoMapper;
using Liberty.Entity.ValueObjects;
using Liberty.Pagination;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BookingReservation;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using MockQueryable;
using Moq;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class ReservationGetAllPersonAgeTypesQueryHandlerTest
{
    [Fact]
    public async Task ShouldReturnReservationNotfoundException()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var mapperMock = new Mock<IMapper>();
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var reservationRepositoryMock = new Mock<IReservationRepository>();
        var personAgeTypeRepositoryMock = new Mock<IPersonAgeTypeRepository>();

        var userCode = "test-user-code";
        var query = new ReservationGetAllPersonAgeTypesQuery(1, PageableBinderConfig.DefaultPageable);

        var existingReservation = new ReservationEntity
        {
            UserCode = userCode,
            Facility = new() { Id = 1 }
        };

        securityContextAccessorMock.Setup(x => x.ApplicationUserKey).Returns(userCode);

        reservationRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(new[] { existingReservation }.AsQueryable().BuildMock());

        var personAgeTypes = new List<PersonAgeType>
        {
            new()
            {
                Id = 1,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
                IsMain = true,
                AgeMin = 0,
                AgeMax = 12,
                IsEnabled = true,
                IsVisible = true
            }
        };

        personAgeTypeRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(personAgeTypes.AsQueryable().BuildMock());

        var mapperConfig = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<PersonAgeType, PersonAgeTypeResponse>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.Id))
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name!.GetValueByHeader()))
                    .ForMember(dest => dest.IsMain, opt => opt.MapFrom(x => x.IsMain))
                    .ForMember(dest => dest.AgeMin, opt => opt.MapFrom(x => x.AgeMin))
                    .ForMember(dest => dest.AgeMax, opt => opt.MapFrom(x => x.AgeMax));
            }
        );

        var mapper = new Mapper(mapperConfig);
        mapperMock.Setup(m => m.Map<PersonAgeTypeResponse>(It.IsAny<PersonAgeType>()))
            .Returns(mapper.Map<PersonAgeTypeResponse>(new PersonAgeType()));
        var handler = new ReservationGetAllPersonAgeTypesQueryHandler(
            mapperMock.Object,
            securityContextAccessorMock.Object,
            reservationRepositoryMock.Object,
            personAgeTypeRepositoryMock.Object
        );

        await Assert.ThrowsAsync<ReservationNotfoundException>(
            async () => await handler.Handle(query, cancellationToken)
        );
    }
}
