using QuizMaker.Models;
using QuizMaker.Models.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizMaker.Data
{
    public interface IQuizDataContext
    {
        Task<QuizEntity> ActivateQuizAsync(string id);
        Task<int> DeleteUserAsync(string connectionId);
        Task<QuizEntity> GetActiveQuizAsync();
        Task<QuizEntity> GetQuizAsync(string id);
        Task<List<QuizResponseEntity>> GetQuizResponsesAsync(string id);
        void Initialize();
        Task<bool> UserHasResponseAsync(string quizID, string userID);
        Task<int> UpsertUserAsync(string connectionId);
        Task UpsertResponseAsync(ResponseViewModel quizResponse);
        Task<List<QuizEntity>> GetQuizzesAsync();
    }
}
