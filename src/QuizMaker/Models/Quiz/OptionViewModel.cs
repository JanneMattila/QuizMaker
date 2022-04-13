using System.Text.Json.Serialization;

namespace QuizMaker.Models.Quiz;

public class OptionViewModel
{
    [JsonPropertyName("optionId")]
    public string OptionId { get; set; } = string.Empty;

    [JsonPropertyName("optionText")]
    public string OptionText { get; set; } = string.Empty;
}
