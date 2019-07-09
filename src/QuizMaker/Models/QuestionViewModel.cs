using System.Collections.Generic;

namespace QuizMaker.Models
{
    public class QuestionViewModel
    {
        public string ID { get; set; }
        public string Title { get; set; }

        public List<OptionViewModel> Options { get; set; }

        public QuestionViewModel()
        {
            Options = new List<OptionViewModel>();
        }
    }
}
