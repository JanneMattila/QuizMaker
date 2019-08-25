using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace QuizUserSimulator.Interfaces
{
    public class ResponseQuestion
    {
        [JsonPropertyName("questionId")]
        public string ID { get; set; }

        [JsonPropertyName("options")]
        public List<string> Options { get; }

        public ResponseQuestion()
        {
            Options = new List<string>();
        }
    }
}
