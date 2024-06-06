using MassTransit;
using MassTransitTest.Models;
using Microsoft.AspNetCore.Mvc;

namespace MassTransitTest.Controllers;

[ApiController]
[Route("[controller]")]
public class MassTransitController(IPublishEndpoint publishEndpoint) : ControllerBase
{
    [HttpPost("Publish")]
    public async Task<IActionResult> Publish([FromBody] EventDetails order)
    {
        await publishEndpoint.Publish(order);
        return Ok("Success");
    }
}