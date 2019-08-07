using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace QuizMaker.Models
{
    public class QuizResultsViewModel
    {
        [JsonPropertyName("quizId")]
        public string ID { get; set; }

        [JsonPropertyName("quizTitle")]
        public string Title { get; set; }

        [JsonPropertyName("results")]
        public List<QuizQuestionResultsViewModel> Results { get; set; }

        public QuizResultsViewModel()
        {
            Results = new List<QuizQuestionResultsViewModel>();
        }
    }
}
