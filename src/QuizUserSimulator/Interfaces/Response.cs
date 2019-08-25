using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace QuizUserSimulator.Interfaces
{
    public class Response
    {
        [JsonPropertyName("quizId")]
        public string ID { get; set; }

        [JsonPropertyName("userId")]
        public string UserID { get; set; }

        [JsonPropertyName("responses")]
        public List<ResponseQuestion> Responses { get; set; }

        public Response()
        {
            Responses = new List<ResponseQuestion>();
        }
    }
}
