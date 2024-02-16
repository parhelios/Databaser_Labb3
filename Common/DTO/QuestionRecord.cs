namespace Common.DTO;

public record QuestionRecord(string id, string category, string questionText, string correctAnswer, string[] incorrectAnswers);