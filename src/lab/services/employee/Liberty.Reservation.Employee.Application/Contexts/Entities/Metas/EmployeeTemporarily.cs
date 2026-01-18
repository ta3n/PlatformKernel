namespace Liberty.Reservation.Employee.Application.Contexts.Entities.Metas;

public class EmployeeTemporarily
{
    public EmailConfirmation[] EmailConfirmations = [];
    public PasswordReset[] PasswordResets = [];

    public class EmailConfirmation
    {
        public string? Code { get; set; }
        public string? Token { get; set; }
        public string? Hash { get; set; }
        public DateTime Expired { get; set; }
    }

    public class PasswordReset : EmailConfirmation;
}
