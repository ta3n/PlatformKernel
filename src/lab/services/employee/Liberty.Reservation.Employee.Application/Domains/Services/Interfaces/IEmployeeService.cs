using Liberty.Reservation.Employee.Application.Constants;
using Liberty.Reservation.Employee.Application.Contexts.Entities.Relations;

namespace Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;

public interface IEmployeeService
{
    Task<(int, Contexts.Entities.Employee[])> GetMany(
        string? employeeCode,
        string? name,
        string? email,
        int? take,
        int? skip
    );

    Task<(Contexts.Entities.Employee, string)> Create(
        string name,
        string email
    );

    Task<Contexts.Entities.Employee> GetCreatedUser(
        string email
    );

    Task<Contexts.Entities.Employee> GetMailConfirmedUser(
        string email
    );

    Task<(string, string, string)> GenerateEmailConfirmationTokenAsync(
        Contexts.Entities.Employee data,
        DateTime expired
    );

    Task<(string, string, string)> GeneratePasswordResetTokenAsync(
        Contexts.Entities.Employee data,
        DateTime expired
    );

    Task ConfirmEmailAsync(
        Contexts.Entities.Employee data,
        string token
    );

    Task ResetPasswordAsync(
        Contexts.Entities.Employee data,
        string token,
        string password
    );

    Task UpdateEmail(
        string employeeCode,
        string email
    );

    Task UpdatePassword(
        string employeeCode,
        string email
    );

    Task Delete(
        string employeeCode
    );

    Task<(int, FileEmployee[])> GetFileMany(
        string employeeCode,
        string? fileCode,
        int? take,
        int? skip
    );

    Task<FileEmployee> CreateFile(
        string employeeCode,
        string fileCode,
        string secret,
        EmployeeFileTypes type
    );

    Task DeleteFile(
        string employeeCode,
        string fileCode,
        string secret
    );

    Task<FileEmployee> CreateAvatar(
        string employeeCode,
        string fileCode,
        string secret
    );

    Task DeleteAvatar(
        string employeeCode
    );
}
