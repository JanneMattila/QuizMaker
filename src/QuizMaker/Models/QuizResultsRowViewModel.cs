using System.Text.Json.Serialization;

namespace QuizMaker.Models
{
    public class QuizResultsRowViewModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("count")]
        public double Count { get; set; }
    }
}
