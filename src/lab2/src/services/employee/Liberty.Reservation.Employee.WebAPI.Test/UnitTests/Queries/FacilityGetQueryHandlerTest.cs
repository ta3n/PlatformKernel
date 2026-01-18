using Moq;
using AutoMapper;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Facility;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.ExternalServices.Membership.Facility.Repositories;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using MockQueryable;
using Liberty.Reservation.Employee.WebAPI.Application.ExternalServices.Membership.Facility.Dtos;
using Liberty.Reservation.Employee.Application.Exceptions;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest.Utilities;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests.Queries;

public class FacilityGetQueryHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnFacilityDetailResponse_WhenFacilityFound()
    {
        // Arrange
        long requestId = 1;

        var facilityExternal = new FacilityInfoDto
        {
            Id = requestId,
            Code = "F001",
            Name = "Facility Name",
            Kana = "FAC",
            Postcode = "12345",
            Address1 = "Address 1",
            Address2 = "Address 2",
            Address3 = "Address 3",
            Address4 = "Address 4",
            Memo = "Memo content"
        };

        var dummyFacility = new Facility
        {
            Id = requestId,
            Fax = "Fax123",
            CanOnLinePayment = true,
            IsEnabled = true,
            Meta = new FacilityMeta { SystemEMail = "system@fac.com" },
            FacilitySites = new List<FacilitySite>
            {
                new()
                {
                    SiteId = 100,
                    Site = new Site
                    {
                        Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test" } },
                        IsEnabled = true,
                        IsVisible = true
                    }
                }
            },
            FacilityFaxServices = new List<FacilityFaxService>
            {
                new()
                {
                    FaxServiceId = 200,
                    FaxService = new FaxService
                    {
                        Name = "FaxService 1",
                        IsEnabled = true,
                        IsVisible = true
                    }
                }
            }
        };

        var mockMapper = new Mock<IMapper>();

        var mockFacilityExternalRepository = new Mock<IFacilityExternalRepository>();
        mockFacilityExternalRepository
            .Setup(repo => repo.GetFacilityByIdAsync(requestId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(facilityExternal);

        var mockFacilityRepository = new Mock<IFacilityRepository>();
        mockFacilityRepository
            .Setup(repo => repo.GetQueryableWithAsNoTracking())
            .Returns(new List<Facility> { dummyFacility }.AsQueryable().BuildMock());

        // Tạo instance handler
        var handler = new FacilityGetQueryHandler(
            mockMapper.Object,
            mockFacilityExternalRepository.Object,
            mockFacilityRepository.Object
        );

        var query = new FacilityGetQuery(requestId);

        // Act
        var (_, response) = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(requestId, response.Id);
        Assert.Equal(facilityExternal.Code, response.Code);
        Assert.Equal(facilityExternal.Name, response.Name);
        Assert.Equal(facilityExternal.Kana, response.Kana);
        Assert.Equal(facilityExternal.Address1, response.Address1);
        Assert.Equal(facilityExternal.Address2, response.Address2);
        Assert.Equal(facilityExternal.Address3, response.Address3);
        Assert.Equal(facilityExternal.Address4, response.Address4);
        Assert.Equal(dummyFacility.Fax, response.Fax);
        Assert.Equal(facilityExternal.Memo, response.Memo);
        Assert.Equal(dummyFacility.CanOnLinePayment, response.CanOnLinePayment);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowFacilityNotfoundException_WhenFacilityExternalIsNull()
    {
        // Arrange
        long requestId = 1;
        var mockMapper = new Mock<IMapper>();

        var mockFacilityExternalRepository = new Mock<IFacilityExternalRepository>();
        mockFacilityExternalRepository
            .Setup(repo => repo.GetFacilityByIdAsync(requestId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((FacilityInfoDto?)null);

        var mockFacilityRepository = new Mock<IFacilityRepository>();
        mockFacilityRepository
            .Setup(repo => repo.GetQueryableWithAsNoTracking())
            .Returns(Enumerable.Empty<Facility>().AsQueryable());

        var handler = new FacilityGetQueryHandler(
            mockMapper.Object,
            mockFacilityExternalRepository.Object,
            mockFacilityRepository.Object
        );
        var query = new FacilityGetQuery(requestId);

        // Act & Assert
        await Assert.ThrowsAsync<FacilityNotfoundException>(() => handler.Handle(query, CancellationToken.None));
    }
}
