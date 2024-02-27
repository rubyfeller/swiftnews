using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostService.Data;
using PostService.Models;

namespace PostService.Controllers
{
    [ApiController]
    [Route("/api/posts")]
    public class PostController : ControllerBase
    {
        private readonly PostContext _context;
        public PostController(PostContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetAll()
        {
            return await _context.Posts.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetById(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return post;
        }

        [HttpPost]
        public async Task<ActionResult<Post>> Create(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Post post)
        {
            if (id != post.Id)
            {
                return BadRequest();
            }
            _context.Entry(post).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("test")]
        public ActionResult<string> Test()
        {
            return "Hello from PostService!";
        }
    }
}
