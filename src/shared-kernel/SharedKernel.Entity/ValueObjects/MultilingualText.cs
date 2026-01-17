using SharedKernel.Entity.Utils;

namespace SharedKernel.Entity.ValueObjects;

/// <summary>
/// Represents a multilingual text object that extends a dictionary,
/// effectively mapping language codes to their respective translations.
/// This class provides methods to retrieve, update, and manage translations
/// according to language codes.
/// </summary>
public class MultilingualText : Dictionary<string, string>
{
    /// <summary>
    /// Represents a collection of string values keyed by their respective language codes.
    /// </summary>
    /// <remarks>
    /// This class is designed to store and retrieve text values in multiple languages using language codes (e.g., "en" for English, "fr" for French) as keys.
    /// </remarks>
    public MultilingualText()
    {
    }

    /// <summary>
    /// Represents a dictionary-like container for managing multilingual text values,
    /// with additional methods to retrieve values based on a specified language code
    /// or HTTP header information.
    /// </summary>
    public MultilingualText(
        Dictionary<string, string> toDictionary
    ) : base(toDictionary)
    {
    }

    /// <summary>
    /// Retrieves the text value associated with the specified language code.
    /// </summary>
    /// <param name="languageCode">The language code used to look up the associated text value.</param>
    /// <returns>The text value associated with the specified language code if found; otherwise, an empty string.</returns>
    public string GetValueByCode(
        string languageCode
    )
    {
        if (TryGetValue(languageCode, out var value) && !string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var firstKey = Keys.FirstOrDefault();
        if (!string.IsNullOrEmpty(firstKey) && TryGetValue(firstKey, out var firstValue))
        {
            return firstValue;
        }

        return string.Empty;
    }

    /// Retrieves the value associated with a specific language code determined by the language header.
    /// If the language code is not found or the header is empty, the value for the first key in the dictionary is returned.
    /// <return>
    /// The value corresponding to the resolved language code or the value of the first key if no match is found.
    /// </return>
    public string GetValueByHeader()
    {
        var languageCode = LanguageHeaderUtil.GetLanguageCodeFromHeader();

        if (string.IsNullOrEmpty(languageCode))
        {
            languageCode = Keys.First();
        }

        return GetValueByCode(languageCode);
    }

    /// Retrieves a value based on the current language code obtained from the header.
    /// If a value is not found or valid for the language code, returns the default value.
    /// <param name="defaultCode">The default language code to use if no value is found for the current language.</param>
    /// <returns>The string value corresponding to the current language code, or the value for the default language code if unavailable.</returns>
    public string GetValueByHeader(
        string defaultCode
    )
    {
        var languageCode = LanguageHeaderUtil.GetLanguageCodeFromHeader();
        if (TryGetValue(languageCode, out var value) && value is { Length: > 0 })
        {
            return value;
        }

        return GetValueByCode(defaultCode);
    }

    /// Updates the multilingual text values using the provided dictionary of updates for the specified language code.
    /// If the language code is not provided, it will default to the value returned from the language header utility.
    /// If the key already exists, its value is updated; otherwise, a new entry is added with the specified language code.
    /// <param name="updated">Dictionary containing updated multilingual text values.</param>
    /// <param name="languageCode">Optional language code to indicate which language's value should be updated. Defaults to the current language from the header if null.</param>
    public void UpdateLocalized(
        Dictionary<string, string>? updated,
        string? languageCode = null
    )
    {
        if (string.IsNullOrEmpty(languageCode))
        {
            languageCode = LanguageHeaderUtil.GetLanguageCodeFromHeader();
        }

        if (TryGetValue(languageCode, out _))
        {
            this[languageCode] = updated?[languageCode] ?? string.Empty;
        }
        else
        {
            var value = string.Empty;
            var isExisting = updated?.ContainsKey(languageCode) ?? false;
            if (isExisting)
            {
                value = updated![languageCode];
            }

            Add(languageCode, value);
        }
    }
}
