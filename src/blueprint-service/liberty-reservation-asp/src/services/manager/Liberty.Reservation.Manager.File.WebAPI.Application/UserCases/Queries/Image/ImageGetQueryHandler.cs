using Liberty.Media.Services;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.SysIntegrationEvent.Events;
using Liberty.SysIntegrationEvent.Models;
using MassTransit;

namespace Liberty.Reservation.Manager.File.WebAPI.Application.UserCases.Queries.Image;

public class ImageGetQueryHandler(
    IMapper mapper,
    IBus bus,
    IFileRepository fileRepository,
    IAwsS3Service awsS3Service
) : QuerySingleBaseHandler<ImageGetQuery, ImagePreviewResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, ImagePreviewResponse)> HandleAsync(
        ImageGetQuery request,
        CancellationToken cancellationToken
    )
    {
        var fileExits = await fileRepository
            .GetQueryableWithAsNoTracking()
            .AnyAsync(x => x.Code == request.Code, cancellationToken);

        if (!fileExits)
        {
            throw new ImageNotfoundException();
        }

        var keyCodeWithSizeType = request.Code;
        var sizeType = request.SizeType;

        if (request.SizeType != null)
        {
            keyCodeWithSizeType = $"{request.Code}-{request.SizeType}";
        }

        var (responseStream, contentType) = await awsS3Service.GetImageWithResizeAsync(
            request.Code,
            keyCodeWithSizeType,
            sizeType!,
            cancellationToken
        );

        if (responseStream is null)
        {
            var imageResizeModel = new ImageResizeModel(
                request.Code,
                keyCodeWithSizeType,
                sizeType!
            );
            var message = new ImageResizeEvent();
            message.SerializeJsonData(imageResizeModel);

            await bus.Send(
                message,
                cancellationToken
            );
        }

        if (responseStream is null)
        {
            throw new ImageNotfoundException();
        }

        var headers = new HeaderDictionary
        {
            { "Content-Type", contentType },
            { "Cache-Control", "public, max-age=3600" }
        };

        return (headers, new ImagePreviewResponse(contentType, responseStream));
    }
}
