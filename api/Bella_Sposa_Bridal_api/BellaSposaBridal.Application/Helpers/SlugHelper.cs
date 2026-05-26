using System.Text.RegularExpressions;

namespace BellaSposaBridal.Application.Helpers;

public static partial class SlugHelper
{
    public static string Generate(string input)
    {
        var slug = input.ToLowerInvariant().Trim();
        slug = NonAlphanumericSpaceOrDash().Replace(slug, "");
        slug = WhitespaceRun().Replace(slug, "-");
        slug = DashRun().Replace(slug, "-");
        return slug.Trim('-');
    }

    // Unique slug: appends -2, -3, … until the candidate is not in the existing set.
    public static string EnsureUnique(string baseSlug, IEnumerable<string> existing)
    {
        var set = new HashSet<string>(existing, StringComparer.OrdinalIgnoreCase);
        if (!set.Contains(baseSlug)) return baseSlug;
        var n = 2;
        string candidate;
        do { candidate = $"{baseSlug}-{n++}"; } while (set.Contains(candidate));
        return candidate;
    }

    [GeneratedRegex(@"[^a-z0-9\s-]")]
    private static partial Regex NonAlphanumericSpaceOrDash();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRun();

    [GeneratedRegex(@"-+")]
    private static partial Regex DashRun();
}
