using Microsoft.Azure.Cosmos.Table;
using QuizMaker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizMaker.Data
{
    public class QuizDataContext : IQuizDataContext
    {
        private const string Active = "active";
        private const string Quizzes = "quizzes";

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
            var retrieveUserOperation = TableOperation.Retrieve<QuizEntity>(Active, Active);
            var result = await _quizzesTable.ExecuteAsync(retrieveUserOperation);
            return result.Result as QuizEntity;
        }

        public async Task<QuizEntity> ActivateQuizAsync(string id)
        {
            var retrieveOperation = TableOperation.Retrieve<QuizEntity>(Quizzes, id);
            var result = await _quizzesTable.ExecuteAsync(retrieveOperation);
            if (!(result.Result is QuizEntity quizEntity))
            {
                // No quiz found so clear the current active quiz
                var retrieveActiveQuizOperation = TableOperation.Retrieve<QuizEntity>(Active, Active);
                var activeQuizResult = await _quizzesTable.ExecuteAsync(retrieveActiveQuizOperation);
                var activeQuiz = activeQuizResult.Result as QuizEntity;
                if (activeQuiz != null)
                {
                    var deleteOperation = TableOperation.Delete(activeQuiz);
                    await _quizzesTable.ExecuteAsync(deleteOperation);
                }
                return null;
            }

            quizEntity.PartitionKey = quizEntity.RowKey = Active;
            var upsertOperation = TableOperation.InsertOrReplace(quizEntity);
            var upsertResult = await _quizzesTable.ExecuteAsync(upsertOperation);
            return upsertResult.Result as QuizEntity;
        }

        public async Task UpsertResponseAsync(QuizResponseViewModel quizResponse)
        {
            await Task.CompletedTask;
        }

        public async Task<List<QuizEntity>> GetQuizzesAsync()
        {
            var list = new List<QuizEntity>();
            var query = new TableQuery<QuizEntity>();
            TableContinuationToken token = null;

            do
            {
                var result = await _quizzesTable.ExecuteQuerySegmentedAsync<QuizEntity>(query, token);
                list.AddRange(result);
            } while (token != null);

            return list;
        }
    }
}
