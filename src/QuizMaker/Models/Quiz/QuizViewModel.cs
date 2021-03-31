using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuizMaker.Models.Quiz
{
    public class QuizViewModel
    {
        [JsonPropertyName("quizId")]
        public string ID { get; set; } = string.Empty;

        [JsonPropertyName("quizTitle")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("userId")]
        public string UserID { get; set; } = string.Empty;

        [JsonPropertyName("questions")]
        public List<QuestionViewModel> Questions { get; set; }

        [JsonIgnore]
        public string Json { get; set; } = string.Empty;

        public QuizViewModel()
        {
            Questions = new List<QuestionViewModel>();
        }

        public static QuizViewModel FromJson(string json)
        {
            var model = JsonSerializer.Deserialize<QuizViewModel>(json);
            if (model != null)
            {
                model.Json = json;
                return model;
            }
            throw new ArgumentException("Not valid model payload", nameof(json));
        }

        public static QuizViewModel CreateBlank()
        {
            return new QuizViewModel()
            {
                Title = "Quiz",
                ID = Guid.Empty.ToString()
            };
        }
    }
}
