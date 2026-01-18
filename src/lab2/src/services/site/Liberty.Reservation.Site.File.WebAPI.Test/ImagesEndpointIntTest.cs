using Liberty.ApplicationShared.Utils;
using Liberty.Media.Utils;
using Liberty.Reservation.Site.Application.Domains.Repositories.Interfaces;
using System.Net;
using Liberty.Reservation.Site.File.WebAPI.Test.InfrastructureOfTest;

namespace Liberty.Reservation.Site.File.WebAPI.Test;

public class ImagesEndpointIntTest : BaseIntegrationTest
{
    private const string BaseUrl = "api/images";

    private async Task<Reservation.Application.Contexts.DataContexts.Entities.Data.File> CreateImageAsync()
    {
        var mediaRepo = Factory.GetRequiredService<IMediaRepository>() ?? throw new NullReferenceException(nameof(IMediaRepository));

        var imageCreate = new Reservation.Application.Contexts.DataContexts.Entities.Data.File
        {
            Code = EntityUtil.CreateCode(),
            RecordMemo = EntityUtil.CreateRecordMemo(),
            FileSize = 10,
            Extension = "jpeg",
            ContentType = "image/jpeg"
        };

        return await mediaRepo.AddAsync(imageCreate, true);
    }

    [Fact]
    public async Task GetImage_ReturnsOk()
    {
        var newImage = await CreateImageAsync();

        var response = await Client.GetAsync(
            $"{BaseUrl}/{newImage.Code}/{nameof(ImageSizeType.Compress500)}"
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
