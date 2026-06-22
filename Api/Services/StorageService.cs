using System.Net.Http.Headers;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace Api.Services;

public interface IStorageService
{
  Task<string> UploadAsync(Stream content, string key, string contentType, CancellationToken ct = default);

  Task DeleteAsync(string key, CancellationToken ct = default);
}

public class GarageStorageService(IConfiguration configuration, ILogger<GarageStorageService> logger) : IStorageService
{
  private readonly string _bucket = configuration["Storage:Bucket"]
      ?? throw new InvalidOperationException("Storage:Bucket is not configured.");

  private AmazonS3Client CreateClient()
  {
    var endpoint = configuration["Storage:Endpoint"]
        ?? throw new InvalidOperationException("Storage:Endpoint is not configured.");
    var accessKey = configuration["Storage:AccessKey"]
        ?? throw new InvalidOperationException("Storage:AccessKey is not configured.");
    var secretKey = configuration["Storage:SecretKey"]
        ?? throw new InvalidOperationException("Storage:SecretKey is not configured.");
    var region = configuration["Storage:Region"] ?? "garage";

    var credentials = new BasicAWSCredentials(accessKey, secretKey);
    var config = new AmazonS3Config
    {
      ServiceURL = endpoint,
      ForcePathStyle = true,      // Garage uses path-style: endpoint/bucket/key
      AuthenticationRegion = region
    };
    return new AmazonS3Client(credentials, config);
  }

  public async Task<string> UploadAsync(Stream content, string key, string contentType, CancellationToken ct = default)
  {
    logger.LogInformation("Storage upload starting — bucket: {Bucket}, key: {Key}, endpoint: {Endpoint}",
        _bucket, key, configuration["Storage:Endpoint"]);

    using var client = CreateClient();

    var endpoint = configuration["Storage:Endpoint"]!;
    var protocol = endpoint.StartsWith("https", StringComparison.OrdinalIgnoreCase)
        ? Protocol.HTTPS
        : Protocol.HTTP;

    var presigned = client.GetPreSignedURL(new GetPreSignedUrlRequest
    {
      BucketName = _bucket,
      Key = key,
      Verb = HttpVerb.PUT,
      Expires = DateTime.UtcNow.AddMinutes(5),
      Protocol = protocol
    });

    using var http = new HttpClient();
    using var req = new HttpRequestMessage(HttpMethod.Put, presigned)
    {
      Content = new StreamContent(content)
    };
    req.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

    logger.LogInformation("Uploading to presigned URL: {Url}", presigned);

    var resp = await http.SendAsync(req, ct);

    if (!resp.IsSuccessStatusCode)
    {
      var body = await resp.Content.ReadAsStringAsync(ct);
      logger.LogError("Garage upload failed — status {Status}, body: {Body}", (int)resp.StatusCode, body);
    }

    resp.EnsureSuccessStatusCode();

    logger.LogInformation("Storage upload succeeded — key: {Key}", key);
    return $"{endpoint.TrimEnd('/')}/{_bucket}/{key}";
  }

  public async Task DeleteAsync(string key, CancellationToken ct = default)
  {
    using var client = CreateClient();
    try
    {
      await client.DeleteObjectAsync(_bucket, key, ct);
    }
    catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
    {
      // Object already gone — not an error
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Garage delete failed for key {Key}", key);
      throw;
    }
  }
}
