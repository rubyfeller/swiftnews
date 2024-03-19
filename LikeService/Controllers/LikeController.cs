using AsyncDataServices;
using Microsoft.AspNetCore.Mvc;

namespace LikeService.Controllers
{
    [ApiController]
    [Route("api/l/likes")]
    public class LikeController : ControllerBase
    {
        private readonly IMessageBusClient _messageBusClient;

        public LikeController(IMessageBusClient messageBusClient)
        {
            _messageBusClient = messageBusClient;
        }

        [HttpGet]
        public IActionResult GetLike() {
            Console.WriteLine("Hello there!");

            return Ok("I am a like?");
        }   

        [HttpPost]
        public IActionResult AddLike()
        {
            Console.WriteLine("Like added");

            try
            {
                _messageBusClient.AddLike();
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"Could not send: {ex.Message}");
            }

            return Ok("Like added!");
        }
    }
}