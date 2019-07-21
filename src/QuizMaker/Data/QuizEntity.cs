using Microsoft.Azure.Cosmos.Table;

namespace QuizMaker.Data
{
    public class QuizEntity : TableEntity
    {
        public QuizEntity()
        {
        }

        public QuizEntity(string quizKey, string quizQuestionKey) : base(quizKey, quizQuestionKey)
        {
        }

        public string Json { get; set; }
    }
}
