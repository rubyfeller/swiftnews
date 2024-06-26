using PostService.Data;

namespace PostService.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IPostRepo _postRepo;

    public EventProcessor(IPostRepo postRepo)
    {
        _postRepo = postRepo;
    }

    public void ProcessEvent(string message)
    {
        throw new NotImplementedException();
    }
        
    public Task AddLike(string message) {

        var postId = int.Parse(message);

        _postRepo.LikePost(postId);        

        return Task.CompletedTask;
    }

    public Task RemoveLike(string message) {

        var postId = int.Parse(message);

        _postRepo.RemoveLike(postId);        

        return Task.CompletedTask;
    }

    public Task RemoveUserPosts(string message) {

        var userId = message;

        var posts = _postRepo.GetAllPosts().Where(p => p.UserId == userId);

        foreach (var post in posts)
        {
            Console.WriteLine(post.Content);
            _postRepo.RemovePost(post);
        }

        return Task.CompletedTask;
    }
}