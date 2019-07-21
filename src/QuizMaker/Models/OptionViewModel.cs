using System.Text.Json.Serialization;

namespace QuizMaker.Models
{
    public class OptionViewModel
    {
        [JsonPropertyName("optionId")]
        public string OptionId { get; set; }

        [JsonPropertyName("optionText")]
        public string OptionText { get; set; }
    }
}
