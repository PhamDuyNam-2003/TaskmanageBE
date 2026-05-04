using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BE.Models
{
    public class Project
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string OwnerId { get; set; } = null!;

        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> MemberIds { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
       
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
