using System.Text.Json.Serialization;

namespace QuizMaker.Models.Quiz
{
    public class ParametersViewModel
    {
        [JsonPropertyName("randomizeOrder")]
        public bool RandomizeOrder { get; set; }

        [JsonPropertyName("multiSelect")]
        public bool MultiSelect { get; set; }
    }
}
