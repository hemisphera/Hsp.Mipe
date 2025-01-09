using Microsoft.AspNetCore.Mvc;
using Mipe.Core;

namespace Mipe.Service.Controllers;

[ApiController]
[Route("instance")]
public class InstanceController : ControllerBase
{
  [HttpGet]
  public IActionResult GetStatus(MipeInstance instance)
  {
    return Ok(new
    {
      instance.CurrentFilePath
    });
  }

  [HttpDelete]
  public async Task ReloadInstance(MipeInstance instance)
  {
    await instance.Load(null);
  }

  [HttpPost]
  public async Task LoadInstance([FromBody] string filePath, MipeInstance instance)
  {
    await instance.Load(filePath);
  }
}