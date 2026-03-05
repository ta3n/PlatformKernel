using Liberty.Fax.Services;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.UnitOfWork.Abstractions;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Facility;

public class FacilitySendFaxCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IFaxService faxService
) : ActionCommandHandlerBase<FacilitySendFaxCommand, (IHeaderDictionary, SentFaxResponse)>(unitOfWork, mapper)
{
    protected override async Task<(IHeaderDictionary, SentFaxResponse)> HandleAsync(
        FacilitySendFaxCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var facilityId = payload.Id ?? 0;
        var bodyConvert = $"""
                Subject: {payload.Subject}

                Body:
                {payload.Body}
            """;
        var faxResponse = await faxService.SendFaxAsync(
            facilityId,
            payload.FaxNumber,
            payload.Subject,
            bodyConvert,
            cancellationToken
        );

        var header = new HeaderDictionary();
        var response = new SentFaxResponse(
            faxResponse?.IsSuccess,
            faxResponse?.Result,
            faxResponse?.ProcessKey,
            faxResponse?.AcceptTime
        );

        if (response.IsSuccess is not true)
        {
            throw new FaxNumberIncorrectException(
                response.Result ?? string.Empty
            );
        }

        return (header, response);
    }
}
