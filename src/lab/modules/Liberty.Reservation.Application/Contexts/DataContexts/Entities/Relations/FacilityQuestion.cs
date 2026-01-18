using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class FacilityQuestion : EntityRelation
{
    public long FacilityId { get; set; }
    public Facility? Facility { get; set; }

    public long QuestionId { get; set; }
    public Question? Question { get; set; }

    public FacilityQuestion()
    {
    }

    public FacilityQuestion(
        Facility facility,
        Question question
    )
    {
        Facility = facility;
        Question = question;
    }
}
