
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace Facets.SharedKernal.Helpers;

public static class ImageHelper
{
    public static string GetThumbnailAsBase64(Stream inputStream)
    {
        var outStream = new MemoryStream();
        var image = Image.Load(inputStream);
        image.Mutate(x => x.Resize(150, 200));

        image.Save(outStream, image.Metadata.DecodedImageFormat!);
        image.Dispose();

        byte[] imageBytes = outStream.ToArray();
        var imageDataUrl = $"data:{image.Metadata.DecodedImageFormat!.DefaultMimeType};base64," + Convert.ToBase64String(imageBytes);
        return imageDataUrl;
    }

    public static Stream CompressImage(Stream inputStream)
    {
        var outStream = new MemoryStream();

        var image = Image.Load(inputStream);

        image.Mutate(x => x.Resize(image.Width, image.Height));

        var encoder = new PngEncoder()
        {
            ColorType = PngColorType.Palette
        };

        image.Save(outStream, encoder);
        image.Dispose();

        outStream.Position = 0;
        return outStream;
    }
}
