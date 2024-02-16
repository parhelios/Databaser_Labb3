using MongoDB.Bson;

namespace DataAccess.Entities;

public class Question
{
    public ObjectId Id { get; set; }
    public Category Category { get; set; } 
    public string QuestionText { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public string[] IncorrectAnswers { get; set; } = new string[2];
}
