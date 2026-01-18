using Liberty.Reservation.Employee.Application.Contexts.Entities;

namespace Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;

public interface IAddressService
{
    Task<Address> CreateAddress(
        Address addressToCreate
    );

    Task<Address> UpdateAddress(
        Address addressToUpdate
    );
}
