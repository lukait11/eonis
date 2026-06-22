using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Utility;

[ApiController]
[Route("api/image")]
public class ImageController(IStorageService storageService) : ControllerBase
{
  [HttpGet("{*key}")]
  public async Task<IActionResult> Get(string key, CancellationToken ct)
  {
    try
    {
      var (stream, contentType) = await storageService.GetAsync(key, ct);
      return File(stream, contentType);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }
}
