using System.Text;

namespace SharedKernel.BulkInsertOther.Utilities;

internal static class BulkInsertNamingPolicy
{
    public static string ToSnakeCase(
        string input
    )
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        var builder = new StringBuilder(input.Length + 8);

        for (var index = 0; index < input.Length; index++)
        {
            var character = input[index];
            if (char.IsUpper(character))
            {
                if (index > 0)
                {
                    builder.Append('_');
                }

                builder.Append(char.ToLowerInvariant(character));
                continue;
            }

            builder.Append(character);
        }

        return builder.ToString();
    }
}
