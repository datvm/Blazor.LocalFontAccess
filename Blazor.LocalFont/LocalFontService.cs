namespace Blazor.LocalFont;

public interface ILocalFontService
{
    Task<IEnumerable<FontData>> GetFontsAsync();
    Task<FontPermission> GetPermissionAsync();
    Task<bool> IsSupportedAsync();
}

public class LocalFontService : ILocalFontService
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

    public async Task<IEnumerable<FontData>> GetFontsAsync()
        => await InvokeJsAsync<IEnumerable<FontData>>("getFontsAsync", false);
    
    async Task<T> InvokeJsAsync<T>(string name, params object?[]? args)
        => await js.InvokeAsync<T>($"{InteropPrefix}{name}", args);

}
