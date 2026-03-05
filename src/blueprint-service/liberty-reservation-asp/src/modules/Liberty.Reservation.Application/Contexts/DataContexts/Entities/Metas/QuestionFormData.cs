namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;

public class QuestionFormData
{
    public string? Type { get; set; }

    public List<string>? Data { get; set; }

    public string? Selected { get; set; }

    public bool IsRequired { get; set; }

    public int? SelectionMin { get; set; }

    public int? SelectionMax { get; set; }

    public string? Placeholder { get; set; }

    public string? ValueLabel { get; set; }
}
