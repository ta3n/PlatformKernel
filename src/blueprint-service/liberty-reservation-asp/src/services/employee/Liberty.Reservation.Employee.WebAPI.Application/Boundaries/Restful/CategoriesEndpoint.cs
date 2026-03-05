using Liberty.Pagination;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful.Base;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Category;
using Liberty.Reservation.Employee.WebAPI.Application.Validations;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;
using Liberty.SysException.Exceptions;
using MediatR;

namespace Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;

[Route("")]
public abstract class BaseCategoriesEndpoint(
    IMapper mapper,
    ICategoryService categoryService,
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
        category.IsMaster = true;

        var response = await categoryService.CreateAsync(category, cancellationToken: cancellationToken);

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
        var validation = await new ItemUpdateOrderRequestValidator().ValidateAsync(
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

        return NoContent();
    }

    [HttpPut("{id:long:min(1)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
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

        var response = await categoryService.UpdateAsync(category, cancellationToken: cancellationToken);

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
        var category = await categoryService.FindByIdAsync(
            id,
            categoryType,
            cancellationToken
        );
        var response = Mapper.Map<CategoryResponse>(category);

        return ActionResultUtil.WrapOrNotFound(response);
    }
}

[Route("api/facility-categories")]
public class FacilityCategoriesEndpoint(
    IMapper mapper,
    IMediator mediator,
    ICategoryService categoryService
) : BaseCategoriesEndpoint(
    mapper,
    categoryService,
    CategoryTypes.Facility
)
{
    [HttpPatch("{id:long:min(1)}/enable")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public override async Task<IActionResult> EnableCategory(
        [FromRoute] long id,
        [FromBody] CategoryEnabledRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;

        var response = await mediator.Send(
            new CategoryEnableCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Category),
                    response.ToString()
                )
            );
    }
}

[Route("api/facility-feature-categories")]
public class FacilityFeatureCategoriesEndpoint(
    IMapper mapper,
    IMediator mediator,
    ICategoryService categoryService
) : BaseCategoriesEndpoint(
    mapper,
    categoryService,
    CategoryTypes.FacilityFeature
)
{
    [HttpPatch("{id:long:min(1)}/enable")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public override async Task<IActionResult> EnableCategory(
        [FromRoute] long id,
        [FromBody] CategoryEnabledRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;

        var response = await mediator.Send(
            new CategoryEnableCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Category),
                    response.ToString()
                )
            );
    }
}

[Route("api/facility-equipment-categories")]
public class FacilityEquipmentCategoriesEndpoint(
    IMapper mapper,
    IMediator mediator,
    ICategoryService categoryService
) : BaseCategoriesEndpoint(
    mapper,
    categoryService,
    CategoryTypes.FacilityEquipment
)
{
    [HttpPatch("{id:long:min(1)}/enable")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public override async Task<IActionResult> EnableCategory(
        [FromRoute] long id,
        [FromBody] CategoryEnabledRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;

        var response = await mediator.Send(
            new CategoryEnableCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Category),
                    response.ToString()
                )
            );
    }
}

[Route("api/room-group-categories")]
public class RoomGroupCategoriesEndpoint(
    IMapper mapper,
    ICategoryService categoryService
) : BaseCategoriesEndpoint(
    mapper,
    categoryService,
    CategoryTypes.RoomGroup
);

[Route("api/room-group-feature-categories")]
public class RoomGroupFeatureCategoriesEndpoint(
    IMapper mapper,
    ICategoryService categoryService
) : BaseCategoriesEndpoint(
    mapper,
    categoryService,
    CategoryTypes.RoomGroupFeature
);

[Route("api/room-group-equipment-categories")]
public class RoomGroupEquipmentCategoriesEndpoint(
    IMapper mapper,
    IMediator mediator,
    ICategoryService categoryService
) : BaseCategoriesEndpoint(
    mapper,
    categoryService,
    CategoryTypes.RoomGroupEquipment
)
{
    [HttpPatch("{id:long:min(1)}/enable")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public override async Task<IActionResult> EnableCategory(
        [FromRoute] long id,
        [FromBody] CategoryEnabledRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;
        var response = await mediator.Send(
            new CategoryEnableCommand { Payload = request },
            cancellationToken
        );
        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Category),
                    response.ToString()
                )
            );
    }
};

[Route("api/leisure-categories")]
public class LeisureCategoriesEndpoint(
    IMapper mapper,
    IMediator mediator,
    ICategoryService categoryService
) : BaseCategoriesEndpoint(
    mapper,
    categoryService,
    CategoryTypes.Leisure
)
{
    [HttpPatch("{id:long:min(1)}/enable")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public override async Task<IActionResult> EnableCategory(
        [FromRoute] long id,
        [FromBody] CategoryEnabledRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;

        var response = await mediator.Send(
            new CategoryEnableCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Category),
                    response.ToString()
                )
            );
    }
}

[Route("api/spa-categories")]
public class SpaCategoriesEndpoint(
    IMapper mapper,
    IMediator mediator,
    ICategoryService categoryService
) : BaseCategoriesEndpoint(
    mapper,
    categoryService,
    CategoryTypes.Spa
)
{
    [HttpPatch("{id:long:min(1)}/enable")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public override async Task<IActionResult> EnableCategory(
        [FromRoute] long id,
        [FromBody] CategoryEnabledRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;

        var response = await mediator.Send(
            new CategoryEnableCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Category),
                    response.ToString()
                )
            );
    }
}

[Route("api/view-categories")]
public class ViewCategoriesEndpoint(
    IMapper mapper,
    IMediator mediator,
    ICategoryService categoryService
) : BaseCategoriesEndpoint(
    mapper,
    categoryService,
    CategoryTypes.View
)
{
    [HttpPatch("{id:long:min(1)}/enable")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public override async Task<IActionResult> EnableCategory(
        [FromRoute] long id,
        [FromBody] CategoryEnabledRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;

        var response = await mediator.Send(
            new CategoryEnableCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Category),
                    response.ToString()
                )
            );
    }
}

[Route("api/plan-categories")]
public class PlanCategoriesEndpoint(
    IMapper mapper,
    ICategoryService categoryService
) : BaseCategoriesEndpoint(
    mapper,
    categoryService,
    CategoryTypes.Plan
);

[Route("api/amenity-categories")]
public class AmenityCategoriesEndpoint(
    IMapper mapper,
    IMediator mediator,
    ICategoryService categoryService
) : BaseCategoriesEndpoint(
    mapper,
    categoryService,
    CategoryTypes.Amenity
)
{
    [HttpPatch("{id:long:min(1)}/enable")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public override async Task<IActionResult> EnableCategory(
        [FromRoute] long id,
        [FromBody] CategoryEnabledRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;

        var response = await mediator.Send(
            new CategoryEnableCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Category),
                    response.ToString()
                )
            );
    }
}

[Route("api/option-item-categories")]
public class OptionItemCategoriesEndpoint(
    IMapper mapper,
    ICategoryService categoryService
) : BaseCategoriesEndpoint(
    mapper,
    categoryService,
    CategoryTypes.OptionItem
);

[Route("api/file-categories")]
public class FileCategoriesEndpoint(
    IMapper mapper,
    ICategoryService categoryService
) : BaseCategoriesEndpoint(
    mapper,
    categoryService,
    CategoryTypes.File
);

[Route("api/meal-type-categories")]
public class MealTypeCategoriesEndpoint(
    IMapper mapper,
    IMediator mediator,
    ICategoryService categoryService
) : BaseCategoriesEndpoint(
    mapper,
    categoryService,
    CategoryTypes.MealType
)
{
    [HttpPatch("{id:long:min(1)}/enable")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public override async Task<IActionResult> EnableCategory(
        [FromRoute] long id,
        [FromBody] CategoryEnabledRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;

        var response = await mediator.Send(
            new CategoryEnableCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Category),
                    response.ToString()
                )
            );
    }
}
