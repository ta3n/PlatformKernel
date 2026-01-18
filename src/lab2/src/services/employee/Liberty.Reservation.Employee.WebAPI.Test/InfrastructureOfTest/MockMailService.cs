using Liberty.ApplicationShared.Domains.Services.Mails;

namespace Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;

public class MockMailService : IMailService
{
    public Task SendAsync(
        string[] tos,
        string subject,
        string body
    )
    {
        return Task.CompletedTask;
    }

    public Task SendAsync(
        string[] tos,
        string subject,
        string body,
        string? fromDisplayName
    )
    {
        return Task.CompletedTask;
    }
}
