using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SharedKernel.Pagination.Extensions;

namespace SharedKernel.Pagination.Binders;

/// <summary>
/// Provides a model binder implementation for creating instances of <see cref="IPageable"/>
/// from HTTP query string parameters in an ASP.NET Core request pipeline.
/// </summary>
/// <remarks>
/// This model binder parses query string parameters to construct an <see cref="IPageable"/> object
/// that encapsulates pagination-related information, such as the page number, page size,
/// sort options, and pagination state.
/// </remarks>
/// <example>
/// The query string parameters are resolved based on the configuration provided by <see cref="PageableBinderConfig"/>.
/// Expected parameters include:
/// - Page number parameter (e.g., as defined by <c>PageParameterName</c>).
/// - Page size parameter (e.g., as defined by <c>SizeParameterName</c>).
/// - Pagination enabled parameter (e.g., as defined by <c>IsEnabledParameterName</c>).
/// </example>
/// <threadsafety>
/// This type is thread-safe as it does not maintain any instance-level state in its operations.
/// </threadsafety>
/// <seealso cref="IModelBinder"/>
/// <seealso cref="PageableBinderConfig"/>
/// <seealso cref="IPageable"/>
public class PageableBinder : IModelBinder
{
    /// <summary>
    /// A configuration instance of <see cref="PageableBinderConfig"/> utilized by the <see cref="PageableBinder"/>
    /// to define parameter names, default values, and behaviors when handling pageable query parameters.
    /// </summary>
    /// <remarks>
    /// The configuration includes settings such as parameter names for paging (page, size, etc.),
    /// delimiter characters for query parsing, maximum allowed page size, and fallback pageable values.
    /// </remarks>
    private readonly PageableBinderConfig _binderConfig = new();

    /// <summary>
    /// Binds a model of type <see cref="Pageable"/> from the query string of the HTTP request.
    /// </summary>
    /// <param name="bindingContext">
    /// The <see cref="ModelBindingContext"/> containing the context and metadata
    /// necessary for model binding.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. When completed,
    /// the binding result is assigned to the <see cref="ModelBindingContext.Result"/> property.
    /// </returns>
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

    /// Resolves a `Pageable` object from the given query string. Parses the query
    /// string parameters to extract pagination-related values such as page number,
    /// page size, whether pagination is enabled, and sort information.
    /// <param name="queryString">The query string containing pagination parameters.</param>
    /// <returns>A `Pageable` object populated with values parsed from the query string.</returns>
    private Pageable ResolvePageableArgumentFromQueryString(
        QueryString queryString
    )
    {
        var pageNumberString = queryString.GetParameter(_binderConfig.PageParameterName);
        var pageSizeString = queryString.GetParameter(_binderConfig.SizeParameterName);
        var isEnabledString = queryString.GetParameter(_binderConfig.IsEnabledParameterName);

        var (pageNumber, isParsePageNumber) = ParseIntOrDefault(pageNumberString, _binderConfig.FallbackPageable.PageNumber);
        if (!isParsePageNumber)
        {
            throw new ValidationException("Invalid page number.");
        }

        var (pageSize, isParsePageSize) = ParseIntOrDefault(
            pageSizeString,
            _binderConfig.FallbackPageable.PageSize,
            _binderConfig.MaxPageSize
        );
        if (!isParsePageSize)
        {
            throw new ValidationException("Invalid page size.");
        }

        var (isEnabled, isParseEnabled) = ParseBoolOrDefault(isEnabledString);
        if (!isParseEnabled)
        {
            throw new ValidationException("Invalid is enabled.");
        }

        var sort = ResolveSortArgument(queryString);

        return Pageable.Of(pageNumber, pageSize, isEnabled, sort);
    }

    /// <summary>
    /// Parses a string into an integer value or returns a default value if parsing fails.
    /// </summary>
    /// <param name="parameter">The string to be parsed into an integer.</param>
    /// <param name="defaultValue">The default integer value to return if parsing fails.</param>
    /// <param name="upper">The upper limit for the resulting integer. Defaults to <see cref="int.MaxValue"/>.</param>
    /// <returns>
    /// A tuple containing the parsed integer value (clamped between 0 and the specified upper limit)
    /// and a boolean indicating whether parsing was successful.
    /// </returns>
    private static (int value, bool isParse) ParseIntOrDefault(
        string? parameter,
        int defaultValue,
        int upper = int.MaxValue
    )
    {
        var isParse = true;
        if (!int.TryParse(parameter, out var value))
        {
            value = defaultValue;
            if (!string.IsNullOrEmpty(parameter))
            {
                isParse = false;
            }
        }

        value = value < 0 ? 0 : value;
        value = value > upper ? upper : value;

        return (value, isParse);
    }

    /// <summary>
    /// Parses the given string parameter into a nullable boolean value. If the parameter is not a valid
    /// boolean string, returns null, along with an indicator of whether parsing succeeded.
    /// </summary>
    /// <param name="parameter">
    /// The string parameter to be parsed into a boolean value.
    /// </param>
    /// <returns>
    /// A tuple containing:
    /// - The parsed nullable boolean value, which is null if parsing fails.
    /// - A boolean indicating whether the parsing operation was successful.
    /// </returns>
    private static (bool? value, bool isParse) ParseBoolOrDefault(
        string? parameter
    )
    {
        bool? response = null;
        var isParse = false;

        if (!bool.TryParse(parameter, out var value))
        {
            if (string.IsNullOrEmpty(parameter))
            {
                isParse = true;
            }

            return (response, isParse);
        }

        response = value;
        isParse = true;

        return (response, isParse);
    }

    /// <summary>
    /// Resolves and parses the sort argument from a given query string.
    /// Constructs and returns a <see cref="Sort"/> object based on the parsed sorting parameters found in the query string.
    /// </summary>
    /// <param name="queryString">The query string containing sorting parameters.</param>
    /// <return>Returns a <see cref="Sort"/> object representing the resolved sorting orders.</return>
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
