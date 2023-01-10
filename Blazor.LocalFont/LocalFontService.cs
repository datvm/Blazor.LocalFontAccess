namespace Blazor.LocalFont;

/// <summary>
/// Represents a service for <see href="https://developer.mozilla.org/en-US/docs/Web/API/Local_Font_Access_API">Local Font Access API</see>.
/// </summary>
public interface ILocalFontService
{

    /// <summary>
    /// Get the FontData objects representing the font faces available locally. The FontData objects are serialized for information.
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/API/Window/queryLocalFonts">queryLocalFonts</see>
    /// </summary>
    /// <returns>An enumerable of FontData information</returns>
    Task<IEnumerable<FontData>> QueryLocalFontsAsync();
    /// <summary>
    /// Get the FontData objects representing the font faces available locally. The FontData objects are serialized for information.
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/API/Window/queryLocalFonts">queryLocalFonts</see>
    /// </summary>
    /// <param name="postscriptNames">An array of font PostScript names. If this is specified, only fonts with PostScript names matching those in the array will be included in the results.</param>
    /// <returns>An enumerable of FontData information</returns>
    Task<IEnumerable<FontData>> QueryLocalFontsAsync(IEnumerable<string>? postscriptNames);
    /// <summary>
    /// Get the FontData objects representing the font faces available locally. The FontData objects are serialized for information.
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/API/Window/queryLocalFonts">queryLocalFonts</see>
    /// </summary>
    /// <param name="options">Contains optional configuration parameters.</param>
    /// <returns>An enumerable of FontData information</returns>
    Task<IEnumerable<FontData>> QueryLocalFontsAsync(QueryLocalFontsOptions? options);

    /// <summary>
    /// Get the FontData objects representing the font faces available locally. 
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/API/Window/queryLocalFonts">window.queryLocalFonts</see>
    /// </summary>
    /// <returns>A Font Collection that keeps reference to the Javascript object of the array of FontData</returns>
    Task<IFontDataRefCollection> QueryLocalFontsRefAsync();
    /// <summary>
    /// Get the FontData objects representing the font faces available locally. 
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/API/Window/queryLocalFonts">window.queryLocalFonts</see>
    /// </summary>
    /// <param name="postscriptNames">An array of font PostScript names. If this is specified, only fonts with PostScript names matching those in the array will be included in the results.</param>
    /// <returns>A Font Collection that keeps reference to the Javascript object of the array of FontData</returns>
    Task<IFontDataRefCollection> QueryLocalFontsRefAsync(IEnumerable<string>? postscriptNames);
    /// <summary>
    /// Get the FontData objects representing the font faces available locally. 
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/API/Window/queryLocalFonts">window.queryLocalFonts</see>
    /// </summary>
    /// <param name="options">Contains optional configuration parameters.</param>
    /// <returns>A Font Collection that keeps reference to the Javascript object of the array of FontData</returns>
    Task<IFontDataRefCollection> QueryLocalFontsRefAsync(QueryLocalFontsOptions? options);

    /// <summary>
    /// Get the current Font (local-fonts) Permission state.
    /// </summary>
    /// <returns>The current state of local-fonts permission</returns>
    Task<FontPermission> GetPermissionAsync();

    /// <summary>
    /// Check if Local Font Access API is available
    /// </summary>
    /// <returns>true if <see href="https://developer.mozilla.org/en-US/docs/Web/API/Window/queryLocalFonts">queryLocalFonts</see>  is available. false otherwise.</returns>
    Task<bool> IsSupportedAsync();
}

internal class LocalFontService : ILocalFontService
{
    const string InteropPrefix = "blazorLocalFont.";

    readonly IJSRuntime js;
    public LocalFontService(IJSRuntime js)
    {
        this.js = js;
    }

    public async Task<bool> IsSupportedAsync()
        => await InvokeJsAsync<bool>("isSupported");

    public async Task<FontPermission> GetPermissionAsync()
    {
        var str = await InvokeJsAsync<string>("getPermissionStateAsync");
        return Enum.Parse<FontPermission>(str, true);
    }

    public Task<IEnumerable<FontData>> QueryLocalFontsAsync()
        => QueryLocalFontsAsync((QueryLocalFontsOptions?)null);

    public Task<IEnumerable<FontData>> QueryLocalFontsAsync(IEnumerable<string>? postscriptNames)
        => QueryLocalFontsAsync(postscriptNames is null ?
            null :
            new QueryLocalFontsOptions
            {
                PostscriptNames = postscriptNames
            });

    public async Task<IEnumerable<FontData>> QueryLocalFontsAsync(QueryLocalFontsOptions? options)
        => await InvokeJsAsync<IEnumerable<FontData>>("getFontsAsync", options, false);

    public Task<IFontDataRefCollection> QueryLocalFontsRefAsync()
        => QueryLocalFontsRefAsync((QueryLocalFontsOptions?)null);

    public Task<IFontDataRefCollection> QueryLocalFontsRefAsync(IEnumerable<string>? postscriptNames)
        => QueryLocalFontsRefAsync(postscriptNames is null ? 
            null : 
            new QueryLocalFontsOptions { PostscriptNames = postscriptNames });

    public async Task<IFontDataRefCollection> QueryLocalFontsRefAsync(QueryLocalFontsOptions? options)
    {
        var @ref = await InvokeJsAsync<IJSObjectReference>(
            "getFontsAsync", options, true);

        return new FontDataRefCollection(@ref, this);
    }

    internal async Task<IJSObjectReference> GetArrayItem(IJSObjectReference @ref, long index)
        => await InvokeJsAsync<IJSObjectReference>("getArrayItem", @ref, index);

    internal async Task<FontData> GetFontDataAsync(IJSObjectReference fontDataRef)
        => await InvokeJsAsync<FontData>("serializeFontData", fontDataRef);

    internal async Task<long> GetCollectionLength(IJSObjectReference @ref)
        => await InvokeJsAsync<long>("getArrLength", @ref);

    internal async Task<Stream> GetFontStreamAsync(IJSObjectReference @ref, long maxAllowedSize)
    {
        var streamRef = await InvokeJsAsync<IJSStreamReference>("getFontStreamAsync", @ref);
        return await streamRef.OpenReadStreamAsync(maxAllowedSize);
    }

    async Task<T> InvokeJsAsync<T>(string name, params object?[]? args)
        => await js.InvokeAsync<T>($"{InteropPrefix}{name}", args);

}
