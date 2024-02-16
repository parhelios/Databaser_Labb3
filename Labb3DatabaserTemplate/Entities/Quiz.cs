using MongoDB.Bson;

namespace DataAccess.Entities;

public class Quiz
{
    public ObjectId Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<Category> Categories { get; set; } = new();
    public List<Question> Questions { get; set; } = new();
}