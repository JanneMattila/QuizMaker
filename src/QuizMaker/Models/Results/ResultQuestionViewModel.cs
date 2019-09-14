using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace QuizMaker.Models.Results
{
    public class ResultQuestionViewModel
    {
        [JsonPropertyName("questionId")]
        public string ID { get; set; } = string.Empty;

        [JsonPropertyName("questionTitle")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("answers")]
        public List<ResultQuestionAnswerViewModel> Answers { get; set; }

        public ResultQuestionViewModel()
        {
            Answers = new List<ResultQuestionAnswerViewModel>();
        }
    }
}
