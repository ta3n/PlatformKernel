using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Boundaries.Restful.Base;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Queries.Rss;
using Microsoft.AspNetCore.Authorization;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Boundaries.Restful;

[AllowAnonymous]
[Route("api/rss")]
public class RssEndpoint(
    IMapper mapper,
    IMediator mediator
) : BaseEndpoint(mapper, mediator)
{
    private const string RssContentType = "application/rss+xml";

    [HttpGet("facilities")]
    [Produces(RssContentType)]
    [ProducesResponseType(typeof(SyndicationFeed), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllFacilities(
        [FromQuery] RssRequest request,
        CancellationToken cancellationToken
    )
    {
        var (header, response) = await Mediator!.Send(
            new FacilityGetAllQuery(request),
            cancellationToken
        );
        var output = WriteRssOutput(response);
        if (header["ErrorCode"] == StatusCodes.Status400BadRequest.ToString())
        {
            return new ContentResult
            {
                Content = output,
                ContentType = RssContentType,
                StatusCode = StatusCodes.Status400BadRequest
            };
        }

        return Content(
            output,
            RssContentType,
            Encoding.UTF8
        );
    }

    [HttpGet("booking")]
    [Produces(RssContentType)]
    [ProducesResponseType(typeof(SyndicationFeed), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllBooking(
        [FromQuery] GetBookingSearchRequest request,
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var (header, response) = await Mediator!.Send(
            new BookingGetAllQuery(request, pageable),
            cancellationToken
        );

        var output = WriteRssOutput(response);
        if (header["ErrorCode"] == StatusCodes.Status400BadRequest.ToString())
        {
            return new ContentResult
            {
                Content = output,
                ContentType = RssContentType,
                StatusCode = StatusCodes.Status400BadRequest
            };
        }

        return Content(
            output,
            RssContentType,
            Encoding.UTF8
        );
    }

    private static string WriteRssOutput(
        SyndicationFeed feed
    )
    {
        var utf8 = new UTF8Encoding(false);
        var feedBytes = RenderFeedUtf8(feed, utf8);

        // 2) Post-process XML: add xmlns:dc on <rss>, remove on <channel> if duplicated
        var doc = XDocument.Parse(utf8.GetString(feedBytes), LoadOptions.PreserveWhitespace);
        var rss = doc.Root!;
        rss.SetAttributeValue(XNamespace.Xmlns + "dc", "http://purl.org/dc/elements/1.1/");
        rss.Element("channel")?.Attribute(XNamespace.Xmlns + "dc")?.Remove();

        // 3) Write back -> UTF-8 string WITH XML declaration
        using var ms = new MemoryStream();
        using (var xw = XmlWriter.Create(
                ms,
                new XmlWriterSettings
                {
                    Indent = true,
                    Encoding = utf8,
                    OmitXmlDeclaration = false
                }
            ))
        {
            doc.WriteTo(xw);
        }

        return utf8.GetString(ms.ToArray());
    }

    private static byte[] RenderFeedUtf8(
        SyndicationFeed feed,
        Encoding enc
    )
    {
        using var ms = new MemoryStream();
        using (var xw = XmlWriter.Create(
                ms,
                new XmlWriterSettings
                {
                    Indent = true,
                    Encoding = enc,
                    OmitXmlDeclaration = false
                }
            ))
        {
            new Rss20FeedFormatter(feed, false).WriteTo(xw);
        }

        return ms.ToArray();
    }
}
