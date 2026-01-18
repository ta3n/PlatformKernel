using Liberty.Reservation.Site.Application.Exceptions;

namespace Liberty.Reservation.Site.Application.Domains.Services;

public class QuestionService(
    ILogger<QuestionService> logger,
    IQuestionRepository questionRepository
) : BaseService<Question>(
        logger,
        questionRepository,
        new QuestionNotfoundException()
    ),
    IQuestionService;
