using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace LikeService.Models
{
    public class Like
    {
        public Like(int postid)
        {
            PostId = postid;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("postid")]
        [BsonRequired]
        public int PostId { get; set; }

        [BsonElement("userid")]
        public int UserId { get; set; }

        [BsonElement("likedat")]
        public DateTime LikedAt { get; set; } = DateTime.UtcNow;
    }
}