namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record PersonAgeTypeResponse
{
    public long Id { get; init; }
    public string? Name { get; set; }
    public int? AgeMax { get; init; }
    public int? AgeMin { get; init; }
    public bool IsMain { get; init; }
    public bool IsEnabled { get; init; }
    public bool IsVisible { get; init; }
    public MetaOfPersonAgeTypeResponse? Meta { get; init; }
    public IEnumerable<SpaOfPersonAgeTypeResponse>? Spas { get; init; }

    public record MetaOfPersonAgeTypeResponse
    {
        public string? GroupName { get; set; }
        public FoodBeds Bed { get; set; }
        public FoodBeds Food { get; set; }
        public long PersonAgeGroup { get; set; }
    };

    public record SpaOfPersonAgeTypeResponse
    {
        public long Id { get; set; }
        public int? PriceMin { get; set; }
        public int? PriceMax { get; set; }
        public int? Tax { get; set; }
    };
}
