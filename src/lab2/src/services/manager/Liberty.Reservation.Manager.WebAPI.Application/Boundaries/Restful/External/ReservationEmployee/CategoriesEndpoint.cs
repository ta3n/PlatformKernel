using Liberty.Entity.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.Web.ApiService;

namespace Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful.External.ReservationEmployee;

[ApiExplorerSettings(GroupName = "reservation-employee")]
[Route("")]
public abstract class BaseCategoriesOfEmployeeServiceEndpoint(
    IMapper mapper,
    IMediator mediator,
    IExternalApiService externalApiService,
    string endpoint
) : BaseEndpoint(mapper, mediator)
{
    [HttpGet]
    [ProducesResponseType(typeof(List<CategoryResponse>), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> GetAllCategories(
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var (headers, data) = await externalApiService.GetAsync(
            ExternalService.ReservationEmployeeService,
            $"{endpoint}{HttpContext.Request.QueryString.Value}",
            new Dictionary<string, string> { { "accept-language", LanguageHeaderUtil.GetLanguageCodeFromHeader() } },
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(data).WithHeaders(headers);
    }
}

[Route("api/reservation/employee/facility-categories")]
public class FacilityCategoriesEndpoint(
    IMapper mapper,
    IMediator mediator,
    IExternalApiService externalApiService
) : BaseCategoriesOfEmployeeServiceEndpoint(
    mapper,
    mediator,
    externalApiService,
    "api/facility-categories"
);

[Route("api/reservation/employee/facility-feature-categories")]
public class FacilityFeatureCategoriesEndpoint(
    IMapper mapper,
    IMediator mediator,
    IExternalApiService externalApiService
) : BaseCategoriesOfEmployeeServiceEndpoint(
    mapper,
    mediator,
    externalApiService,
    "api/facility-feature-categories"
);

[Route("api/reservation/employee/facility-equipment-categories")]
public class FacilityEquipmentCategoriesEndpoint(
    IMapper mapper,
    IMediator mediator,
    IExternalApiService externalApiService
) : BaseCategoriesOfEmployeeServiceEndpoint(
    mapper,
    mediator,
    externalApiService,
    "api/facility-equipment-categories"
);

[Route("api/reservation/employee/room-group-categories")]
public class RoomGroupCategoriesEndpoint(
    IMapper mapper,
    IMediator mediator,
    IExternalApiService externalApiService
) : BaseCategoriesOfEmployeeServiceEndpoint(
    mapper,
    mediator,
    externalApiService,
    "api/room-group-categories"
);

[Route("api/reservation/employee/room-group-feature-categories")]
public class RoomGroupFeatureCategoriesEndpoint(
    IMapper mapper,
    IMediator mediator,
    IExternalApiService externalApiService
) : BaseCategoriesOfEmployeeServiceEndpoint(
    mapper,
    mediator,
    externalApiService,
    "api/room-group-feature-categories"
);

[Route("api/reservation/employee/room-group-equipment-categories")]
public class RoomGroupEquipmentCategoriesEndpoint(
    IMapper mapper,
    IMediator mediator,
    IExternalApiService externalApiService
) : BaseCategoriesOfEmployeeServiceEndpoint(
    mapper,
    mediator,
    externalApiService,
    "api/room-group-equipment-categories"
);

[Route("api/reservation/employee/leisure-categories")]
public class LeisureCategoriesEndpoint(
    IMapper mapper,
    IMediator mediator,
    IExternalApiService externalApiService
) : BaseCategoriesOfEmployeeServiceEndpoint(
    mapper,
    mediator,
    externalApiService,
    "api/leisure-categories"
);

[Route("api/reservation/employee/spa-categories")]
public class SpaCategoriesEndpoint(
    IMapper mapper,
    IMediator mediator,
    IExternalApiService externalApiService
) : BaseCategoriesOfEmployeeServiceEndpoint(
    mapper,
    mediator,
    externalApiService,
    "api/spa-categories"
);

[Route("api/reservation/employee/view-categories")]
public class ViewCategoriesEndpoint(
    IMapper mapper,
    IMediator mediator,
    IExternalApiService externalApiService
) : BaseCategoriesOfEmployeeServiceEndpoint(
    mapper,
    mediator,
    externalApiService,
    "api/view-categories"
);

[Route("api/reservation/employee/amenity-categories")]
public class AmenityCategoriesEndpoint(
    IMapper mapper,
    IMediator mediator,
    IExternalApiService externalApiService
) : BaseCategoriesOfEmployeeServiceEndpoint(
    mapper,
    mediator,
    externalApiService,
    "api/amenity-categories"
);

[Route("api/reservation/employee/meal-type-categories")]
public class MealTypeCategoriesEndpoint(
    IMapper mapper,
    IMediator mediator,
    IExternalApiService externalApiService
) : BaseCategoriesOfEmployeeServiceEndpoint(
    mapper,
    mediator,
    externalApiService,
    "api/meal-type-categories"
);

[Route("api/reservation/employee/plan-categories")]
public class PlanCategoriesEndpoint(
    IMapper mapper,
    IMediator mediator,
    IExternalApiService externalApiService
) : BaseCategoriesOfEmployeeServiceEndpoint(
    mapper,
    mediator,
    externalApiService,
    "api/plan-categories"
);

[Route("api/reservation/employee/option-item-categories")]
public class OptionItemCategoriesEndpoint(
    IMapper mapper,
    IMediator mediator,
    IExternalApiService externalApiService
) : BaseCategoriesOfEmployeeServiceEndpoint(
    mapper,
    mediator,
    externalApiService,
    "api/option-item-categories"
);

[Route("api/reservation/employee/file-categories")]
public class FileCategoriesEndpoint(
    IMapper mapper,
    IMediator mediator,
    IExternalApiService externalApiService
) : BaseCategoriesOfEmployeeServiceEndpoint(
    mapper,
    mediator,
    externalApiService,
    "api/file-categories"
);
