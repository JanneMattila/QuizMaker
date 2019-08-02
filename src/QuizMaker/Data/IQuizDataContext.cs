using QuizMaker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizMaker.Data
{
    public interface IQuizDataContext
    {
        Task<QuizEntity> ActivateQuizAsync(string id);
        Task<QuizEntity> GetActiveQuizAsync();
        void Initialize();
        Task UpsertResponseAsync(QuizResponseViewModel quizResponse);
        Task<List<QuizEntity>> GetQuizzesAsync();
    }
}
