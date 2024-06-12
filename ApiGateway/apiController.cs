namespace APIGateway;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/")]
public class ApiController : ControllerBase
{
    [HttpGet]
    public IActionResult Index()
    {
        return Ok("Hello from API Gateway!");
    }
}
