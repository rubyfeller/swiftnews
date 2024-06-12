using PostService.Models;

namespace PostService.Data;

public class PostRepo : IPostRepo
{
    private readonly PostContext _context;

    public PostRepo(PostContext context)
    {
        _context = context;
    }

    public void CreatePost(Post post)
    {
        if (post == null)
        {
            throw new ArgumentNullException(nameof(post));
        }

        _context.Posts.Add(post);
    }

    public IEnumerable<Post> GetAllPosts()
    {
        return _context.Posts.ToList();
    }

    public Post? GetPostById(int id)
    {
        return _context.Posts.FirstOrDefault(p => p.Id == id);
    }

    public void LikePost(int postId)
    {
        var post = GetPostById(postId);

        if (post != null)
        {
            post.LikeCount++;
            Console.WriteLine("Added like to post with id: " + postId);
            SaveChanges();
        }
        else
        {
            Console.WriteLine("Post with id: " + postId + " not found");
        }
    }

    public void RemoveLike(int postId)
    {
        var post = GetPostById(postId);

        if (post != null)
        {
            post.LikeCount--;
            Console.WriteLine("Removed like from post with id: " + postId);
            SaveChanges();
        }
        else
        {
            Console.WriteLine("Post with id: " + postId + " not found");
        }
    }

    public void RemovePost(Post post)
    {
        ArgumentNullException.ThrowIfNull(post);

        _context.Posts.Remove(post);

        SaveChanges();
    }

    public bool SaveChanges()
    {
        return _context.SaveChanges() >= 0;
    }
}