using System.Net;
using System.Net.Http.Headers;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.File.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.File.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.File.WebAPI.Test.InfrastructureOfTest.Utilities;
using Microsoft.AspNetCore.Http;
using SkiaSharp;

namespace Liberty.Reservation.Manager.File.WebAPI.Test;

public class ImagesEndpointIntBaseIntegrationTest : BaseIntegrationTest
{
    private string BaseUrl { get; set; } = "/api/images";

    private async Task<Category> CreateCategoryFileAsync()
    {
        var categoryRepo = Factory.GetRequiredService<ICategoryRepository>();

        var existingCategory = await categoryRepo!.GetAllAsync();
        if (existingCategory is { Count: > 0 })
        {
            return existingCategory[0];
        }

        var facilityCategoryRepo = Factory.GetRequiredService<IFacilityCategoryRepository>()
            ?? throw new ArgumentException(nameof(IFacilityCategoryRepository));

        var categoryToCreate = new FacilityCategory
        {
            FacilityId = FacilityInfo.Id,
            Category = new Category
            {
                CategoryType = CategoryTypes.File,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Name" } },
                Description = new MultilingualText { { TestUtil.DefaultLanguageCode, "Description" } },
                IsEnabled = true
            },
            IsEnabled = true
        };

        var createdFileCategories = await facilityCategoryRepo.AddAsync(
            categoryToCreate,
            true
        );

        return createdFileCategories.Category!;
    }

    private static IFormFile CreateFakeImageFile
    {
        get
        {
            using var bitmap = new SKBitmap(100, 100);
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.Blue);

            var stream = new MemoryStream();
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Jpeg, 100);
            data.SaveTo(stream);
            stream.Position = 0;

            var formFile = new FormFile(stream, 0, stream.Length, "file", "test.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };

            return formFile;
        }
    }

    private async Task<string> CreateImagesAsync()
    {
        var category = await CreateCategoryFileAsync();
        var formFile = CreateFakeImageFile;
        var createReq = new ImageCreateRequest(
            formFile,
            [
                FilePurposeTypes.FacilityAppearance,
                FilePurposeTypes.FacilityMeal
            ],
            [
                category.Id
            ]
        );

        var content = new MultipartFormDataContent
        {
            {
                new StreamContent(formFile.OpenReadStream())
                {
                    Headers =
                    {
                        ContentLength = formFile.Length,
                        ContentType = new MediaTypeHeaderValue(formFile.ContentType)
                    }
                },
                "file", formFile.FileName
            },
            { new StringContent(string.Join(",", createReq.FilePurposeTypes ?? [])), "filePurposeTypes" },
            { new StringContent(string.Join(",", createReq.ImageCategoryIds!)), "imageCategoryIds" }
        };

        var response = await Client.PostAsync(BaseUrl, content);
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        return responseString;
    }

    [Fact]
    public async Task CreateImages_ReturnsOk_WithImages()
    {
        var responseString = await CreateImagesAsync();
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task UpdateImages_ReturnsOk_WithImages()
    {
        var newImagesResponse = await CreateImagesAsync();
        var newImagesId = long.TryParse(newImagesResponse, out var id) ? id : 0;

        var updateReq = new ImageUpdateRequest(
            true,
            1,
            "Description",
            ["hot"],
            [
                FilePurposeTypes.FacilityAppearance,
                FilePurposeTypes.FacilityMeal
            ],
            null,
            null
        ) { Id = newImagesId };

        var response = await Client.PutAsync(
            $"{BaseUrl}/{newImagesId}",
            TestUtil.ToJsonContent(updateReq)
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteImages_ReturnsOk_WithImages()
    {
        var newImagesResponse = await CreateImagesAsync();
        var newImagesId = long.TryParse(newImagesResponse, out var id) ? id : 0;

        var response = await Client.DeleteAsync(
            $"{BaseUrl}/{newImagesId}"
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetAllImages_ReturnsOk_WithImagesList()
    {
        await CreateImagesAsync();

        var response = await Client.GetAsync(BaseUrl);
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetImages_ReturnsOk_WithImages()
    {
        var newImagesResponse = await CreateImagesAsync();
        var newImagesId = long.TryParse(newImagesResponse, out var id) ? id : 0;
        var fileRepo = Factory.GetRequiredService<IFileRepository>();

        var code = (await fileRepo!.GetByIdAsync(newImagesId))?.Code;

        var response = await Client.GetAsync($"{BaseUrl}/{code}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(responseString);
    }
}
