using Microsoft.AspNetCore.Mvc;

namespace Mipe.Service.Controllers;

[ApiController]
[Route("instance")]
public class InstanceController : ControllerBase
{
  private readonly ILogger<InstanceController> _logger;

  public InstanceController(ILogger<InstanceController> logger)
  {
    _logger = logger;
  }

  [HttpGet]
  public IActionResult GetStatus(MipeLoader loader)
  {
    return Ok(new
    {
      loader.CurrentFilePath
    });
  }

  [HttpDelete]
  public async Task ReloadInstance(MipeLoader loader)
  {
    await loader.LoadConfiguration(null);
  }

  [HttpPost]
  public async Task LoadInstance([FromBody] string filePath, MipeLoader loader)
  {
    await loader.LoadConfiguration(filePath);
  }
}