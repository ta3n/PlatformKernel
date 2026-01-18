namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class FacilityQuestionService(
    ILogger<FacilityQuestionService> logger,
    IFacilityQuestionRepository facilityQuestionRepository
) : BaseServiceRelation<FacilityQuestion>(logger, facilityQuestionRepository), IFacilityQuestionService
{
    public async Task<IPage<FacilityQuestion>> FindAllAsync(
        long facilityId,
        IPageable pageable,
        CancellationToken cancellationToken = default
    )
    {
        var page = await facilityQuestionRepository
            .GetQueryable()
            .Select(
                data => new FacilityQuestion
                {
                    Question = data.Question,
                    Facility = new Facility
                    {
                        Id = data.Facility!.Id,
                        Code = data.Facility.Code
                    },
                    IsDeleted = data.IsDeleted,
                    QuestionId = data.QuestionId,
                    FacilityId = data.FacilityId
                }
            )
            .Where(x => x.FacilityId == facilityId)
            .UsePageableAsync(pageable, cancellationToken: cancellationToken);

        return page;
    }

    public async Task<FacilityQuestion> FindByIdAsync(
        long id,
        CancellationToken cancellationToken = default
    )
    {
        return await facilityQuestionRepository
                .GetQueryableWithAsNoTracking()
                .Where(x => x.QuestionId == id)
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new FacilityQuestionNotFoundException();
    }

    public async Task<long> CreateFacilityQuestionAsync(
        long facilityId,
        Question question,
        CancellationToken cancellationToken = default
    )
    {
        question.Code = Guid.NewGuid().ToString();
        var facilityQuestion = new FacilityQuestion
        {
            Question = question,
            FacilityId = facilityId
        };
        var response = await CreateAsync(facilityQuestion, true, cancellationToken);
        return response.QuestionId;
    }

    public async Task<long> UpdateFacilityQuestionAsync(
        long id,
        Question question,
        CancellationToken cancellationToken = default
    )
    {
        var facilityQuestion = await FindByQuestionIdAsync(
            question.Id,
            cancellationToken
        );
        facilityQuestion.Question!.Name = question.Name;
        facilityQuestion.Question.Description = question.Description;
        facilityQuestion.Question.FormData = question.FormData;

        return facilityQuestion.QuestionId;
    }

    public async Task EnableAsync(
        long id,
        bool isEnabled,
        CancellationToken cancellationToken = default
    )
    {
        var facilityQuestion = await FindByQuestionIdAsync(
            id,
            cancellationToken
        );
        facilityQuestion.IsEnabled = isEnabled;
        await UpdateAsync(facilityQuestion, cancellationToken: cancellationToken);
    }

    public async Task<bool> DeleteAsync(
        long id,
        CancellationToken cancellationToken = default
    )
    {
        var facilityQuestion = await FindByQuestionIdAsync(
            id,
            cancellationToken
        );
        await DeleteAsync(facilityQuestion, cancellationToken: cancellationToken);

        return true;
    }

    private async Task<FacilityQuestion> FindByQuestionIdAsync(
        long id,
        CancellationToken cancellationToken = default
    )
    {
        return await facilityQuestionRepository
                .GetQueryable()
                .Include(x => x.Question)
                .Where(x => x.QuestionId == id)
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new FacilityQuestionNotFoundException();
    }
}
