using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataAccess.Entities;

//TODO Se över om nedan behövs
[BsonIgnoreExtraElements]
public class Category
{
    public ObjectId Id { get; set; }
    public int InternalId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}