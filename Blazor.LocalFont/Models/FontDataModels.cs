using System.Text.Json.Serialization;

namespace Blazor.LocalFont;

public enum FontPermission
{
    Granted,
    Denied,
    Prompt,
}

/// <summary>
/// Options for querying Local Fonts. From https://developer.mozilla.org/en-US/docs/Web/API/Window/queryLocalFonts
/// </summary>
public class QueryLocalFontsOptions
{
    [JsonPropertyName("postscriptNames")]
    public IEnumerable<string>? PostscriptNames { get; set; }
}

/// <summary>
/// Represent a Font. From https://developer.mozilla.org/en-US/docs/Web/API/FontData 
/// </summary>
public class FontData
{
    [JsonPropertyName("family")]
    public required string Family { get; init; }

    [JsonPropertyName("fullName")]
    public required string FullName { get; init; }

    [JsonPropertyName("postscriptName")]
    public required string PostScriptName { get; init; }

    [JsonPropertyName("style")]
    public required string Style { get; init; }
}