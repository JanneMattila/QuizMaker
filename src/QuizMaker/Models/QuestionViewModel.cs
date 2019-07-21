using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace QuizMaker.Models
{
    public class QuestionViewModel
    {
        [JsonPropertyName("questionId")]
        public string ID { get; set; }

        [JsonPropertyName("questionTitle")]
        public string Title { get; set; }

        [JsonPropertyName("options")]
        public List<OptionViewModel> Options { get; set; }

        public QuestionViewModel()
        {
            Options = new List<OptionViewModel>();
        }
    }
}
