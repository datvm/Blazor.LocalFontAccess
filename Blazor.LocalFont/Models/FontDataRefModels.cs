namespace Blazor.LocalFont;

public interface IFontDataRefCollection : IAsyncEnumerable<IFontDataRef>
{
    Task<IFontDataRef> GetItemAsync(long index);
    Task<long> GetLengthAsync();
}

public interface IFontDataRef
{
    const long DefaultMaxAllowedSize = 10 * 1024 * 1024; // 10MB

    Task<FontData> GetFontDataAsync();
    Task<Stream> GetFontFileAsync(long maxAllowedSize = DefaultMaxAllowedSize);
}

internal class FontDataRefCollection : IFontDataRefCollection
{

    readonly IJSObjectReference jsRef;
    readonly LocalFontService lFont;
    internal FontDataRefCollection(IJSObjectReference jsRef, LocalFontService lFont)
    {
        this.jsRef = jsRef;
        this.lFont = lFont;
    }

    public Task<long> GetLengthAsync()
        => lFont.GetCollectionLength(jsRef);

    public async Task<IFontDataRef> GetItemAsync(long index)
    {
        var @ref = await lFont.GetArrayItem(jsRef, index);
        return new FontDataRef(@ref, lFont);
    }

    public async IAsyncEnumerator<IFontDataRef> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        var length = await GetLengthAsync();

        for (int i = 0; i < length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var jsRef = await lFont.GetArrayItem(this.jsRef, i);
            yield return new FontDataRef(jsRef, lFont);
        }
    }

}

internal class FontDataRef : IFontDataRef
{
    readonly IJSObjectReference jsRef;
    readonly LocalFontService lFont;
    internal FontDataRef(IJSObjectReference jsRef, LocalFontService lFont)
    {
        this.jsRef = jsRef;
        this.lFont = lFont;
    }

    public Task<FontData> GetFontDataAsync()
        => lFont.GetFontDataAsync(jsRef);

    public Task<Stream> GetFontFileAsync(long maxAllowedSize)
        => lFont.GetFontStreamAsync(jsRef, maxAllowedSize);

}
