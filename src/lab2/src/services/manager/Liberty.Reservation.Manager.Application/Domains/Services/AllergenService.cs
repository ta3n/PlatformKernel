namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class AllergenService(
    ILogger<AllergenService> logger,
    IAllergenRepository allergenRepository
) : BaseService<Allergen>(logger, allergenRepository, new AllergenNotfoundException()), IAllergenService;
