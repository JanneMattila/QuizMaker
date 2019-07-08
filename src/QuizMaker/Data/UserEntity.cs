using Microsoft.Azure.Cosmos.Table;

namespace QuizMaker.Data
{
    public class UserEntity : TableEntity
    {
        public UserEntity(string userKey) : base(userKey, userKey)
        {
        }
    }
}
