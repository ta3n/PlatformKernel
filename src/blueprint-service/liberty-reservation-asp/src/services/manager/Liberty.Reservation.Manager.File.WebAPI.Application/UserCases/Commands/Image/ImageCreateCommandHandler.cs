using Liberty.ApplicationShared.Utils;
using Liberty.Media.Models.Requests;
using Liberty.Media.Services;
using Liberty.Media.Utils;
using Liberty.Reservation.Manager.Application.Auth;
using FileEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.File;

namespace Liberty.Reservation.Manager.File.WebAPI.Application.UserCases.Commands.Image;

public class ImageCreateCommandHandler(
    ILogger<ImageCreateCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IFacilityFileService facilityFileService,
    IFileCategoryService fileCategoryService,
    ICategoryService categoryService,
    IAwsS3Service awsS3Service
) : CreateCommandHandlerBase<ImageCreateCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        ImageCreateCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var fileExtension = Path.GetExtension(payload.File.FileName).ToLowerInvariant();
        var fileCode = ImageConvertUtil.GenerateRandomKey(fileExtension);

        var uploadFileToS3 = new UploadFileRequest(payload.File, fileCode, true, null);
        var uploadFileS3 = awsS3Service.UploadFileAsync(uploadFileToS3, cancellationToken);

        var (facilityFile, listFileCategory) = await HandleFileAsync(
            payload,
            fileExtension,
            fileCode,
            cancellationToken
        );

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var newfacilityFile = facilityFileService.CreateAsync(
                facilityFile,
                false,
                cancellationToken
            );

            var newfileCategory = fileCategoryService.CreateRangeAsync(
                listFileCategory,
                false,
                cancellationToken
            );

            await Task.WhenAll(newfacilityFile, newfileCategory, uploadFileS3);

            await UnitOfWork.CommitAsync(cancellationToken);

            return newfacilityFile.Result.FileId;
        }
        catch (Exception ex) when (uploadFileS3.IsFaulted)
        {
            logger.LogError(ex, "{Action} {Message}", nameof(ImageCreateCommandHandler), ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Action} {Message}", nameof(ImageCreateCommandHandler), ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);

            if (!uploadFileS3.IsCompletedSuccessfully)
            {
                throw new AppLibertyException(ex.Message, ex);
            }

            try
            {
                await awsS3Service.RemoveFileAsync(fileCode, cancellationToken);
                logger.LogInformation("S3 file {FileCode} deleted due to DB error", fileCode);
            }
            catch (Exception s3Ex)
            {
                logger.LogError(s3Ex, "Failed to delete S3 file {FileCode} after DB error", fileCode);
            }

            throw new AppLibertyException(ex.Message, ex);
        }
    }

    private async Task<(FacilityFile facilityFile, List<FileCategory> listFileCategory)> HandleFileAsync(
        ImageCreateRequest payload,
        string fileExtension,
        string fileCode,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;
        var listFileCategory = new List<FileCategory>();
        if (payload.ImageCategoryIds is not null)
        {
            var existingCategoryCount = await categoryService.CountByIdsAsync(
                [.. payload.ImageCategoryIds],
                [
                    CategoryTypes.File
                ],
                cancellationToken
            );

            if (existingCategoryCount != payload.ImageCategoryIds.Count)
            {
                throw new CategoryNotfoundException();
            }
        }

        var filePurposeType = payload.FilePurposeTypes?.Aggregate(
                FilePurposeTypes.None,
                (
                    acc,
                    flag
                ) => acc | flag
            )
            ?? FilePurposeTypes.None;

        var facilityFile = new FacilityFile
        {
            FacilityId = facilityId,
            FilePurposeType = filePurposeType,
            File = new FileEntity
            {
                Code = fileCode,
                Encrypt = EncryptUtil.Sha256(fileCode),
                Secret = string.Empty,
                FileSize = payload.File.Length,
                ContentType = payload.File.ContentType,
                Extension = fileExtension,
                IsEnabled = true
            },
            IsEnabled = true
        };

        if (payload.ImageCategoryIds is null)
        {
            return (facilityFile, listFileCategory);
        }

        foreach (var categoryId in payload.ImageCategoryIds)
        {
            var fileCategory = new FileCategory
            {
                File = facilityFile.File,
                CategoryId = categoryId
            };
            listFileCategory.Add(fileCategory);
        }

        return (facilityFile, listFileCategory);
    }
}
