using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace QuizMaker.Models
{
    public class QuizResponseViewModel
    {
        [JsonPropertyName("quizId")]
        public string ID { get; set; }

        [JsonPropertyName("userId")]
        public string UserID { get; set; }

        [JsonPropertyName("responses")]
        public List<QuestionResponseViewModel> Responses { get; set; }

        public QuizResponseViewModel()
        {
            Responses = new List<QuestionResponseViewModel>();
        }
    }
}
