namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface ICancellationTableHtmlService
{
    string GenerateHtmlTable(
        List<CancellationData> data,
        string lang
    );
}
