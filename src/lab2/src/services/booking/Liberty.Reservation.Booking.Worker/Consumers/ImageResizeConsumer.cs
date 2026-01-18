using Liberty.Media.Services;
using Liberty.Reservation.Booking.Worker.Consumers.Base;
using Liberty.SysIntegrationEvent.Events;
using Liberty.SysIntegrationEvent.Models;
using MassTransit;
using Newtonsoft.Json;

namespace Liberty.Reservation.Booking.Worker.Consumers;

public class ImageResizeConsumer(
    ILogger<ImageResizeConsumer> logger,
    IAwsS3Service awsS3Service
) : BaseConsumer<ImageResizeEvent>(logger)
{
    protected override async Task ExecuteAsync(
        ConsumeContext<ImageResizeEvent> context
    )
    {
        var jsonContext = context.Message.JsonData ?? string.Empty;
        var model = JsonConvert.DeserializeObject<ImageResizeModel>(
            jsonContext
        );

        var keyCode = model?.OriginalCode ?? string.Empty;
        var keyCodeWithSizeType = model?.CodeWithSizeType ?? string.Empty;
        var sizeType = model?.SizeType ?? string.Empty;

        try
        {
            var isExisting = await awsS3Service.DoesS3ObjectExistAsync(
                keyCodeWithSizeType
            );
            if (isExisting)
            {
                logger.LogInformation(
                    "Image resize skipped: Image already exists for {CodeWithSizeType}",
                    keyCodeWithSizeType
                );

                return;
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "Failed to check existence of image: {CodeWithSizeType}",
                keyCodeWithSizeType
            );
        }

        try
        {
            var (responseStream, contentType, fileName) = await awsS3Service.GetFileAsync(
                keyCode
            );
            if (responseStream is null)
            {
                return;
            }

            var (code, _) = await awsS3Service.UploadImageAsync(
                keyCodeWithSizeType,
                sizeType,
                fileName,
                contentType,
                responseStream
            );

            if (string.IsNullOrEmpty(code))
            {
                logger.LogWarning(
                    "Image resize failed: Original Image: {OriginalCode}, Requested Dimensions: {SizeType}",
                    keyCode,
                    sizeType
                );

                return;
            }

            logger.LogInformation(
                "Image resize successful: Original Image: {OriginalCode}, Resized Image: {CodeWithSizeType}",
                keyCode,
                keyCodeWithSizeType
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Image resize error: Original Image: {OriginalCode}, Requested Dimensions: {SizeType}",
                keyCode,
                sizeType
            );
        }
    }
}
