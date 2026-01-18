using Liberty.Cache.Services;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;

[Route("api/questions")]
public class QuestionsEndpoint(
    IMapper mapper,
    IQuestionService questionService,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    ICacheManagementService cacheManagementService
) : BaseEndpoint(mapper)
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateQuestion(
        [FromBody] QuestionCreateRequest request,
        CancellationToken cancellationToken
    )
    {
        var validation = await new QuestionCreateRequestValidator().ValidateAsync(
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

        var question = Mapper.Map<Question>(request);
        var response = await questionService.CreateAsync(
            question,
            cancellationToken: cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response.Id)
            .WithHeaders(
                HeaderUtil.CreateEntityCreationAlert(
                    nameof(Question),
                    response.Id.ToString()
                )
            );
    }

    [HttpPut("order")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public virtual async Task<IActionResult> ArrangeOrderOfQuestions(
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

        await questionService.ArrangeOrderAsync(
            request.Ids,
            cancellationToken
        );

        cacheManagementService.RemoveFacilityRelatedCache();

        return NoContent();
    }

    [HttpPut("{id:long:min(1)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateQuestion(
        [FromRoute] long id,
        [FromBody] QuestionUpdateRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;

        var validation = await new QuestionUpdateRequestValidator().ValidateAsync(
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

        var question = Mapper.Map<Question>(request);
        if (!question.HasContent)
        {
            question.IsEnabled = false;
        }

        var response = await questionService.UpdateAsync(
            question,
            cancellationToken: cancellationToken
        );

        var facilityId = securityContextAccessor.FacilityKey;

        await cacheService.ResetAsync(
            string.Format(CacheKeys.ResetPatternSiteBookingSearch, facilityId),
            false,
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Question),
                    response.Id.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/enable")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> EnableQuestion(
        [FromRoute] long id,
        [FromBody] QuestionEnabledRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;

        var validation = await new QuestionEnabledRequestValidator().ValidateAsync(
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

        var existingQuestion = await questionService.FindByIdAsync(
            id,
            cancellationToken
        );
        if (request.IsEnabled && !existingQuestion.HasContent)
        {
            throw new QuestionHasNoContentException();
        }

        var response = await questionService.EnableAsync(
            request.Id ?? 0,
            request.IsEnabled,
            cancellationToken: cancellationToken
        );
        var facilityId = securityContextAccessor.FacilityKey;

        await cacheService.ResetAsync(
            string.Format(CacheKeys.ResetPatternSiteBookingSearch, facilityId),
            false,
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Question),
                    response.Id.ToString()
                )
            );
    }

    [HttpDelete("{id:long:min(1)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteQuestion(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var response = await questionService.DeleteAsync(
            id,
            cancellationToken: cancellationToken
        );

        var facilityId = securityContextAccessor.FacilityKey;
        await cacheService.ResetAsync(
            string.Format(CacheKeys.ResetPatternSiteBookingSearch, facilityId),
            false,
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityDeletionAlert(
                    nameof(Question),
                    response.Id.ToString()
                )
            );
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<QuestionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllQuestions(
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var page = await questionService.FindAllAsync(
            pageable,
            cancellationToken
        );
        var questions = page.Content;
        var response = Mapper.Map<List<QuestionResponse>>(questions);
        var headers = page.GeneratePaginationHttpHeaders();

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}")]
    [ProducesResponseType(typeof(QuestionDetailResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetQuestion(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var question = await questionService.FindByIdAsync(
            id,
            cancellationToken
        );
        var response = Mapper.Map<QuestionDetailResponse>(question);

        return ActionResultUtil.WrapOrNotFound(response);
    }
}
