using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;

public class PersonAgeTypeMeta
{
    /// <summary>
    /// あすなろで扱う、大人、子供(小学生高学年・低学年)、幼児、乳児の区分け
    /// </summary>
    public PersonAgeGroups PersonAgeGroup { get; set; }

    /// <summary>
    /// 食事布団あり・なしの組み合わせ
    /// </summary>
    public FoodBeds FoodBed { get; set; }
}
