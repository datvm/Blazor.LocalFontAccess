namespace Microsoft.Extensions.DependencyInjection;

public static class BlazorLocalFontExtensions
{

    public static IServiceCollection AddLocalFont(this IServiceCollection services)
        => services.AddSingleton<ILocalFontService, LocalFontService>();

}
