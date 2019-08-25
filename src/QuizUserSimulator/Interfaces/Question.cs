using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace QuizUserSimulator.Interfaces
{
    public class Question
    {
        [JsonPropertyName("questionId")]
        public string ID { get; set; }

        [JsonPropertyName("questionTitle")]
        public string Title { get; set; }

        [JsonPropertyName("options")]
        public List<Option> Options { get; set; }

        [JsonPropertyName("parameters")]
        public Parameters Parameters { get; set; }
    }
}
