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
        public async Task<IActionResult> GetLikes()
        {
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

        [HttpPost("{id}")]
        public async Task<IActionResult> AddLike(int id)
        {
            try
            {
                await _likeRepository.Create(new Like(postid: id));
                _messageBusClient.AddLike(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not send: {ex.Message}");

                return StatusCode(500, "Could not send message");
            }

            return Ok("Like added!");
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveLike(string id)
        {
            try
            {
                var like = await _likeRepository.Get(id);

                if (like == null)
                {
                    return NotFound();
                }

                await _likeRepository.Remove(id);
                _messageBusClient.RemoveLike(like.PostId);

                return Ok("Like removed!");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}