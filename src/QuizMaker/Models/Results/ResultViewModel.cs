using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace QuizMaker.Models.Results
{
    public class ResultViewModel
    {
        [JsonPropertyName("quizId")]
        public string ID { get; set; }

        [JsonPropertyName("quizTitle")]
        public string Title { get; set; }

        [JsonPropertyName("responses")]
        public int Responses { get; set; }

        [JsonPropertyName("results")]
        public List<ResultQuestionViewModel> Results { get; set; }

        public ResultViewModel()
        {
            Results = new List<ResultQuestionViewModel>();
        }
    }
}
