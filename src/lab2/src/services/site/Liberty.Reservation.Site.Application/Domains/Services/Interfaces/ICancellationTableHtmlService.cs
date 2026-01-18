namespace Liberty.Reservation.Site.Application.Domains.Services.Interfaces;

internal interface ICancellationTableHtmlService
{
    string GenerateHtmlTable(
        List<CancellationData> data,
        string lang
    );
}
