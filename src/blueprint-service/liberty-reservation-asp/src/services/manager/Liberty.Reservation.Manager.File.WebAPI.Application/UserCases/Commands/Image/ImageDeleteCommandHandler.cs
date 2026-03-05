using Liberty.Media.Services;

namespace Liberty.Reservation.Manager.File.WebAPI.Application.UserCases.Commands.Image;

public class ImageDeleteCommandHandler(
    ILogger<ImageDeleteCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IFacilityFileService facilityFileService,
    IFilePlanService filePlanService,
    IFileRoomGroupService fileRoomGroupService,
    IFileOptionItemService fileOptionItemService,
    IAwsS3Service awsS3Service
) : DeleteCommandHandlerBase<ImageDeleteCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        ImageDeleteCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        var facilityFile = await facilityFileService.GetByFileIdAsync(
            payload.Id,
            cancellationToken
        );
        facilityFile.File!.IsDeleted = true;

        try
        {
            await awsS3Service.RemoveFileAsync(
                facilityFile.File!.Code!,
                cancellationToken
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "{Action} {Message}",
                nameof(ImageDeleteCommandHandler),
                ex.Message
            );
        }

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var filePlan = await filePlanService.GetByFileIdAsync(
                payload.Id,
                cancellationToken
            );

            await filePlanService.DeleteRangeAsync(
                filePlan,
                false,
                cancellationToken
            );

            var fileRoomGroup = await fileRoomGroupService.GetByFileIdAsync(
                payload.Id,
                cancellationToken
            );

            await fileRoomGroupService.DeleteRangeAsync(
                fileRoomGroup,
                false,
                cancellationToken
            );

            var fileOptionItem = await fileOptionItemService.GetByFileIdAsync(
                payload.Id,
                cancellationToken
            );

            await fileOptionItemService.DeleteRangeAsync(
                fileOptionItem,
                false,
                cancellationToken
            );

            var deleteFile = await facilityFileService.DeleteAsync(
                facilityFile,
                false,
                cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            return deleteFile.FileId;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Action} {Message}", nameof(ImageDeleteCommandHandler), ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
