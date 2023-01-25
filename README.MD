This library add interop support for [Local Font Access API](https://developer.mozilla.org/en-US/docs/Web/API/Local_Font_Access_API) in Blazor WebAssembly.

# Demo & Documentation

See [Demo project](./Blazor.LocalFont.DemoWebAssembly/).

The library is fully XML documented. All the methods and properties names follow the original [Javascript API](https://developer.mozilla.org/en-US/docs/Web/API/Local_Font_Access_API).

# Installation & Setup

## Nuget Package

Install this library through [Blazor.LocalFont](https://www.nuget.org/packages/Blazor.LocalFont):

```ps
dotnet add package Blazor.LocalFont
```

The project requires .NET 7.0 or later.

## Setup Dependency Injection

Call `AddLocalFont` method to register `ILocalFontService` into your DI container.

```cs
builder.Services
    .AddLocalFont();
```

From your pages or services, you can inject an instance of `ILocalFontService` to use the library.

```cs
@inject ILocalFontService LFonts;
```

# Usage

## `Task<bool> IsSupportedAsync()`

Check if the browser supports the Local Font Access API.

```cs
if (await LFonts.IsSupportedAsync()) 
{
    // It is supported
}
```

## `Task<FontPermission> GetPermissionAsync()`

Get the current permission status for `local-fonts` permission. See [Javascript Permissions API](https://developer.mozilla.org/en-US/docs/Web/API/Permissions_API).

```cs
var permission = await LFonts.GetPermissionAsync();
// permission is one of the following values: "granted", "denied", "prompt"
```

If the permission is `prompt`, you can request the permission with one of the query methods.

## `QueryLocalFontsAsync` and `QueryLocalFontsRefAsync`

You can request a list of local fonts with these methods:

```cs
    Task<IEnumerable<FontData>> QueryLocalFontsAsync();
    Task<IEnumerable<FontData>> QueryLocalFontsAsync(IEnumerable<string>? postscriptNames);
    Task<IEnumerable<FontData>> QueryLocalFontsAsync(QueryLocalFontsOptions? options);
    Task<IFontDataRefCollection> QueryLocalFontsRefAsync();
    Task<IFontDataRefCollection> QueryLocalFontsRefAsync(IEnumerable<string>? postscriptNames);
    Task<IFontDataRefCollection> QueryLocalFontsRefAsync(QueryLocalFontsOptions? options);
```

`postscriptNames` is a list of PostScript names of the fonts you want to query. If you don't specify any, all the fonts will be returned.

The difference between `QueryLocalFontsAsync` and `QueryLocalFontsRefAsync` is that one returns the serialized information only (and no further operation is possible), while the other returns a reference to the font data that can be used to load the font data. See [Font Data JS Reference types below](#ifontdatarefcollection-and-ifontdataref) for more information.

**Example**

```cs
// Get the font from backend C#
var fonts = await LFonts.QueryLocalFontsAsync();
firstFont = fonts.First();
```

```html
<!-- Use it in HTML -->
<span style="font-family: @(firstFont.Family);">
    The quick brown fox jumps over the lazy dog
</span>
```

## `IFontDataRefCollection` and `IFontDataRef`

If you use `QueryLocalFontsRefAsync` method, you will receive an instance of `IFontDataRefCollection` instead of `FontData`.

- `IFontDataRefCollection`: A reference to a Javascript Array of FontData for further operation.

    - `Task<long> GetLengthAsync()`: Get the total number of items (length) in this array.

    - `Task<IFontDataRef> GetItemAsync(long index)`: Get a single FontData reference at the specified index.

- `IFontDataRef`: A reference to a Javascript `FontData` for further operation.

    - `Task<FontData> GetFontDataAsync()`: Get the information of this `FontData`.

    - `Task<Stream> GetFontFileAsync(long maxAllowedSize = DefaultMaxAllowedSize)`: Get the raw binary data of this font. The default max size is 10MB, you will receive an error if a font is larger than this size.

**Example**

This example request a list of fonts and then load its binary data to determine the font type according to [the example from MDN](https://developer.mozilla.org/en-US/docs/Web/API/Window/queryLocalFonts#accessing_low-level_data).


```cs
// Get font list
fontRefs = await LFonts.QueryLocalFontsRefAsync();
fontRefsLen = await fontRefs.GetLengthAsync();

// Read data from the first font
var font = await fontRefs.GetItemAsync(0);
var data = await font.GetFontDataAsync();

using var stream = await font.GetFontFileAsync();
var buffer = new byte[4];
var count = await stream.ReadAsync(buffer, 0, 4);

if (count != 4)
{
    currFontType = "Unknown";
}
else
{
    var sfntVersion = System.Text.Encoding.UTF8.GetString(buffer);
    switch (sfntVersion)
    {
        case "\x00\x01\x00\x00":
        case "true":
        case "typ1":
            currFontType = "truetype";
            break;
        case "OTTO":
            currFontType = "cff";
            break;
    }
}
```