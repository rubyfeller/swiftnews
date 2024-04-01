namespace LikeService.Clients;


public interface IPostServiceClient
{
    Task<bool> CheckPostExistence(int postId);
}
