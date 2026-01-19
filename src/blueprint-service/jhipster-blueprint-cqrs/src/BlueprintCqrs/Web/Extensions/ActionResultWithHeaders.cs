using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlueprintCqrs.Web.Extensions;

public class ActionResultWithHeaders(
    ActionResult receiver,
    IHeaderDictionary headers
) : ActionResult
{
    private readonly IHeaderDictionary _headers = headers;
    private readonly ActionResult _result = receiver;

    private void AddHeaders(
        HttpResponse response
    )
    {
        foreach (var (name, value) in _headers)
        {
            response.Headers.Append(name, value);
        }
    }

    public override Task ExecuteResultAsync(
        ActionContext context
    )
    {
        AddHeaders(context.HttpContext.Response);
        return _result.ExecuteResultAsync(context);
    }

    public override void ExecuteResult(
        ActionContext context
    )
    {
        AddHeaders(context.HttpContext.Response);
        _result.ExecuteResult(context);
    }
}
