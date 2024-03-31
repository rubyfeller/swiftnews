namespace LikeService.AsyncDataServices
{
    public interface IMessageBusClient
    {
        void AddLike(int postId);
        void RemoveLike(int postId);
    }
}