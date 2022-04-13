using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace QuizMaker.Models.Results;

public class ResultViewModel
{
    [JsonPropertyName("quizId")]
    public string ID { get; set; } = string.Empty;

    [JsonPropertyName("quizTitle")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("responses")]
    public int Responses { get; set; } = 0;

    [JsonPropertyName("results")]
    public List<ResultQuestionViewModel> Results { get; set; }

    public ResultViewModel()
    {
        Results = new List<ResultQuestionViewModel>();
    }
}
