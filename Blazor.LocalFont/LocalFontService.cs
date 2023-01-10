using System.Reflection.Metadata;

namespace Blazor.LocalFont;

public interface ILocalFontService
{
    Task<IEnumerable<FontData>> GetFontsAsync();
    Task<IEnumerable<FontData>> GetFontsAsync(IEnumerable<string>? postscriptNames);
    Task<IEnumerable<FontData>> GetFontsAsync(QueryLocalFontsOptions? options);
    Task<IFontDataRefCollection> GetFontsRefAsync();
    Task<IFontDataRefCollection> GetFontsRefAsync(IEnumerable<string>? postscriptNames);
    Task<IFontDataRefCollection> GetFontsRefAsync(QueryLocalFontsOptions? options);
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

    public Task<IEnumerable<FontData>> GetFontsAsync()
        => GetFontsAsync((QueryLocalFontsOptions?)null);

    public Task<IEnumerable<FontData>> GetFontsAsync(IEnumerable<string>? postscriptNames)
        => GetFontsAsync(postscriptNames is null ?
            null :
            new QueryLocalFontsOptions
            {
                PostscriptNames = postscriptNames
            });

    public async Task<IEnumerable<FontData>> GetFontsAsync(QueryLocalFontsOptions? options)
        => await InvokeJsAsync<IEnumerable<FontData>>("getFontsAsync", options, false);

    public Task<IFontDataRefCollection> GetFontsRefAsync()
        => GetFontsRefAsync((QueryLocalFontsOptions?)null);

    public Task<IFontDataRefCollection> GetFontsRefAsync(IEnumerable<string>? postscriptNames)
        => GetFontsRefAsync(postscriptNames is null ? 
            null : 
            new QueryLocalFontsOptions { PostscriptNames = postscriptNames });

    public async Task<IFontDataRefCollection> GetFontsRefAsync(QueryLocalFontsOptions? options)
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
