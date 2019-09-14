using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace QuizMaker.Models.Responses
{
    public class ResponseViewModel
    {
        [JsonPropertyName("quizId")]
        public string ID { get; set; } = string.Empty;

        [JsonPropertyName("userId")]
        public string UserID { get; set; } = string.Empty;

        [JsonPropertyName("responses")]
        public List<ResponseQuestionViewModel> Responses { get; set; }

        public ResponseViewModel()
        {
            Responses = new List<ResponseQuestionViewModel>();
        }
    }
}
