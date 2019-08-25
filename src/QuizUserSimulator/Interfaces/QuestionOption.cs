using System.Text.Json.Serialization;

namespace QuizUserSimulator.Interfaces
{
    public class QuestionOption
    {
        [JsonPropertyName("optionId")]
        public string OptionId { get; set; }

        [JsonPropertyName("optionText")]
        public string OptionText { get; set; }
    }
}
