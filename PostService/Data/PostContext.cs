using Microsoft.EntityFrameworkCore;
using PostService.Models;

namespace PostService.Data;

public class PostContext : DbContext
{
    public PostContext(DbContextOptions<PostContext> options) : base(options)
    {
    }

    public DbSet<Post> Posts { get; set; }
}