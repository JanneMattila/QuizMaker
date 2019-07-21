using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace QuizMaker.Models
{
    public class QuizViewModel
    {
        [JsonPropertyName("quizId")]
        public string ID { get; set; }

        [JsonPropertyName("quizTitle")]
        public string Title { get; set; }

        [JsonPropertyName("userId")]
        public string UserID { get; set; }

        [JsonPropertyName("questions")]
        public List<QuestionViewModel> Questions { get; set; }

        [JsonIgnore]
        public string Json { get; set; }

        public QuizViewModel()
        {
            Questions = new List<QuestionViewModel>();
        }
    }
}
