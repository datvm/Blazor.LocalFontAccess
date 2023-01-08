using Microsoft.JSInterop;

namespace Blazor.LocalFont.DemoWebAssembly.Pages;

partial class Index
{
    bool supported = false;
    string? noGestureErr;
    IEnumerable<FontData>? fonts;
    FontPermission permission;

    bool showDemo = false;

    protected override async Task OnInitializedAsync()
    {
        supported = await LFonts.IsSupportedAsync();
        if (!supported) { return; }

        await this.UpdatePermissionStateAsync();

        try
		{
			await LFonts.GetFontsAsync();
		}
		catch (Exception ex)
		{
            noGestureErr = ex.Message;
        }
    }

    async Task RequestFontAsync()
    {
        try
        {
            fonts = await LFonts.GetFontsAsync();

            var test = await LFonts.GetFontReferencesAsync();
        }
        catch (Exception ex)
        {
            await Js.InvokeVoidAsync("alert", ex.ToString());
        }
        
        await UpdatePermissionStateAsync();
    }

    async Task UpdatePermissionStateAsync()
    {
        permission = await LFonts.GetPermissionAsync();
    }

}
