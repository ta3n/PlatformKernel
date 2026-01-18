using Liberty.Pagination.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Liberty.Pagination.Binders;

public class PageableBinder : IModelBinder
{
    private readonly PageableBinderConfig _binderConfig = new();

    public Task BindModelAsync(
        ModelBindingContext bindingContext
    )
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var queryString = bindingContext.HttpContext.Request.QueryString;
        var pageable = ResolvePageableArgumentFromQueryString(queryString);
        bindingContext.Result = ModelBindingResult.Success(pageable);
        return Task.CompletedTask;
    }

    private IPageable ResolvePageableArgumentFromQueryString(
        QueryString queryString
    )
    {
        var pageNumberString = queryString.GetParameter(_binderConfig.PageParameterName);
        var pageSizeString = queryString.GetParameter(_binderConfig.SizeParameterName);

        var pageNumber = ParseIntOrDefault(pageNumberString, _binderConfig.FallbackPageable.PageNumber);
        var pageSize = ParseIntOrDefault(
            pageSizeString,
            _binderConfig.FallbackPageable.PageSize,
            _binderConfig.MaxPageSize
        );

        var sort = ResolveSortArgument(queryString);
        return Pageable.Of(pageNumber, pageSize, sort);
    }

    private static int ParseIntOrDefault(
        string? parameter,
        int defaultValue,
        int upper = int.MaxValue
    )
    {
        if (!int.TryParse(parameter, out var value))
        {
            value = defaultValue;
        }

        value = value < 0 ? 0 : value;
        value = value > upper ? upper : value;

        return value;
    }

    private Sort ResolveSortArgument(
        QueryString queryString
    )
    {
        var sortParts = queryString.GetParameterValues(_binderConfig.SortParameterName);
        var orders = new List<Order>();
        foreach (var sortPart in sortParts)
        {
            var sortSpt = sortPart?.Split(_binderConfig.SortDelimiter);
            var property = sortSpt?[0];
            var direction = sortSpt is [_, "desc"]
                ? Direction.Desc
                : Sort.DefaultDirection;
            if (!string.IsNullOrEmpty(property))
            {
                orders.Add(new Order(direction, property));
            }
        }

        return new Sort(orders);
    }
}
