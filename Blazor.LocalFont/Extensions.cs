namespace Microsoft.Extensions.DependencyInjection;

public static class BlazorLocalFontExtensions
{

    /// <summary>
    /// Add ILocalFontService services to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddLocalFont(this IServiceCollection services)
        => services.AddSingleton<ILocalFontService, LocalFontService>();

}
