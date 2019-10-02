using QuizMaker.Models.Quiz;
using QuizMaker.Models.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizMaker.Data
{
    public interface IQuizDataContext
    {
        Task<QuizViewModel?> ActivateQuizAsync(string id);
        Task CreateQuizAsync(QuizViewModel quiz);
        Task DeleteAllResponsesAsync();
        Task DeleteResponsesAsync(string id);
        Task<int> DeleteServerConnectionsAsync();
        Task<QuizViewModel?> GetActiveQuizAsync();
        Task<int> GetConnectionCountAsync();
        Task<QuizViewModel?> GetQuizAsync(string id);
        IAsyncEnumerable<QuizViewModel> GetQuizzesAsync();
        IAsyncEnumerable<string> GetQuizResponsesAsync(string id);
        void Initialize();
        Task<bool> UserHasResponseAsync(string quizID, string userID);
        Task<int> UpsertServerConnectionsAsync(int count);
        Task UpsertResponseAsync(ResponseViewModel quizResponse);
    }
}
