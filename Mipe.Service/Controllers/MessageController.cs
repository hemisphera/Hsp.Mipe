using Microsoft.AspNetCore.Mvc;
using Mipe.Core;
using Mipe.Core.Inputs;
using Mipe.Service.Models;

namespace Mipe.Service.Controllers;

[ApiController]
[Route("messages")]
public class MessageController : ControllerBase
{
  private readonly MipeInstance _instance;
  private readonly ILogger<MessageController> _logger;


  public MessageController(MipeInstance instance, ILogger<MessageController> logger)
  {
    _instance = instance;
    _logger = logger;
  }


  [Route("{queueName}")]
  [HttpPost]
  public IActionResult GetOutputDevices([FromRoute] string queueName, [FromBody] HttpMidiMessage message)
  {
    var midiMessage = message.ToMidiMessage();
    if (midiMessage == null) return BadRequest();

    var ports = _instance.Connections?
      .Where(c => c.Connected)
      .Select(c => c.Port)
      .OfType<WebRequestInputPort>()
      .Where(p => p.QueueName == queueName)
      .ToArray();
    if (ports == null || ports.Length == 0) return NotFound();

    foreach (var port in ports)
    {
      _logger.LogInformation("Posted {} to {}.", midiMessage, port);
      port.RaiseMessageReceived(midiMessage);
    }

    return Ok();
  }
}