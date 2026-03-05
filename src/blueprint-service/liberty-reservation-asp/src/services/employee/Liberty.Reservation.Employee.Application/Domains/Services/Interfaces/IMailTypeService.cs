namespace Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;

public interface IMailTypeService
{
    IEnumerable<(string IoType, string Name)> FindAll();
}
