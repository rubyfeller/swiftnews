namespace PostService.EventProcessing;

public interface IEventProcessor
{
    void ProcessEvent(string message);
    Task AddLike(string message);
    Task RemoveLike(string message);
    Task RemoveUserPosts(string message);
}