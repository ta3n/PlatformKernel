using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.Application.Domains.Services.Interfaces;

/// <summary>
/// Provides methods for securely generating, decrypting, and validating booking URLs,
/// as well as retrieving reservation data based on a booking code.
/// </summary>
public interface IBookingSecureUrlService
{
    /// <summary>
    /// Encrypts data using time-based validation and an HMAC SHA-256 signature.
    /// </summary>
    /// <param name="request">The data containing the booking ID and validation time in minutes.</param>
    /// <returns>A string representing the encrypted data with an HMAC signature.</returns>
    string EncryptDataWithHmacSha256(
        BookingSecureUrlRequest request
    );

    /// <summary>
    /// Decrypts the provided encrypted data and validates its content.
    /// </summary>
    /// <param name="encryptedData">The encrypted data to be decrypted and validated.</param>
    /// <returns>A response object containing the validation result and the associated booking information.</returns>
    BookingSecureUrlResponse DecryptAndValidate(
        string encryptedData
    );
}
