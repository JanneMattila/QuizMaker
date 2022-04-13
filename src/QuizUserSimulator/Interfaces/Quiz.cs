using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace QuizUserSimulator.Interfaces;

public class Quiz
{
    [JsonPropertyName("quizId")]
    public string ID { get; set; }

    [JsonPropertyName("quizTitle")]
    public string Title { get; set; }

    [JsonPropertyName("userId")]
    public string UserID { get; set; }

    [JsonPropertyName("questions")]
    public List<Question> Questions { get; set; }
}
