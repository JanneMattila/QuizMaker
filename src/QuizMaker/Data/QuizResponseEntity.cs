using Microsoft.Azure.Cosmos.Table;

namespace QuizMaker.Data
{
    public class QuizResponseEntity : TableEntity
    {
        public QuizResponseEntity()
        {
        }

        public QuizResponseEntity(string quizKey, string userKey) : base(quizKey, userKey)
        {
        }

        public string Response { get; set; } = string.Empty;
    }
}
