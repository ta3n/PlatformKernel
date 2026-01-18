namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record BathingTaxAgeResponse(
    bool UseSpaTax,
    string? SpaTaxComment,
    string? SpaTaxTable
)
{
    public bool PlanHasData { get; set; }
    public IEnumerable<BathingTaxAgeDetailsResponse> BathingTaxAges { get; set; } = [];
};

public record BathingTaxAgeDetailsResponse
{
    public long Id { get; init; }
    public string? Name { get; init; }
    public int? AgeMax { get; init; }
    public int? AgeMin { get; init; }
    public bool IsMain { get; init; }
    public bool IsEnabled { get; init; }
    public bool IsVisible { get; init; }
    public long? DisplayOrder { get; init; }
    public MetaOfBathingTaxAgeResponse? Meta { get; init; }
    public IEnumerable<SpaOfBathingTaxAgeResponse>? Spas { get; init; }
}

public record MetaOfBathingTaxAgeResponse(
    string? GroupName,
    FoodBeds Bed,
    FoodBeds Food,
    PersonAgeGroups PersonAgeGroup
);

public record SpaOfBathingTaxAgeResponse
{
    public int? PriceMin { get; set; }
    public int? PriceMax { get; set; }
    public int? Tax { get; set; }
}
