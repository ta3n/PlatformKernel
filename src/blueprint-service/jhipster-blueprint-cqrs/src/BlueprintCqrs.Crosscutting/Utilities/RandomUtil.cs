using System;
using System.Linq;

namespace BlueprintCqrs.Crosscutting.Utilities;

public class RandomUtil
{
    private const int DefCount = 20;
    private static readonly Random Random = new();

    public static string GeneratePassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(
            Enumerable.Repeat(chars, DefCount)
                .Select(s => s[Random.Next(s.Length)])
                .ToArray()
        );
    }

    public static string RandomNumeric(
        int length
    )
    {
        const string chars = "0123456789";
        return new string(
            Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)])
                .ToArray()
        );
    }

    public static string GenerateActivationKey()
    {
        return RandomNumeric(DefCount);
    }

    public static string GenerateResetKey()
    {
        return RandomNumeric(DefCount);
    }
}
