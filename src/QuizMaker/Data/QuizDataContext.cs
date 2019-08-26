﻿using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using QuizMaker.Models.Responses;
using System.IO;
using System.Text.Json;
using QuizMaker.Models.Quiz;
using System;

namespace QuizMaker.Data
{
    public class QuizDataContext : IQuizDataContext
    {
        private const string Active = "active";
        private const string Users = "users";
        private const string Quizzes = "quizzes";

        private readonly CloudStorageAccount _cloudStorageAccount;
        private readonly CloudTable _usersTable;
        private readonly CloudTable _quizzesTable;
        private readonly CloudTable _quizResponsesTable;

        public QuizDataContext(QuizDataContextOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _cloudStorageAccount = CloudStorageAccount.Parse(options.StorageConnectionString);
            var tableClient = _cloudStorageAccount.CreateCloudTableClient();
            _usersTable = tableClient.GetTableReference(TableNames.Users);
            _quizzesTable = tableClient.GetTableReference(TableNames.Quizzes);
            _quizResponsesTable = tableClient.GetTableReference(TableNames.QuizResponses);
        }

        public void Initialize()
        {
            _usersTable.CreateIfNotExists();
            _quizResponsesTable.CreateIfNotExists();
            if (_quizzesTable.CreateIfNotExists())
            {
                AddExampleQuizzes();
            }
        }

        private void AddExampleQuizzes()
        {
            var exampleQuizzes = @"Data\Quizzes.json";
            if (File.Exists(exampleQuizzes))
            {
                var json = File.ReadAllText(exampleQuizzes);
                var quizzes = JsonSerializer.Deserialize<QuizViewModel[]>(json);
                foreach (var quiz in quizzes)
                {
                    UpsertQuiz(quiz);
                }
            }
        }

        public async Task<QuizEntity> GetActiveQuizAsync()
        {
            var retrieveUserOperation = TableOperation.Retrieve<QuizEntity>(Active, Active);
            var result = await _quizzesTable.ExecuteAsync(retrieveUserOperation);
            return result.Result as QuizEntity;
        }

        public async Task<QuizEntity> GetQuizAsync(string id)
        {
            var retrieveOperation = TableOperation.Retrieve<QuizEntity>(Quizzes, id);
            var result = await _quizzesTable.ExecuteAsync(retrieveOperation);
            return result.Result as QuizEntity;
        }

        public async Task<List<QuizResponseEntity>> GetQuizResponsesAsync(string id)
        {
            var list = new List<QuizResponseEntity>();
            var query = new TableQuery<QuizResponseEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id));
            TableContinuationToken token = null;

            do
            {
                var result = await _quizResponsesTable.ExecuteQuerySegmentedAsync<QuizResponseEntity>(query, token);
                list.AddRange(result);
            } while (token != null);

            return list;
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
                if (activeQuizResult.Result is QuizEntity activeQuiz)
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

        public async Task<bool> UserHasResponseAsync(string quizID, string userID)
        {
            var retrieveOperation = TableOperation.Retrieve(quizID, userID);
            var retrieveResult = await _quizResponsesTable.ExecuteAsync(retrieveOperation);
            return retrieveResult.HttpStatusCode == (int)HttpStatusCode.OK;
        }

        public async Task UpsertResponseAsync(ResponseViewModel quizResponse)
        {
            if (quizResponse == null)
            {
                throw new ArgumentNullException(nameof(quizResponse));
            }
            var quizResponseEntity = new QuizResponseEntity(quizResponse.ID, quizResponse.UserID)
            {
                Response = string.Join(';', quizResponse.Responses.Select(r => $"{r.ID}={string.Join(',', r.Options)}"))
            };
            var upsertOperation = TableOperation.InsertOrReplace(quizResponseEntity);
            var upsertResult = await _quizResponsesTable.ExecuteAsync(upsertOperation);
        }

        public void UpsertQuiz(QuizViewModel quiz)
        {
            if (quiz == null)
            {
                throw new ArgumentNullException(nameof(quiz));
            }
            var quizEntity = new QuizEntity(Quizzes, quiz.ID)
            {
                Json = JsonSerializer.Serialize(quiz)
            };
            var upsertOperation = TableOperation.InsertOrReplace(quizEntity);
            _quizzesTable.Execute(upsertOperation);
        }

        public async Task<List<QuizEntity>> GetQuizzesAsync()
        {
            var list = new List<QuizEntity>();
            var query = new TableQuery<QuizEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, Quizzes)); 
            TableContinuationToken token = null;

            do
            {
                var result = await _quizzesTable.ExecuteQuerySegmentedAsync<QuizEntity>(query, token);
                list.AddRange(result);
            } while (token != null);

            return list;
        }

        public async Task<int> UpsertUserAsync(string connectionId)
        {
            var userEntity = new UserEntity(Users, connectionId);
            var upsertOperation = TableOperation.InsertOrReplace(userEntity);
            await _usersTable.ExecuteAsync(upsertOperation);
            return await GetUserCountAsync();
        }

        public async Task<int> DeleteUserAsync(string connectionId)
        {
            var userEntity = new UserEntity(Users, connectionId)
            {
                ETag = "*"
            };
            var deleteOperation = TableOperation.Delete(userEntity);
            await _usersTable.ExecuteAsync(deleteOperation);
            return await GetUserCountAsync();
        }

        public async Task<int> GetUserCountAsync()
        {
            var query = new TableQuery<UserEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, Users))
                .Select(new List<string> { "PartitionKey", "RowKey", "Timestamp" });
            TableContinuationToken token = null;
            var count = 0;
            do
            {
                var result = await _usersTable.ExecuteQuerySegmentedAsync(query, token);
                count += result.Count();
            } while (token != null);

            return count;
        }
    }
}
