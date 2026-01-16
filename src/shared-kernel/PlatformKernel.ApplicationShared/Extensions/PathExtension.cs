using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace PlatformKernel.ApplicationShared.Extensions;

public static class PathExtension
{
    /// <summary>
    /// Checks if a path matches a glob pattern with * and ? wildcards.
    /// </summary>
    /// <param name="path">The path to check.</param>
    /// <param name="pattern">The glob pattern with * and ? wildcards.</param>
    /// <returns>True if the path matches the pattern, otherwise false.</returns>
    /// <example>
    /// Examples of valid patterns:
    /// - Pattern: "/api/*", Path: "/api/users" -> Match: True
    /// - Pattern: "/api/?.*", Path: "/api/u123" -> Match: True
    /// - Pattern: "/static/*.css", Path: "/static/style.css" -> Match: True
    /// - Pattern: "/static/*.css", Path: "/static/js/script.js" -> Match: False
    /// </example>
    public static bool MatchesPattern(
        this PathString path,
        string pattern
    )
    {
        var pathString = path.ToString();
        var patternString = pattern;

        // Unified handling for all patterns
        // Create a regular expression from the glob pattern
        // Where * matches any sequence of characters, including '/'
        var regexPattern = "^"
            + Regex.Escape(patternString)
                .Replace("\\*", ".*") // * matches any sequence of characters; inclusion of '/' depends on the pattern
                .Replace("\\?", ".") // ? matches any single character
            + "$";

        return Regex.IsMatch(
            pathString,
            regexPattern,
            RegexOptions.IgnoreCase | RegexOptions.NonBacktracking
        );
    }
}
