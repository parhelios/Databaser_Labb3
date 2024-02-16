using DataAccess.Entities;
using MongoDB.Driver;
using Common.DTO;
using MongoDB.Bson;
namespace DataAccess.Services;

public class QuestionRepository
{
    public readonly IMongoCollection<Question> _questions;

    private readonly CategoryRepository _category;

    public QuestionRepository()
    {
        _category = new CategoryRepository();

        var databaseName = "db_labb3";

        var client = new MongoClient($"mongodb+srv://admin:SEq1X2pxF62349lI@cluster01.koulmbi.mongodb.net/");
        var database = client.GetDatabase(databaseName);
        _questions = database.GetCollection<Question>("question_db", new MongoCollectionSettings { AssignIdOnInsert = true });
    }

    public List<QuestionRecord> GetAllQuestions()
    {
        var filter = Builders<Question>.Filter.Empty;

        var allquestions = _questions
            .Find(filter).ToList()
            .Select(q => new QuestionRecord(
                q.Id.ToString(),
                q.Category.CategoryName,
                q.QuestionText,
                q.CorrectAnswer,
                q.IncorrectAnswers.ToArray()
            )
            );

        return allquestions.ToList();
    }

    public List<QuestionRecord> GetQuestionsFromQuiz(string id)
    {
        var filter = Builders<Question>.Filter.Eq("_id", id);

        var questions = _questions
            .Find(filter).ToList()
            .Select(q => new QuestionRecord(
                    q.Id.ToString(),
                    q.Category.ToString(),
                    q.QuestionText,
                    q.CorrectAnswer,
                    q.IncorrectAnswers.ToArray()
                )
            );

        return questions.ToList();
    }

    public Question GetQuestionById(string id)
    {
        var filter = Builders<Question>.Filter.Eq("_id", ObjectId.Parse(id));
        var question = _questions.Find(filter).FirstOrDefault();

        return question;
    }

    public void AddQuestion(QuestionRecord questionRecord)
    {
        var category = _category.GetCategoryByName(questionRecord.category);

        if (category == null)
        {
            return;
        }

        var question = new Question
        {
            Category = new Category { Id = category.Id, InternalId = category.InternalId, CategoryName = category.CategoryName}, 
            QuestionText = questionRecord.questionText,
            CorrectAnswer = questionRecord.correctAnswer,
            IncorrectAnswers = questionRecord.incorrectAnswers.ToArray()
        };

        _questions.InsertOne(question);
    }
}