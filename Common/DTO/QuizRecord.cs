namespace Common.DTO;

public record QuizRecord(string id, string title, string description, string[] categories, string[] questions);