namespace Liberty.Reservation.Employee.WebAPI.Application.Web.Extensions;

public class ActionResultWithHeaders(
    ActionResult receiver,
    IHeaderDictionary headers
) : ActionResult
{
    public ActionResult Receiver => receiver;

    private void AddHeaders(
        HttpResponse response
    )
    {
        foreach (var (name, value) in headers)
        {
            response.Headers.Append(name, value);
        }
    }

    public override Task ExecuteResultAsync(
        ActionContext context
    )
    {
        AddHeaders(context.HttpContext.Response);
        return receiver.ExecuteResultAsync(context);
    }

    public override void ExecuteResult(
        ActionContext context
    )
    {
        AddHeaders(context.HttpContext.Response);
        receiver.ExecuteResult(context);
    }
}
