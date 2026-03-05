using AutoMapper;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.Application.Exceptions;
using Liberty.Reservation.Employee.WebAPI.Application.ExternalServices.Membership.Facility.Dtos;
using Liberty.Reservation.Employee.WebAPI.Application.ExternalServices.Membership.Facility.Repositories;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Facility;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest.Utilities;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests.Commands;

public class FacilityCreateCommandHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldCreateFacilityAndReturnResponse()
    {
        var loggerMock = new Mock<ILogger<FacilityCreateCommandHandler>>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var facilityServiceMock = new Mock<IFacilityService>();
        var facilityInitDefaultServiceMock = new Mock<IFacilityInitDefaultDataService>();
        var facilityExternalRepositoryMock = new Mock<IFacilityExternalRepository>();

        var facilityId = 1;
        var request = new FacilityCreateCommand { Payload = new FacilityCreateRequest(facilityId) };
        var facility = new Facility
        {
            Id = facilityId,
            Code = "FAC123",
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test Facility" } }
        };
        var expectedResponse = new FacilityResponse(facilityId, "FAC123", 1, true, "Test Facility");

        var expectedFacility = new FacilityInfoDto
        {
            Id = facilityId,
            Code = "FAC123",
            Name = "Test Facility",
            Kana = "テスト施設",
            Email = "facility@example.com",
            Tel = "123-456-7890",
            Mobile = "098-765-4321",
            Postcode = "123-4567",
            CountryCode = "JP",
            Address1 = "123 Main St",
            Address2 = "Apt 101",
            Address3 = "Some City",
            Address4 = "Some Region",
            Memo = "Test Memo"
        };

        facilityExternalRepositoryMock.Setup(x => x.GetFacilityByIdAsync(facilityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedFacility);
        facilityServiceMock.Setup(x => x.CreateAsync(It.IsAny<Facility>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);
        mapperMock.Setup(x => x.Map<FacilityResponse>(facility)).Returns(expectedResponse);

        var handler = new FacilityCreateCommandHandler(
            loggerMock.Object,
            unitOfWorkMock.Object,
            mapperMock.Object,
            facilityServiceMock.Object,
            facilityInitDefaultServiceMock.Object,
            facilityExternalRepositoryMock.Object
        );

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(facilityId, result.Id);
        Assert.Equal("FAC123", result.Code);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowFacilityNotFoundException_WhenFacilityDoesNotExist()
    {
        var facilityExternalRepositoryMock = new Mock<IFacilityExternalRepository>();
        var handler = new FacilityCreateCommandHandler(
            new Mock<ILogger<FacilityCreateCommandHandler>>().Object,
            new Mock<IUnitOfWork>().Object,
            new Mock<IMapper>().Object,
            new Mock<IFacilityService>().Object,
            new Mock<IFacilityInitDefaultDataService>().Object,
            facilityExternalRepositoryMock.Object
        );

        var request = new FacilityCreateCommand { Payload = new FacilityCreateRequest(1) };

        facilityExternalRepositoryMock.Setup(x => x.GetFacilityByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((FacilityInfoDto)null!);

        await Assert.ThrowsAsync<FacilityNotfoundException>(() => handler.Handle(request, CancellationToken.None));
    }
}
