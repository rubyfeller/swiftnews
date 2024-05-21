using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostService.Data;
using PostService.Dtos;
using PostService.Models;
using Prometheus;

namespace PostService.Controllers
{
    [ApiController]
    [Route("/api/posts")]
    public class PostController : ControllerBase
    {
        private readonly PostContext _context;
        private readonly IMapper _mapper;
        private readonly IPostRepo _repository;

        private static readonly Counter RequestsCounter = Metrics.CreateCounter("dotnet_requests_total", "Total number of requests to the web API");

        public PostController(PostContext context, IMapper mapper, IPostRepo repository)
        {
            _context = context;
            _mapper = mapper;
            _repository = repository;
        }

        private void ObserveRequestDuration(double duration)
        {
            var histogram = Metrics.CreateHistogram(
                "dotnet_request_duration_seconds",
                "Histogram for the duration in seconds.",
                new HistogramConfiguration
                {
                    Buckets = Histogram.LinearBuckets(start: 1, width: 1, count: 5)
                });

            histogram.Observe(duration);
        }

        [HttpGet]
        public ActionResult<IEnumerable<PostReadDTO>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            RequestsCounter.Inc();

            var sw = Stopwatch.StartNew();

            var posts = _repository.GetAllPosts()
                                   .Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToList();

            sw.Stop();

            ObserveRequestDuration(sw.Elapsed.TotalSeconds);

            return Ok(_mapper.Map<IEnumerable<PostReadDTO>>(posts));
        }

        [HttpGet("{id}")]
        public ActionResult<PostReadDTO> GetById(int id)
        {
            RequestsCounter.Inc();

            var sw = Stopwatch.StartNew();

            var post = _repository.GetPostById(id);
            if (post == null)
            {
                sw.Stop();
                return NotFound();
            }

            sw.Stop();
            ObserveRequestDuration(sw.Elapsed.TotalSeconds);

            return Ok(_mapper.Map<PostReadDTO>(post));
        }

        [HttpPost]
        public ActionResult<PostReadDTO> Create(PostCreateDTO postCreateDTO)
        {
            RequestsCounter.Inc();

            var sw = Stopwatch.StartNew();

            var authorizationHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (authorizationHeader != null && authorizationHeader.StartsWith("Bearer "))
            {
                var token = authorizationHeader.Substring("Bearer ".Length).Trim();

                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                    var sub = jsonToken?.Subject;

                    if (sub == null)
                    {
                        return Unauthorized();
                    }

                    var post = _mapper.Map<Post>(postCreateDTO);

                    post.UserId = sub;

                    _repository.CreatePost(post);
                    _repository.SaveChanges();

                    sw.Stop();

                    ObserveRequestDuration(sw.Elapsed.TotalSeconds);

                    return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error decoding token: " + ex.Message);
                    return BadRequest("Invalid token");
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Post post)
        {
            RequestsCounter.Inc();

            var sw = Stopwatch.StartNew();

            if (id != post.Id)
            {
                sw.Stop();
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
                    sw.Stop();
                    return NotFound();
                }
                else
                {
                    sw.Stop();
                    throw;
                }
            }

            sw.Stop();

            ObserveRequestDuration(sw.Elapsed.TotalSeconds);

            return NoContent();
        }

        private bool PostExists(int id)
        {
            RequestsCounter.Inc();

            return _context.Posts.Any(e => e.Id == id);
        }

        [HttpGet("{id}/exists")]
        public ActionResult<bool> Exists(int id)
        {
            RequestsCounter.Inc();

            var sw = Stopwatch.StartNew();

            var postExists = PostExists(id);

            sw.Stop();

            ObserveRequestDuration(sw.Elapsed.TotalSeconds);

            return Ok(postExists);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            RequestsCounter.Inc();

            var sw = Stopwatch.StartNew();

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                sw.Stop();
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            sw.Stop();

            ObserveRequestDuration(sw.Elapsed.TotalSeconds);

            return NoContent();
        }
    }
}
