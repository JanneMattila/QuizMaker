using QuizMaker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizMaker.Data
{
    public interface IQuizDataContext
    {
        Task<QuizEntity> ActivateQuizAsync(string id);
        Task<QuizEntity> GetActiveQuizAsync();
        Task<QuizEntity> GetQuizAsync(string id);
        Task<List<QuizResponseEntity>> GetQuizResponsesAsync(string id);
        void Initialize();
        Task<bool> UserHasResponseAsync(string quizID, string userID);
        Task UpsertResponseAsync(QuizResponseViewModel quizResponse);
        Task<List<QuizEntity>> GetQuizzesAsync();
    }
}
