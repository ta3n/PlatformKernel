using Liberty.ApplicationShared.Utils;
using Liberty.Cache.Services;
using Liberty.Reservation.Manager.Application.Auth;
using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class QuestionService(
    ILogger<QuestionService> logger,
    ICacheService cacheService,
    IUnitOfWork unitOfWork,
    ISecurityContextAccessor securityContextAccessor,
    IQuestionRepository questionRepository,
    IFacilityQuestionRepository facilityQuestionRepository
) : BaseService<Question>(logger, cacheService, questionRepository, new QuestionNotfoundException()), IQuestionService
{
    protected override string GetCacheKey(
        string methodName = "",
        params string[] keys
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;
        return $"{string.Format(CacheKeys.FacilityPrefixKey, facilityId)}:" + base.GetCacheKey(methodName, keys);
    }

    protected override IQueryable<Question> GetQueryable()
    {
        var facilityId = securityContextAccessor.FacilityKey;

        return base.GetQueryable()
            .Where(
                x => x.FacilityQuestions!.Any(
                    t => t.FacilityId == facilityId
                )
            );
    }

    public override async Task<Question> CreateAsync(
        Question entityToCreate,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        entityToCreate.Code = EntityUtil.CreateCode();
        entityToCreate.RecordMemo = EntityUtil.CreateRecordMemo();

        autoSave = false;

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var newQuestion = await base.CreateAsync(
                entityToCreate,
                autoSave,
                cancellationToken
            );

            var newFacilityQuestion = new FacilityQuestion
            {
                FacilityId = facilityId,
                Question = newQuestion
            };

            await facilityQuestionRepository.AddAsync(
                newFacilityQuestion,
                autoSave,
                cancellationToken
            );

            await unitOfWork.CommitAsync(cancellationToken);

            return newQuestion;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Create question failed: {Message}", ex.Message);
            await unitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }

    public override Task<Question> UpdateAsync(
        Question entityToUpdate,
        bool autoSave = true,
        Func<Question, Question, Question>? updateAction = null,
        CancellationToken cancellationToken = default
    )
    {
        var updateHandle = updateAction
            ?? ((
                existingEntity,
                updateEntity
            ) =>
            {
                existingEntity.Name ??= [];
                existingEntity.Name.UpdateLocalized(updateEntity.Name);
                existingEntity.Description ??= [];
                existingEntity.Description.UpdateLocalized(updateEntity.Description);
                existingEntity.FormData ??= [];
                existingEntity.FormData.UpdateLocalized(updateEntity.FormData);
                existingEntity.QuestionType = updateEntity.QuestionType;

                return existingEntity;
            });

        return base.UpdateAsync(
            entityToUpdate,
            autoSave,
            updateHandle,
            cancellationToken
        );
    }

    public override async Task<Question> DeleteAsync(
        long id,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        autoSave = false;

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var removeQuestion = await base.DeleteAsync(
                id,
                autoSave,
                cancellationToken
            );

            await facilityQuestionRepository.DeleteAsync(
                new FacilityQuestion
                {
                    FacilityId = facilityId,
                    QuestionId = id
                },
                autoSave,
                cancellationToken
            );

            await unitOfWork.CommitAsync(cancellationToken);

            return removeQuestion;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Update question failed: {Message}", ex.Message);
            await unitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }

    public override async Task<IPage<Question>> FindAllAsync(
        IPageable pageable,
        CancellationToken cancellationToken = default
    )
    {
        var key = JsonConvert.SerializeObject(
            pageable,
            JsonSettings.Optimized
        );

        var cacheKey = GetCacheKey(
            nameof(FindAllAsync),
            key
        );
        IPage<Question>? page = null;
        if (CacheService is not null)
        {
            page = await CacheService.GetAsync<Page<Question>>(
                cacheKey,
                cancellationToken
            );
        }

        if (page is not null)
        {
            return page;
        }

        page = await GetQueryable()
            .Include(x => x.PlanQuestions!)
            .ThenInclude(x => x.Plan)
            .Include(x => x.OptionItemQuestions)
            .OrderByDescending(x => x.DisplayOrder)
            .UsePageableAsync(
                pageable,
                cancellationToken: cancellationToken
            );

        if (CacheService is not null)
        {
            await CacheService.SetAsync(
                cacheKey,
                page,
                cancellationToken
            );
        }

        return page;
    }
}
