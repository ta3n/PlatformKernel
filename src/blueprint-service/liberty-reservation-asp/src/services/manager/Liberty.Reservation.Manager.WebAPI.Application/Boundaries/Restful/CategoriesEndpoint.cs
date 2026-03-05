using Liberty.Reservation.Application.Utils;

namespace Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;

[Route("api/categories")]
public abstract class BaseCategoriesEndpoint(
    IMapper mapper,
    ICategoryService categoryService,
    ICacheManagementService cacheManagementService,
    CategoryTypes categoryType
) : BaseEndpoint(mapper)
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public virtual async Task<IActionResult> CreateCategory(
        [FromBody] CategoryCreateRequest request,
        CancellationToken cancellationToken
    )
    {
        var validation = await new CategoryCreateRequestValidator().ValidateAsync(
            request,
            cancellationToken
        );
        if (!validation.IsValid)
        {
            throw new AppRequestInvalidException(
                validation.GetErrorCode(),
                validation.GetErrorMessage(),
                validation.GetErrorField()
            );
        }

        var category = Mapper.Map<Category>(request);
        category.CategoryType = categoryType;

        var response = await categoryService.CreateAsync(
            category,
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response.Id)
            .WithHeaders(
                HeaderUtil.CreateEntityCreationAlert(
                    nameof(Category),
                    response.Id.ToString()
                )
            );
    }

    [HttpPut("order")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public virtual async Task<IActionResult> ArrangeOrderOfCategories(
        [FromBody] ItemUpdateOrderRequest request,
        CancellationToken cancellationToken
    )
    {
        var validation = await new ItemUpdateArrangeOrderRequestValidator().ValidateAsync(
            request,
            cancellationToken
        );
        if (!validation.IsValid)
        {
            throw new AppRequestInvalidException(
                validation.GetErrorCode(),
                validation.GetErrorMessage(),
                validation.GetErrorField()
            );
        }

        await categoryService.ArrangeOrderAsync(
            request.Ids,
            cancellationToken
        );

        cacheManagementService.RemoveFacilityRelatedCache();

        return NoContent();
    }

    [HttpPut("{id:long:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public virtual async Task<IActionResult> UpdateCategory(
        [FromRoute] long id,
        [FromBody] CategoryUpdateRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;

        var validation = await new CategoryUpdateRequestValidator().ValidateAsync(
            request,
            cancellationToken
        );
        if (!validation.IsValid)
        {
            throw new AppRequestInvalidException(
                validation.GetErrorCode(),
                validation.GetErrorMessage(),
                validation.GetErrorField()
            );
        }

        var category = Mapper.Map<Category>(request);
        category.CategoryType = categoryType;

        var response = await categoryService.UpdateAsync(
            category,
            cancellationToken: cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Category),
                    response.Id.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/enable")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> EnableCategory(
        [FromRoute] long id,
        [FromBody] CategoryEnabledRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;

        var validation = await new CategoryEnabledRequestValidator().ValidateAsync(
            request,
            cancellationToken
        );
        if (!validation.IsValid)
        {
            throw new AppRequestInvalidException(
                validation.GetErrorCode(),
                validation.GetErrorMessage(),
                validation.GetErrorField()
            );
        }

        var response = await categoryService.EnableAsync(
            request.Id ?? 0,
            request.IsEnabled,
            cancellationToken: cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Category),
                    response.Id.ToString()
                )
            );
    }

    [HttpDelete("{id:long:min(1)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> DeleteCategory(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var response = await categoryService.DeleteAsync(id, cancellationToken: cancellationToken);

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityDeletionAlert(
                    nameof(Category),
                    response.Id.ToString()
                )
            );
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CategoryResponse>), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> GetAllCategories(
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var page = await categoryService.FindAllByTypeAsync(
            pageable,
            categoryType,
            cancellationToken
        );
        var categories = page.Content;
        var response = Mapper.Map<List<CategoryResponse>>(categories);
        var headers = page.GeneratePaginationHttpHeaders();

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> GetCategory(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var category = await categoryService.FindByIdAsync(id, cancellationToken);
        var response = Mapper.Map<CategoryResponse>(category);

        return ActionResultUtil.WrapOrNotFound(response);
    }
}

[Route("api/media-categories")]
public class MediaCategoriesEndpoint(
    IMapper mapper,
    ICategoryService categoryService,
    ICacheManagementService cacheManagementService
) : BaseCategoriesEndpoint(
    mapper,
    categoryService,
    cacheManagementService,
    CategoryTypes.File
);

[Route("api/option-item-categories")]
public class OptionItemCategoriesEndpoint(
    IMapper mapper,
    ICategoryService categoryService,
    ICacheManagementService cacheManagementService
) : BaseCategoriesEndpoint(
    mapper,
    categoryService,
    cacheManagementService,
    CategoryTypes.OptionItem
);

[Route("api/room-group-categories")]
public class RoomCategoriesEndpoint(
    IMapper mapper,
    ICategoryService categoryService,
    ICacheManagementService cacheManagementService
) : BaseCategoriesEndpoint(
    mapper,
    categoryService,
    cacheManagementService,
    CategoryTypes.RoomGroup
);

[Route("api/plan-categories")]
public class PlanCategoriesEndpoint(
    IMapper mapper,
    ICategoryService categoryService,
    ICacheManagementService cacheManagementService
) : BaseCategoriesEndpoint(
    mapper,
    categoryService,
    cacheManagementService,
    CategoryTypes.Plan
);
