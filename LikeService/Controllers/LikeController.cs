using LikeService.AsyncDataServices;
using LikeService.Data;
using LikeService.Models;
using Microsoft.AspNetCore.Mvc;

namespace LikeService.Controllers
{
    [ApiController]
    [Route("api/l/likes")]
    public class LikeController : ControllerBase
    {
        private readonly IMessageBusClient _messageBusClient;
        private readonly ILikeRepository _likeRepository;

        public LikeController(IMessageBusClient messageBusClient, ILikeRepository likeRepository)
        {
            _messageBusClient = messageBusClient;
            _likeRepository = likeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetLikes() {
            var likes = await _likeRepository.Get();

            return Ok(likes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLike(string id)
        {
            var like = await _likeRepository.Get(id);

            if (like == null)
            {
                return NotFound();
            }

            return Ok(like);
        }

        [HttpPost]
        public IActionResult AddLike()
        {

            try
            {
                _likeRepository.Create(new Like());
                _messageBusClient.AddLike();
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"Could not send: {ex.Message}");
                
                return StatusCode(500, "Could not send message");
            }

            return Ok("Like added!");
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveLike(string id)
        {
            var like = _likeRepository.Get(id);

            if (like == null)
            {
                return NotFound();
            }

            _likeRepository.Remove(id);

            return Ok("Like removed!");
        }
    }
}