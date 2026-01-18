using Liberty.Media.Services;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.User.Application.Domains.Services.Interfaces;
using Liberty.Reservation.User.Application.Exceptions;
using Liberty.Reservation.User.File.WebAPI.Application.Models.Responses;
using Liberty.SysException.Exceptions;
using Liberty.SysIntegrationEvent.Events;
using Liberty.SysIntegrationEvent.Models;
using MassTransit;

namespace Liberty.Reservation.User.File.WebAPI.Application.UserCases.Queries.Media;

public class ImageGetQueryHandler(
    IMapper mapper,
    IBus bus,
    IMediaService mediaService,
    IAwsS3Service awsS3Service
) : QuerySingleBaseHandler<ImageGetQuery, ImagePreviewResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, ImagePreviewResponse)> HandleAsync(
        ImageGetQuery request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        var existingImage = await mediaService.CountByCodesAsync(
            [payload.Code],
            cancellationToken
        );
        if (existingImage is 0)
        {
            throw new AppFileNotfoundException(payload.Code);
        }

        var (responseStream, contentType) = await awsS3Service.GetImageWithResizeAsync(
            payload.Code,
            $"{payload.Code}-{payload.SizeType}",
            payload.SizeType,
            cancellationToken
        );

        if (responseStream is null)
        {
            var imageResizeModel = new ImageResizeModel(
                payload.Code,
                $"{payload.Code}-{payload.SizeType}",
                payload.SizeType
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

        var headers = new HeaderDictionary { { "Content-Type", contentType } };

        return (
            headers,
            new ImagePreviewResponse(
                contentType,
                responseStream
            )
        );
    }
}
