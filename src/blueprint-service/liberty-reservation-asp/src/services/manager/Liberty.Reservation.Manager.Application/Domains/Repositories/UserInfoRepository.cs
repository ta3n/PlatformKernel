namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class UserInfoRepository(
    ManagerDataContext dbContext
) : RepositoryBase<CustomerInfo>(dbContext), IUserInfoRepository;
