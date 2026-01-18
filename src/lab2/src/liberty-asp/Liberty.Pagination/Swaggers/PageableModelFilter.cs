using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Liberty.Pagination.Swaggers;

/// <summary>
/// Represents a custom operation filter to enhance OpenAPI documentation
/// for pageable models within the API.
/// </summary>
/// <remarks>
/// This filter applies modifications to the OpenAPI operation to standardize
/// the representation of pageable requests and responses, ensuring consistency
/// in the generated documentation. It is typically added to the Swagger
/// configuration during startup.
/// </remarks>
/// <example>
/// Can be used as part of Swagger/OpenAPI configuration by adding this filter
/// to the operation filters collection.
/// </example>
/// <seealso cref="Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter" />
public class PageableModelFilter : IOperationFilter
{
    /// <summary>
    /// Applies the pageable model filter on the given operation during the Swagger generation process.
    /// Modifies the operation parameters based on whether the method is a GET or POST request, and if it utilizes pagination via the <see cref="IPageable"/> interface.
    /// </summary>
    /// <param name="operation">
    /// Represents the API operation being processed. This can be modified to reflect changes to the API definition in Swagger documentation.
    /// </param>
    /// <param name="context">
    /// Provides context for the current operation, including details about the API description
    /// and associated parameter descriptions.
    /// </param>
    public void Apply(
        OpenApiOperation? operation,
        OperationFilterContext context
    )
    {
        var description = context.ApiDescription;
        var isGetMethod = description.HttpMethod is not null
            && description.HttpMethod.Equals(
                HttpMethod.Get.ToString(),
                StringComparison.CurrentCultureIgnoreCase
            );
        var isPostMethod = description.HttpMethod is not null
            && description.HttpMethod.Equals(
                HttpMethod.Post.ToString(),
                StringComparison.CurrentCultureIgnoreCase
            );

        if (!isGetMethod && !isPostMethod)
        {
            // We only want to do this for GET OR POST requests, if this is not a
            // GET request, leave this operation as is, do not modify
            return;
        }

        if (operation == null)
        {
            return;
        }

        if (isGetMethod && description.ParameterDescriptions.Any(elt => elt.Type == typeof(IPageable)))
        {
            // We cleared all the auto generated parameters
            // So now we are going to add back the three parameters page, size and sort
            operation.RequestBody = null;

            operation.RequestBody?.Content?.Clear();
        }
        else if (isPostMethod && description.ParameterDescriptions.Any(elt => elt.Type == typeof(IPageable)))
        {
            operation.Parameters = [.. operation.Parameters.Where(p => !IsPageableParameter(context, p))];
        }
        else
        {
            return;
        }

        // This first parameter is the zero based page number (offset)
        operation.Parameters.Add(
            new OpenApiParameter
            {
                Name = "page",
                In = ParameterLocation.Query,
                Schema = new OpenApiSchema { Type = "number" }
            }
        );

        // This parameter is the number of entities on each page
        operation.Parameters.Add(
            new OpenApiParameter
            {
                Name = "size",
                In = ParameterLocation.Query,
                Schema = new OpenApiSchema { Type = "number" }
            }
        );

        operation.Parameters.Add(
            new OpenApiParameter
            {
                Name = "enabled",
                In = ParameterLocation.Query,
                Schema = new OpenApiSchema { Type = "boolean" }
            }
        );
    }

    /// <summary>
    /// Determines if a given parameter is part of the pageable parameter set
    /// (i.e., contained within an instance of <see cref="IPageable"/> or <see cref="Sort"/>).
    /// </summary>
    /// <param name="context">The current operation filter context containing details about the API operation.</param>
    /// <param name="parameter">The OpenAPI parameter to check against the pageable definitions.</param>
    /// <returns>True if the parameter belongs to a pageable set; otherwise, false.</returns>
    private static bool IsPageableParameter(
        OperationFilterContext context,
        OpenApiParameter parameter
    )
    {
        var pageableParameters = context.ApiDescription.ParameterDescriptions
            .Where(
                p => p.ModelMetadata.ContainerType == typeof(IPageable)
                    || p.ModelMetadata.ContainerType == typeof(Sort)
            )
            .Select(p => p.Name);

        return pageableParameters.Contains(parameter.Name);
    }
}

/// <summary>
/// Represents a custom operation filter to enhance OpenAPI documentation
/// for pageable models with sorting capabilities within the API.
/// </summary>
/// <remarks>
/// This filter applies modifications to the OpenAPI operation to standardize
/// the representation of pageable requests and responses, ensuring consistency
/// in the generated documentation. It is typically added to the Swagger
/// configuration during startup.
/// </remarks>
/// <example>
/// Can be used as part of Swagger/OpenAPI configuration by adding this filter
/// to the operation filters collection.
/// </example>
/// <seealso cref="Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter" />
public class PageableIncludeSortModelFilter : IOperationFilter
{
    /// <summary>
    /// Applies the pageable model filter with sorting capabilities on the given operation
    /// during the Swagger generation process. Modifies the operation parameters based on
    /// whether the method is a GET or POST request, and if it utilizes pagination via the
    /// <see cref="IPageable"/> interface.
    /// </summary>
    /// <param name="operation">
    /// Represents the API operation being processed. This can be modified to reflect changes
    /// to the API definition in Swagger documentation.
    /// </param>
    /// <param name="context">
    /// Provides context for the current operation, including details about the API description
    /// and associated parameter descriptions.
    /// </param>
    public void Apply(
        OpenApiOperation? operation,
        OperationFilterContext context
    )
    {
        var description = context.ApiDescription;
        var isGetMethod = description.HttpMethod is not null
            && description.HttpMethod.Equals(
                HttpMethod.Get.ToString(),
                StringComparison.CurrentCultureIgnoreCase
            );
        var isPostMethod = description.HttpMethod is not null
            && description.HttpMethod.Equals(
                HttpMethod.Post.ToString(),
                StringComparison.CurrentCultureIgnoreCase
            );

        if (!isGetMethod && !isPostMethod)
        {
            // Only process GET or POST requests; leave other operations unmodified.
            return;
        }

        if (operation == null)
        {
            return;
        }

        if (isGetMethod && description.ParameterDescriptions.Any(elt => elt.Type == typeof(IPageable)))
        {
            // Clear auto-generated parameters and add custom pageable parameters.
            operation.RequestBody = null;
            operation.RequestBody?.Content?.Clear();
        }
        else if (isPostMethod && description.ParameterDescriptions.Any(elt => elt.Type == typeof(IPageable)))
        {
            operation.Parameters = [.. operation.Parameters.Where(p => !IsPageableParameter(context, p))];
        }
        else
        {
            return;
        }

        // Add the "page" parameter for zero-based page number (offset).
        operation.Parameters.Add(
            new OpenApiParameter
            {
                Name = "page",
                In = ParameterLocation.Query,
                Schema = new OpenApiSchema { Type = "number" }
            }
        );

        // Add the "size" parameter for the number of entities per page.
        operation.Parameters.Add(
            new OpenApiParameter
            {
                Name = "size",
                In = ParameterLocation.Query,
                Schema = new OpenApiSchema { Type = "number" }
            }
        );

        // Add the "enabled" parameter to indicate whether pagination is enabled.
        operation.Parameters.Add(
            new OpenApiParameter
            {
                Name = "enabled",
                In = ParameterLocation.Query,
                Schema = new OpenApiSchema { Type = "boolean" }
            }
        );

        // Add the "sort" parameter to describe how the list should be sorted.
        operation.Parameters.Add(
            new OpenApiParameter
            {
                Name = "sort",
                In = ParameterLocation.Query,
                Schema = new OpenApiSchema { Type = "string" }
            }
        );
    }

    /// <summary>
    /// Determines if a given parameter is part of the pageable parameter set
    /// (i.e., contained within an instance of <see cref="IPageable"/> or <see cref="Sort"/>).
    /// </summary>
    /// <param name="context">The current operation filter context containing details about the API operation.</param>
    /// <param name="parameter">The OpenAPI parameter to check against the pageable definitions.</param>
    /// <returns>True if the parameter belongs to a pageable set; otherwise, false.</returns>
    private static bool IsPageableParameter(
        OperationFilterContext context,
        OpenApiParameter parameter
    )
    {
        var pageableParameters = context.ApiDescription.ParameterDescriptions
            .Where(
                p => p.ModelMetadata.ContainerType == typeof(IPageable)
                    || p.ModelMetadata.ContainerType == typeof(Sort)
            )
            .Select(p => p.Name);

        return pageableParameters.Contains(parameter.Name);
    }
}
