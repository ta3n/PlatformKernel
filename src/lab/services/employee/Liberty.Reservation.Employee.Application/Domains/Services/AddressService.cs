using Liberty.Reservation.Employee.Application.Contexts.Entities;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Employee.Application.Domains.Services;

public class AddressService(
    ILogger<AddressService> logger,
    IAddressRepository addressRepository
) : IAddressService
{
    public async Task<Address> CreateAddress(
        Address addressToCreate
    )
    {
        var address = new Address
        {
            Code = Guid.NewGuid().ToString(),
            Tel = addressToCreate.Tel?.ToLower().Trim(),
            Mobile = addressToCreate.Mobile?.ToLower().Trim()
        };
        await addressRepository.AddAsync(address);

        logger.LogDebug("Created information for address: {Code}", addressToCreate.Code);

        return address;
    }

    public async Task<Address> UpdateAddress(
        Address addressToUpdate
    )
    {
        var existingAddress = await addressRepository.GetByIdAsync(
            addressToUpdate.Id
        ) ?? throw new AddressNotfoundException();

        existingAddress.Tel = addressToUpdate.Tel;
        existingAddress.Mobile = addressToUpdate.Mobile;

        addressRepository.Update(existingAddress);

        logger.LogDebug("Updated information for address: {Code}", existingAddress.Code);

        return existingAddress;
    }
}
