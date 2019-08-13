using System.Text.Json.Serialization;

namespace QuizMaker.Models.Results
{
    public class ResultQuestionAnswerViewModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("count")]
        public double Count { get; set; }
    }
}
