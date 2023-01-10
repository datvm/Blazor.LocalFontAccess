namespace Blazor.LocalFont;

/// <summary>
/// A reference to a Javascript Array of FontData for further operation.
/// </summary>
public interface IFontDataRefCollection : IAsyncEnumerable<IFontDataRef>
{
    /// <summary>
    /// Get a single FontData reference at the specified index.
    /// </summary>
    /// <param name="index">The index to get the instance.</param>
    /// <returns>A <see cref="IFontDataRef"/> representing a FontData Javascript reference.</returns>
    Task<IFontDataRef> GetItemAsync(long index);

    /// <summary>
    /// Get the total number of items in this array.
    /// </summary>
    /// <returns>The number of items (length) of this array</returns>
    Task<long> GetLengthAsync();
}

/// <summary>
/// A reference to a Javascript FontData for further operation.
/// </summary>
public interface IFontDataRef
{
    const long DefaultMaxAllowedSize = 10 * 1024 * 1024; // 10MB

    /// <summary>
    /// Get the information of this FontData.
    /// </summary>
    /// <returns>A <see cref="FontData"/> with information of this FontData instance</returns>
    Task<FontData> GetFontDataAsync();

    /// <summary>
    /// Get the raw binary data of this font.
    /// </summary>
    /// <param name="maxAllowedSize">Maximum number of bytes permitted to be read from JavaScript. See <see cref="IJSStreamReference.OpenReadStreamAsync(long, CancellationToken)"/></param>
    /// <returns>A <see cref="Stream"/> with binary data of this font</returns>
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
