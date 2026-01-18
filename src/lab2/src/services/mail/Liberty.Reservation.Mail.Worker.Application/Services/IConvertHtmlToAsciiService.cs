namespace Liberty.Reservation.Mail.Worker.Application.Services;

public interface IConvertHtmlToAsciiService
{
    string ConvertAndReplaceTables(
        string input
    );
}
