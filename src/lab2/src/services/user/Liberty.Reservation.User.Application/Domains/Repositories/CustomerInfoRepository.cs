using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork.Implementations;

namespace Liberty.Reservation.User.Application.Domains.Repositories;

public class CustomerInfoRepository(
    UserDataContext dbContext
) : RepositoryBase<CustomerInfo>(dbContext), ICustomerInfoRepository;
