namespace Blazor.LocalFont;

public interface ILocalFontService
{
    
    Task<IEnumerable<FontData>> QueryLocalFontsAsync();
    Task<IEnumerable<FontData>> QueryLocalFontsAsync(IEnumerable<string>? postscriptNames);
    Task<IEnumerable<FontData>> QueryLocalFontsAsync(QueryLocalFontsOptions? options);
    Task<IFontDataRefCollection> QueryLocalFontsRefAsync();
    Task<IFontDataRefCollection> QueryLocalFontsRefAsync(IEnumerable<string>? postscriptNames);
    Task<IFontDataRefCollection> QueryLocalFontsRefAsync(QueryLocalFontsOptions? options);
    Task<FontPermission> GetPermissionAsync();
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
