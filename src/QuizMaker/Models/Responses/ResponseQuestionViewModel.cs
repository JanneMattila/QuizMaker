using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace QuizMaker.Models.Responses
{
    public class ResponseQuestionViewModel
    {
        [JsonPropertyName("questionId")]
        public string ID { get; set; }

        [JsonPropertyName("options")]
        public List<string> Options { get; set; }

        public ResponseQuestionViewModel()
        {
            Options = new List<string>();
        }
    }
}
