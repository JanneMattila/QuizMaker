using Microsoft.Azure.Cosmos.Table;
using System.Threading.Tasks;

namespace QuizMaker.Data
{
    public class QuizDataContext : IQuizDataContext
    {
        private readonly CloudStorageAccount _cloudStorageAccount;
        private readonly CloudTable _usersTable;
        private readonly CloudTable _quizzesTable;
        private readonly CloudTable _quizResponsesTable;

        public QuizDataContext(QuizDataContextOptions options)
        {
            _cloudStorageAccount = CloudStorageAccount.Parse(options.StorageConnectionString);
            var tableClient = _cloudStorageAccount.CreateCloudTableClient();
            _usersTable = tableClient.GetTableReference(TableNames.Users);
            _quizzesTable = tableClient.GetTableReference(TableNames.Quizzes);
            _quizResponsesTable = tableClient.GetTableReference(TableNames.QuizResponses);
        }

        public void Initialize()
        {
            _usersTable.CreateIfNotExists();
            _quizzesTable.CreateIfNotExists();
            _quizResponsesTable.CreateIfNotExists();
        }

        public async Task<QuizEntity> GetActiveQuizAsync()
        {
            const string active = "active";
            var retrieveUserOperation = TableOperation.Retrieve<QuizEntity>(active, active);
            var result = await _quizzesTable.ExecuteAsync(retrieveUserOperation);
            return result.Result as QuizEntity;
        }
    }
}
