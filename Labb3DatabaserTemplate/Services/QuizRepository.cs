using Common.DTO;
using DataAccess.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DataAccess.Services;

public class QuizRepository
{
    private readonly IMongoCollection<Quiz> _quiz;

    private readonly QuestionRepository _question;

    public QuizRepository()
    {
        _question = new QuestionRepository();

        var databaseName = "db_labb3";

        var client = new MongoClient($"mongodb+srv://admin:SEq1X2pxF62349lI@cluster01.koulmbi.mongodb.net/");
        var database = client.GetDatabase(databaseName);
        _quiz = database.GetCollection<Quiz>("quiz_db", new MongoCollectionSettings { AssignIdOnInsert = true });
    }

    public void CreateQuiz(QuizRecord quizRecord)
    {
        var quiz = new Quiz
        {
            Title = quizRecord.title,
            Description = quizRecord.description,
            Categories = new List<Category>(),
            Questions = new List<Question>(),
        };

        _quiz.InsertOne(quiz);
    }

    public List<QuizRecord> GetAllQuizzes()
    {
        var filter = Builders<Quiz>.Filter.Empty;

        var allquizzes = _quiz
            .Find(filter).ToList()
            .Select(q => new QuizRecord(
                    q.Id.ToString(),
                    q.Title,
                    q.Description,
                    q.Categories.Select(c => c.ToString()).ToArray(),
                    q.Questions.Select(question => question.ToString()).ToArray()
                )
            );

        return allquizzes.ToList();
    }

    public List<QuestionRecord> DisplayQuestionsInQuiz(string quizId)
    {
        var filter = Builders<Quiz>.Filter.Eq("_id", ObjectId.Parse(quizId));
        var quiz = _quiz.Find(filter).FirstOrDefault();

        if (quiz == null)
        {
            return null;
        }

        List<Question> questions = quiz.Questions;

        List<QuestionRecord> questionsRecord = new();

        foreach (var q in questions)
        {
            questionsRecord.Add(new QuestionRecord(
                id: q.Id.ToString(),
                category: q.Category.CategoryName,
                questionText: q.QuestionText,
                correctAnswer: q.CorrectAnswer,
                incorrectAnswers: q.IncorrectAnswers
            ));
        }
        return questionsRecord;
    }

    public void AddQuestionToSelectedQuiz(string quizId, string questionId)
    {
        var quizFilter = Builders<Quiz>.Filter.Eq("_id", ObjectId.Parse(quizId));
        var quiz = _quiz.Find(quizFilter).FirstOrDefault();

        var question = _question.GetQuestionById(questionId);

        if (quiz is null || question is null)
        {
            return;
        }

        var update = Builders<Quiz>.Update.AddToSet("Questions", question);

        _quiz.UpdateOne(quizFilter, update);
    }

    public void RemoveQuestionFromSelectedQuiz(string quizId, string questionId)
    {
        var quizFilter = Builders<Quiz>.Filter.Eq("_id", ObjectId.Parse(quizId));
        var questionFilter = Builders<Question>.Filter.Eq("_id", ObjectId.Parse(questionId));

        var update = Builders<Quiz>.Update.PullFilter("Questions", questionFilter);

        if (quizFilter is null || questionFilter is null)
        {
            return;
        }

        _quiz.UpdateOne(quizFilter, update);
    }

    public void DeleteQuiz(string selectedId)
    {
        var filter = Builders<Quiz>.Filter.Eq("_id", ObjectId.Parse(selectedId));

        if (filter is null)
        {
            return;
        }

        _quiz.DeleteOne(filter);
    }
}