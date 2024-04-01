using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostService.Data;
using PostService.Dtos;
using PostService.Models;

namespace PostService.Controllers;

[ApiController]
[Route("/api/posts")]
public class PostController : ControllerBase
{
    private readonly PostContext _context;
    private readonly IMapper _mapper;
    private readonly IPostRepo _repository;

    public PostController(PostContext context, IMapper mapper, IPostRepo repository)
    {
        _context = context;
        _mapper = mapper;
        _repository = repository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PostReadDTO>> GetAll()
    {
        var posts = _repository.GetAllPosts();
        return Ok(_mapper.Map<IEnumerable<PostReadDTO>>(posts));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PostReadDTO>> GetById(int id)
    {
        var post = _repository.GetPostById(id);
        if (post == null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<PostReadDTO>(post));
    }

    [HttpPost]
    public async Task<ActionResult<PostReadDTO>> Create(PostCreateDTO postCreateDTO)
    {
        var post = _mapper.Map<Post>(postCreateDTO);

        _repository.CreatePost(post);
        _repository.SaveChanges();

        var postReadDTO = _mapper.Map<PostReadDTO>(post);

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

    [HttpGet("{id}/exists")]
    public ActionResult<bool> Exists(int id)
    {
        var postExists = PostExists(id);
        return Ok(postExists);
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