using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.User.Application.Domains.Services.Interfaces;
using Liberty.Reservation.User.Application.Exceptions;
using Microsoft.Extensions.Logging;
using File = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.File;

namespace Liberty.Reservation.User.Application.Domains.Services;

public class MediaService(
    ILogger<File> logger,
    IMediaRepository mediaRepository
) : BaseService<File>(logger, mediaRepository, new ImageNotfoundException()), IMediaService;
