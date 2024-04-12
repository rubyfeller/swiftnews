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

    public async Task<Like?> Get(string id)
    {
        try
        {
            var objectId = ObjectId.Parse(id);
            return await _likes.Find(like => like.Id == objectId).FirstOrDefaultAsync();
        }
        catch (FormatException)
        {
            return null;
        }
    }

    public async Task<Like> Create(Like like)
    {
        await _likes.InsertOneAsync(like);
        return like;
    }

    public async Task Remove(string id)
    {
        try
        {
            var objectId = ObjectId.Parse(id);
            await _likes.DeleteOneAsync(like => like.Id == objectId);
        }
        catch (FormatException)
        {
            throw new ArgumentException("Invalid ID format. Please provide a valid 24 digit hex string.", nameof(id));
        }
    }
}