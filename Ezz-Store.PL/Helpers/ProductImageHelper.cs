namespace Ezz_Store.PL.Helpers;

/// <summary>
/// Maps product IDs to default image paths under wwwroot/Images/products/ when ImageUrl is not set in DB.
/// </summary>
public static class ProductImageHelper
{
    private static readonly string[] DefaultImages =
    [
        "Men watch1.jpg",
        "Men watch2.jpg",
        "woment watch1.png",
        "women watch2.jpg",
        "smart watch1.jpg",
        "smart watch2.jpg",
        "Luxury watch1.jpg",
        "Luxury watch2.jpg",
        "sprort watch1.jpg",
        "sprort watch2.jpg"
    ];

    public static string? GetDefaultImagePath(int productId)
    {
        var index = productId - 1;
        if (index < 0 || index >= DefaultImages.Length)
            return null;
        var fileName = DefaultImages[index];
        return "/Images/products/" + Uri.EscapeDataString(fileName);
    }

    /// <summary>
    /// Returns a URL-safe path for static file (encodes spaces/special chars in filename).
    /// </summary>
    public static string? EncodeImageUrl(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return null;
        var lastSlash = path.LastIndexOf('/');
        if (lastSlash < 0)
            return path;
        var dir = path[..(lastSlash + 1)];
        var file = path[(lastSlash + 1)..];
        return dir + Uri.EscapeDataString(file);
    }

    /// <summary>
    /// Returns the image path to use: DB ImageUrl if set, otherwise default from wwwroot by product id.
    /// All paths are URL-encoded so filenames with spaces load correctly.
    /// </summary>
    public static string? GetDisplayImageUrl(string? dbImageUrl, int productId)
    {
        if (!string.IsNullOrWhiteSpace(dbImageUrl))
            return EncodeImageUrl(dbImageUrl);
        return GetDefaultImagePath(productId);
    }
}
