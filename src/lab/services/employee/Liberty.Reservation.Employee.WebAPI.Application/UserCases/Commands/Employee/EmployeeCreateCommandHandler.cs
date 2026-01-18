using AutoMapper;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Employee.Application.Contexts.Entities;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.Application.Exceptions;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Employee;

public class EmployeeCreateCommandHandler(
    ILogger<EmployeeCreateCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    UserManager<Reservation.Employee.Application.Contexts.Entities.Employee> userManager,
    IEmployeeService employeeService,
    IAddressService addressService,
    IEmployeeMetaService employeeMetaService
) : CreateCommandHandlerBase<EmployeeCreateCommand, string>(
    unitOfWork,
    mapper
)
{
    protected override async Task<string> HandleAsync(
        EmployeeCreateCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        if (await userManager.FindByNameAsync(payload.Name) is not null)
        {
            throw new LoginAlreadyUsedException();
        }

        if (await userManager.FindByEmailAsync(payload.Email) is not null)
        {
            throw new EmailAlreadyUsedException();
        }

        var newEmployeeId = string.Empty;
        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken);

            var newAddress = await addressService.CreateAddress(
                Mapper.Map<Address>(payload)
            );

            var newEmployeeMeta = await employeeMetaService.CreateEmployeeMeta(
                Mapper.Map<EmployeeMeta>(payload)
            );

            var (newEmployee, _) = await employeeService.Create(
                payload.Name,
                payload.Email
            );

            newEmployee.Address = newAddress;
            newEmployee.EmployeeMeta = newEmployeeMeta;

            await UnitOfWork.Set<Reservation.Employee.Application.Contexts.Entities.Employee>()
                .AddAsync(
                    newEmployee,
                    cancellationToken
                );

            await UnitOfWork.CommitAsync(cancellationToken);

            newEmployeeId = newEmployee.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Create employee failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
        }

        return newEmployeeId;
    }
}
