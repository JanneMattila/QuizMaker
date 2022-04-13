using System.Text.Json.Serialization;

namespace QuizMaker.Models.Results;

public class ResultQuestionAnswerViewModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("count")]
    public double Count { get; set; } = 0;
}
