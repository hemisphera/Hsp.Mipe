using Hsp.Midi;
using Hsp.Midi.Messages;
using Microsoft.AspNetCore.Mvc;
using Mipe.Core;

namespace Mipe.Service.Controllers;

[ApiController]
[Route("devices")]
public class DevicesController : ControllerBase
{
  [Route("output")]
  [HttpGet]
  public IActionResult GetOutputDevices()
  {
    return Ok(OutputMidiDevicePool.Instance.Enumerate().Select(d => d.Name));
  }

  [Route("input")]
  [HttpGet]
  public IActionResult GetInputDevices()
  {
    return Ok(InputMidiDevicePool.Instance.Enumerate().Select(d => d.Name));
  }

  [Route("input/{name}/{message:int}")]
  [HttpPost]
  public IActionResult Post([FromRoute] string name, [FromRoute] int message, MipeInstance instance)
  {
    var connections = instance.Connections?.Where(c => c.InputPort == name && c.Connected);
    if (connections == null) return Ok();

    var midiMessage = MessageBuilder.Build(message, 0);

    foreach (var connection in connections)
    {
      connection.Dispatch(midiMessage);
    }

    return Ok();
  }
}