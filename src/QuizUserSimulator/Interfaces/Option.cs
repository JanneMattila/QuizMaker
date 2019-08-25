using System.Text.Json.Serialization;

namespace QuizUserSimulator.Interfaces
{
    public class Option
    {
        [JsonPropertyName("optionId")]
        public string OptionId { get; set; }

        [JsonPropertyName("optionText")]
        public string OptionText { get; set; }
    }
}
