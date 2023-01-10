namespace Blazor.LocalFont.DemoWebAssembly.Pages;

partial class Index
{
    bool supported = false;    
    FontPermission permission = FontPermission.Prompt;
    
    protected override async Task OnInitializedAsync()
    {
        supported = await LFonts.IsSupportedAsync();
        if (!supported) { return; }

        await this.UpdatePermissionStateAsync();
    }

    async Task UpdatePermissionStateAsync()
    {
        permission = await LFonts.GetPermissionAsync();
    }

}
