using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace QuizMaker.Models
{
    public class QuizQuestionResultsViewModel
    {
        [JsonPropertyName("questionId")]
        public string ID { get; set; }

        [JsonPropertyName("questionTitle")]
        public string Title { get; set; }

        [JsonPropertyName("answers")]
        public List<QuizResultsRowViewModel> Results { get; set; }

        public QuizQuestionResultsViewModel()
        {
            Results = new List<QuizResultsRowViewModel>();
        }
    }
}
