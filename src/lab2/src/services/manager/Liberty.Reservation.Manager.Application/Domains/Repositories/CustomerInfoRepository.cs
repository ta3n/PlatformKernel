namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class CustomerInfoRepository(
    ManagerDataContext dbContext
) : RepositoryBase<CustomerInfo>(dbContext), ICustomerInfoRepository;
