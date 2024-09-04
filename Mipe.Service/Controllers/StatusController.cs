using Microsoft.AspNetCore.Mvc;

namespace Mipe.Service.Controllers;

[ApiController]
[Route("status")]
public class StatusController : ControllerBase
{
  [HttpGet]
  public async Task GetStatus()
  {
    await Task.CompletedTask;
  }

  [HttpDelete]
  public async Task ReloadStatus(MipeLoader loader)
  {
    await loader.LoadConfiguration();
  }
}