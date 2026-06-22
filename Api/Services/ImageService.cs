using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Api.Services;

/// <summary>
/// Processes and stores user image uploads.
///
/// Pipeline:
///   1. Validate MIME type via magic bytes
///   2. Decode with ImageSharp, reject if dimensions exceed 4096×4096
///   3. Downscale to fit within 256×256 (preserves aspect ratio, never upscales)
///   4. Encode to WebP quality 80
///   5. Upload to Garage under images/{userId}/{timestamp}.webp
///   6. Return the public URL
/// </summary>
public class ImageService(IStorageService storage)
{
  private const int MaxDimension = 4096;
  private const int TargetSize = 256;
  private const int WebpQuality = 80;
  private const long MaxBytes = 5 * 1024 * 1024; // 5 MB

  private static readonly byte[][] AllowedMagicBytes =
  [
    [0xFF, 0xD8, 0xFF],             // JPEG
    [0x89, 0x50, 0x4E, 0x47],       // PNG
    [0x52, 0x49, 0x46, 0x46],       // WebP (RIFF header)
  ];

  public async Task<string> ProcessAndUploadAsync(
      Stream inputStream, string contentType, Guid id, CancellationToken ct = default)
  {
    using var buffer = new MemoryStream();
    await inputStream.CopyToAsync(buffer, ct);
    buffer.Position = 0;

    if (buffer.Length > MaxBytes)
      throw new ImageValidationException("File exceeds the 5 MB size limit.");

    ValidateMagicBytes(buffer);
    buffer.Position = 0;

    Image image;
    try
    {
      image = await Image.LoadAsync(buffer, ct);
    }
    catch (UnknownImageFormatException)
    {
      throw new ImageValidationException("The image file is corrupted or could not be decoded.");
    }

    using (image)
    {
      if (image.Width > MaxDimension || image.Height > MaxDimension)
        throw new ImageValidationException($"Image dimensions must not exceed {MaxDimension}×{MaxDimension} px.");

      // Only downscale — never upscale images already smaller than the target
      if (image.Width > TargetSize || image.Height > TargetSize)
        image.Mutate(x => x.Resize(new ResizeOptions
        {
          Size = new Size(TargetSize, TargetSize),
          Mode = ResizeMode.Max
        }));

      using var output = new MemoryStream();
      await image.SaveAsWebpAsync(output, new WebpEncoder { Quality = WebpQuality }, ct);
      output.Position = 0;

      var key = $"images/{id}/{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.webp";
      return await storage.UploadAsync(output, key, "image/webp", ct);
    }
  }

  private static void ValidateMagicBytes(Stream stream)
  {
    Span<byte> header = stackalloc byte[8];
    var read = stream.Read(header);

    foreach (var magic in AllowedMagicBytes)
    {
      if (read >= magic.Length && header[..magic.Length].SequenceEqual(magic))
        return;
    }

    throw new ImageValidationException("Unsupported image format. Allowed: JPEG, PNG, WebP.");
  }
}

public class ImageValidationException(string message) : Exception(message);
