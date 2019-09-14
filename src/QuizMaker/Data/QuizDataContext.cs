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
        private const string Connections = "connections";
        private const string Quizzes = "quizzes";

        private readonly CloudStorageAccount _cloudStorageAccount;
        private readonly CloudTable _connectionsTable;
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
            _connectionsTable = tableClient.GetTableReference(TableNames.Connections);
            _quizzesTable = tableClient.GetTableReference(TableNames.Quizzes);
            _quizResponsesTable = tableClient.GetTableReference(TableNames.QuizResponses);
        }

        public void Initialize()
        {
            _connectionsTable.CreateIfNotExists();
            _quizResponsesTable.CreateIfNotExists();
            if (_quizzesTable.CreateIfNotExists())
            {
                AddExampleQuizzes();
            }
        }

        private void AddExampleQuizzes()
        {
            var exampleQuizzes = Path.Combine("Data", "Quizzes.json");
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

        public async Task<QuizEntity?> GetActiveQuizAsync()
        {
            var retrieveUserOperation = TableOperation.Retrieve<QuizEntity>(Active, Active);
            var result = await _quizzesTable.ExecuteAsync(retrieveUserOperation);
            return result.Result as QuizEntity;
        }

        public async Task<QuizEntity?> GetQuizAsync(string id)
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
            var token = new TableContinuationToken();

            do
            {
                var result = await _quizResponsesTable.ExecuteQuerySegmentedAsync<QuizResponseEntity>(query, token);
                list.AddRange(result);
                token = result.ContinuationToken;
            } while (token != null);

            return list;
        }

        public async Task<QuizEntity?> ActivateQuizAsync(string id)
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

            var entity = QuizViewModel.FromJson(quizEntity.Json);
            if (entity.Questions.Any(q => q.Parameters.ClearOnActivation))
            {
                await DeleteResponsesAsync(entity.ID);
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
            var token = new TableContinuationToken();

            do
            {
                var result = await _quizzesTable.ExecuteQuerySegmentedAsync<QuizEntity>(query, token);
                list.AddRange(result);
                token = result.ContinuationToken;
            } while (token != null);

            return list;
        }

        public async Task<int> UpsertServerConnectionsAsync(int count)
        {
            var entity = new ConnectionEntity(Connections, Environment.MachineName)
            {
                Count = count
            };
            var upsertOperation = TableOperation.InsertOrReplace(entity);
            await _connectionsTable.ExecuteAsync(upsertOperation);
            return await GetConnectionCountAsync();
        }

        public async Task<int> DeleteServerConnectionsAsync()
        {
            var entity = new ConnectionEntity(Connections, Environment.MachineName)
            {
                ETag = "*"
            };
            var deleteOperation = TableOperation.Delete(entity);

            try
            {
                await _connectionsTable.ExecuteAsync(deleteOperation);
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode != 404)
                {
                    throw;
                }
            }
            return await GetConnectionCountAsync();
        }

        public async Task<int> GetConnectionCountAsync()
        {
            var query = new TableQuery<ConnectionEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, Connections));
            var token = new TableContinuationToken();
            var count = 0;
            do
            {
                var result = await _connectionsTable.ExecuteQuerySegmentedAsync(query, token);
                foreach (var entity in result)
                {
                    if (entity.Timestamp < DateTime.UtcNow.AddMinutes(-2))
                    {
                        var deleteOperation = TableOperation.Delete(entity);

                        try
                        {
                            await _connectionsTable.ExecuteAsync(deleteOperation);
                        }
                        catch (StorageException ex)
                        {
                            if (ex.RequestInformation.HttpStatusCode != 404)
                            {
                                throw;
                            }
                        }
                    }
                    else
                    {
                        count += entity.Count;
                    }
                }

                token = result.ContinuationToken;
            } while (token != null);

            return count;
        }
        
        public async Task CreateQuizAsync(QuizViewModel quiz)
        {
            if (quiz == null)
            {
                throw new ArgumentNullException(nameof(quiz));
            }
            var quizEntity = new QuizEntity(Quizzes, quiz.ID)
            {
                Json = JsonSerializer.Serialize(quiz)
            };
            var upsertOperation = TableOperation.Insert(quizEntity);
            await _quizzesTable.ExecuteAsync(upsertOperation);
        }

        public async Task DeleteAllResponsesAsync()
        {
            await _quizResponsesTable.DeleteAsync();
            await _quizResponsesTable.CreateIfNotExistsAsync();
        }

        public async Task DeleteResponsesAsync(string id)
        {
            var query = new TableQuery<QuizResponseEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id));
            var token = new TableContinuationToken();

            do
            {
                var result = await _quizResponsesTable.ExecuteQuerySegmentedAsync(query, token);
                var tableBatchOperation = new TableBatchOperation();
                foreach (var entity in result)
                {
                    var deleteOperation = TableOperation.Delete(entity);
                    tableBatchOperation.Add(deleteOperation);
                }

                if (tableBatchOperation.Any())
                {
                    await _quizResponsesTable.ExecuteBatchAsync(tableBatchOperation);
                }

                token = result.ContinuationToken;
            } while (token != null);
        }
    }
}
