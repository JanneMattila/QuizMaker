using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace QuizMaker.Models
{
    public class QuestionResponseViewModel
    {
        [JsonPropertyName("questionId")]
        public string ID { get; set; }

        [JsonPropertyName("options")]
        public List<string> Options { get; set; }

        public QuestionResponseViewModel()
        {
            Options = new List<string>();
        }
    }
}
