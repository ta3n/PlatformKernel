using AutoMapper;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.Application.Exceptions;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Facility;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using MockQueryable;
using Moq;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Queries;

public class FacilityGetBasicSettingQueryHandlerTest
{
    private readonly IMapper _mapper;

    public FacilityGetBasicSettingQueryHandlerTest()
    {
        var config = new MapperConfiguration(_ => { });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFacilityDetail_WhenFacilityExists()
    {
        // Arrange
        var facilityId = 1L;
        var facility = new Facility
        {
            Id = facilityId,
            Code = "F001",
            Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test Facility" } },
            Kana = "テスト施設",
            Description = "A test facility",
            Postcode = "123456",
            Address1 = new MultilingualText { { TestUtil.DefaultLanguageCode, "Address 1" } },
            Address2 = new MultilingualText { { TestUtil.DefaultLanguageCode, "Address 2" } },
            Address3 = new MultilingualText { { TestUtil.DefaultLanguageCode, "Address 3" } },
            Address4 = new MultilingualText { { TestUtil.DefaultLanguageCode, "Address 4" } },
            Phone = "123-456-7890",
            Fax = "098-765-4321",
            Url = "https://testfacility.com",
            AreaId = 10,
            CategoryId = 20,
            Meta = new()
            {
                RoomNumberWesternStyle = 5,
                RoomNumberJapaneseStyle = 3,
                RoomNumberJapaneseWesternStyle = 4,
                RoomNumberOtherStyle = 2,
                Logo = "logo.png"
            }
        };

        var facilities = new List<Facility> { facility }.AsQueryable().BuildMock();

        var mockFacilityRepository = new Mock<IFacilityRepository>();
        mockFacilityRepository.Setup(r => r.GetQueryableWithAsNoTracking()).Returns(facilities);

        var mockFacilityFileRepository = new Mock<IFacilityFileRepository>();
        mockFacilityFileRepository
            .Setup(r => r.GetQueryableWithAsNoTracking())
            .Returns(new List<FacilityFile>().AsQueryable().BuildMock());

        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        var handler = new FacilityGetBasicSettingQueryHandler(
            _mapper,
            mockFacilityRepository.Object,
            mockFacilityFileRepository.Object,
            mockSecurityContextAccessor.Object
        );

        var query = new FacilityGetBasicSettingQuery();

        // Act
        var (_, response) = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowFacilityNotfoundException_WhenFacilityDoesNotExist()
    {
        // Arrange
        var facilityId = 1L;

        var facilities = new List<Facility>().AsQueryable().BuildMock();

        var mockFacilityRepository = new Mock<IFacilityRepository>();
        mockFacilityRepository.Setup(r => r.GetQueryableWithAsNoTracking()).Returns(facilities);

        var mockFacilityFileRepository = new Mock<IFacilityFileRepository>();
        mockFacilityFileRepository
            .Setup(r => r.GetQueryableWithAsNoTracking())
            .Returns(new List<FacilityFile>().AsQueryable().BuildMock());

        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        var handler = new FacilityGetBasicSettingQueryHandler(
            _mapper,
            mockFacilityRepository.Object,
            mockFacilityFileRepository.Object,
            mockSecurityContextAccessor.Object
        );

        var query = new FacilityGetBasicSettingQuery();

        // Act & Assert
        await Assert.ThrowsAsync<FacilityNotfoundException>(() => handler.Handle(query, CancellationToken.None));
    }
}
