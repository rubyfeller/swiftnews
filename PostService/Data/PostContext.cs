using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PostService.Models;

namespace PostService.Data;

public class PostContext : DbContext
{

    public class PostContextFactory : IDesignTimeDbContextFactory<PostContext>
    {
        public PostContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PostContext>();
            optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("PostsConn"));

            return new PostContext(optionsBuilder.Options);
        }
    }
    
    public PostContext(DbContextOptions<PostContext> options) : base(options)
    {
    }



    public DbSet<Post> Posts { get; set; }
}