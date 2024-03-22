using LikeService.Models;

namespace LikeService.Data;

public interface ILikeRepository
{
    Task<List<Like>> Get();
    Task<Like> Get(string id);
    Task<Like> Create(Like like);
    Task Remove(string id);
}