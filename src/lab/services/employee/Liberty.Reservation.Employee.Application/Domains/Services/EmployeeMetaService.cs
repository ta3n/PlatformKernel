using Liberty.Reservation.Employee.Application.Contexts.Entities;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Employee.Application.Domains.Services;

public class EmployeeMetaService(
    ILogger<EmployeeMetaService> logger,
    IEmployeeMetaRepository employeeMetaRepository
) : IEmployeeMetaService
{
    public async Task<EmployeeMeta> CreateEmployeeMeta(
        EmployeeMeta employeeMetaToCreate
    )
    {
        var employeeMeta = new EmployeeMeta
        {
            Code = Guid.NewGuid().ToString(),
            Name = employeeMetaToCreate.Name?.Trim(),
            Kana = employeeMetaToCreate.Kana?.Trim(),
            Gender = employeeMetaToCreate.Gender?.Trim(),
            Birthday = employeeMetaToCreate.Birthday
        };
        await employeeMetaRepository.AddAsync(employeeMeta);

        logger.LogDebug("Created information for address: {Code}", employeeMeta.Code);

        return employeeMeta;
    }

    public async Task<EmployeeMeta> UpdateEmployeeMeta(
        EmployeeMeta employeeMetaToUpdate
    )
    {
        var existingEmployeeMeta = await employeeMetaRepository.GetByIdAsync(
            employeeMetaToUpdate.Id
        ) ?? throw new EmployeeMetaNotfoundException();

        existingEmployeeMeta.Name = employeeMetaToUpdate.Name;
        existingEmployeeMeta.Kana = employeeMetaToUpdate.Kana;
        existingEmployeeMeta.Gender = employeeMetaToUpdate.Gender;
        existingEmployeeMeta.Birthday = employeeMetaToUpdate.Birthday;

        employeeMetaRepository.Update(existingEmployeeMeta);

        logger.LogDebug("Updated information for address: {Code}", existingEmployeeMeta.Code);

        return existingEmployeeMeta;
    }
}
