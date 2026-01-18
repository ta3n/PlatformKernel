using Liberty.Reservation.Site.Application.Exceptions;
using File = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.File;

namespace Liberty.Reservation.Site.Application.Domains.Services;

public class MediaService(
    ILogger<File> logger,
    IMediaRepository mediaRepository
) : BaseService<File>(logger, mediaRepository, new ImageNotfoundException()), IMediaService;
