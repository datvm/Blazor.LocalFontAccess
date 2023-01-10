using System.Text.Json.Serialization;

namespace Blazor.LocalFont;

/// <summary>
/// Permission state of Font API.
/// <see href="https://developer.mozilla.org/en-US/docs/Web/API/PermissionStatus/state">PermissionStatus</see>
/// </summary>
public enum FontPermission
{
    Granted,
    Denied,
    Prompt,
}

/// <summary>
/// Contains optional configuration parameters.
/// <see href="https://developer.mozilla.org/en-US/docs/Web/API/Window/queryLocalFonts#parameters">queryLocalFonts Parameters</see>
/// </summary>
public class QueryLocalFontsOptions
{
    /// <summary>
    /// An enumerable of font PostScript names. If this is specified, only fonts with PostScript names matching those in the array will be included in the results.
    /// </summary>
    [JsonPropertyName("postscriptNames")]
    public IEnumerable<string>? PostscriptNames { get; set; }
}

/// <summary>
/// Represent a <see href="https://developer.mozilla.org/en-US/docs/Web/API/FontData">FontData</see> information.
/// </summary>
public class FontData
{
    /// <summary>
    /// Family of the font face. <see href="https://developer.mozilla.org/en-US/docs/Web/API/FontData/family">FontData.family</see>
    /// </summary>
    /// <remarks>
    /// This is the name used when referring to the font family from code, for example, in the font-family property or in places within the @font-face at-rule such as the local() function.
    /// </remarks>
    [JsonPropertyName("family")]
    public required string Family { get; init; }

    /// <summary>
    /// Full name of the font face. <see href="https://developer.mozilla.org/en-US/docs/Web/API/FontData/fullName">FontData.fullName</see>
    /// </summary>
    /// <remarks>
    /// This is usually a human-readable name used to identify the font, e.g., "Optima Bold".
    /// </remarks>
    [JsonPropertyName("fullName")]
    public required string FullName { get; init; }

    /// <summary>
    /// PostScript name of the font face. <see href="https://developer.mozilla.org/en-US/docs/Web/API/FontData/postscriptName">FontData.postscriptName</see>
    /// </summary>
    /// <remarks>
    /// This is the name used to uniquely identify the PostScript font, and is generally an unbroken sequence of characters that includes the font's name and style.
    /// </remarks>
    [JsonPropertyName("postscriptName")]
    public required string PostScriptName { get; init; }

    /// <summary>
    /// Style of the font face. <see href="https://developer.mozilla.org/en-US/docs/Web/API/FontData/style">FontData.style</see>
    /// </summary>
    /// <remarks>
    /// This is the value used to select the style of the font you want to use, for example inside the font-style property.
    /// </remarks>
    [JsonPropertyName("style")]
    public required string Style { get; init; }
}