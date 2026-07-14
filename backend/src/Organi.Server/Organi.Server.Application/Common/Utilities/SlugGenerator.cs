using System.Text;
using System.Text.RegularExpressions;

namespace Organi.Server.Application.Common.Utilities;

public static partial class SlugGenerator
{
    public static string Generate(string input)
    {
        var normalized = input.Trim().ToLowerInvariant();
        var builder = new StringBuilder(normalized.Length);

        foreach (var c in normalized)
        {
            if (char.IsLetterOrDigit(c))
                builder.Append(c);
            else if (builder.Length > 0 && builder[^1] != '-')
                builder.Append('-');
        }

        var slug = builder.ToString().Trim('-');
        slug = CollapseHyphensRegex().Replace(slug, "-");

        return slug.Length == 0 ? "n-a" : slug;
    }

    public static async Task<string> GenerateUniqueAsync(string input, Func<string, Task<bool>> slugExistsAsync)
    {
        var baseSlug = Generate(input);
        var slug = baseSlug;
        var suffix = 2;

        while (await slugExistsAsync(slug))
        {
            slug = $"{baseSlug}-{suffix}";
            suffix++;
        }

        return slug;
    }

    [GeneratedRegex("-{2,}")]
    private static partial Regex CollapseHyphensRegex();
}
