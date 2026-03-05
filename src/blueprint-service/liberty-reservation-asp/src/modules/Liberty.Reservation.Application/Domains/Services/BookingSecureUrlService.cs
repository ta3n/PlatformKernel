using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Application.Settings;
using Microsoft.Extensions.Options;

namespace Liberty.Reservation.Application.Domains.Services;

public class BookingSecureUrlService(
    IOptions<SecretKeySetting> secretKeySetting
) : IBookingSecureUrlService
{
    public string EncryptDataWithHmacSha256(
        BookingSecureUrlRequest request
    )
    {
        // Create a data string with an expiration time
        const char splitChar = '|';
        var expirationTime = DateTime.UtcNow
            .AddMinutes(request.ValidMinutes < 0 ? 1 : request.ValidMinutes)
            .AddMinutes(30)
            .ToString("o"); // ISO 8601 format
        var rawData = $"{request.BookingId}{splitChar}{expirationTime}";

        // Generate HMAC signature
        var signature = GenerateHmac(rawData);

        // Combine data and signature
        return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{rawData}|{signature}"));
    }

    public BookingSecureUrlResponse DecryptAndValidate(
        string encryptedData
    )
    {
        const char splitChar = '|';

        try
        {
            // Decode Base64
            var decodedData = Encoding.UTF8.GetString(Convert.FromBase64String(encryptedData));
            var parts = decodedData.Split('|');

            // Get original data and signature
            var dataWithoutSignature = string.Join(
                splitChar,
                parts,
                0,
                parts.Length - 1
            );

            var signature = parts[^1]; // Signature at the end
            if (!ValidateHmac(dataWithoutSignature, signature))
            {
                throw new GuestSecretCodeIncorrectException();
            }

            // Get the expiration time
            var expirationTime = parts[^2];
            var expiration = DateTime.Parse(expirationTime, new CultureInfo("en-US"));
            if (DateTime.UtcNow > expiration)
            {
                return new(false, null); // The URL has expired
            }

            // Valid data
            var rawDataParts = dataWithoutSignature.Split(splitChar);
            var request = new BookingSecureUrlRequest(
                rawDataParts[0],
                expiration.Minute
            );

            return new(true, request);
        }
        catch
        {
            // return new(false, null); // Invalid data
            throw new GuestSecretCodeIncorrectException();
        }
    }

    /// <summary>
    /// // HMAC secret key
    /// </summary>
    /// <returns></returns>
    private byte[] GetKey()
    {
        return Encoding.UTF8.GetBytes(secretKeySetting.Value.HmacSecretKey ?? string.Empty);
    }

    /// <summary>
    /// HMAC signature generator function
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private string GenerateHmac(
        string data
    )
    {
        var key = GetKey();
        using var hmac = new HMACSHA256(key);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));

        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// Function to check HMAC signature
    /// </summary>
    /// <param name="data"></param>
    /// <param name="signature"></param>
    /// <returns></returns>
    private bool ValidateHmac(
        string data,
        string signature
    )
    {
        var key = GetKey();
        using var hmac = new HMACSHA256(key);
        var computedSignature = Convert.ToBase64String(
            hmac.ComputeHash(Encoding.UTF8.GetBytes(data))
        );

        return signature == computedSignature;
    }
}
