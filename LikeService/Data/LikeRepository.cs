using LikeService.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LikeService.Data;

public class LikeRepository : ILikeRepository
{
    private readonly IMongoCollection<Like> _likes;

    public LikeRepository(ILikeStoreDatabaseSettings settings, IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        _likes = database.GetCollection<Like>(settings.LikeCollectionName);
    }

    public async Task<List<Like>> Get()
    {
        return await _likes.Find(like => true).ToListAsync();
    }

    public async Task<Like> Get(string id)
    {
        return await _likes.Find<Like>(like => like.Id == ObjectId.Parse(id)).FirstOrDefaultAsync();
    }

    public async Task<Like> Create(Like like)
    {
        await _likes.InsertOneAsync(like);
        return like;
    }

    public async Task Remove(string id)
    {
        await _likes.DeleteOneAsync(like => like.Id == ObjectId.Parse(id));
    }
}