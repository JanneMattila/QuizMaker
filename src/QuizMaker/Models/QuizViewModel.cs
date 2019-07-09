using System.Collections.Generic;

namespace QuizMaker.Models
{
    public class QuizViewModel
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Json { get; set; }

        public List<QuestionViewModel> Questions { get; set; }

        public QuizViewModel()
        {
            Questions = new List<QuestionViewModel>();
        }
    }
}
