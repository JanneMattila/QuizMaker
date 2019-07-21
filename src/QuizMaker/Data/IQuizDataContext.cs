using System.Threading.Tasks;

namespace QuizMaker.Data
{
    public interface IQuizDataContext
    {
        Task<QuizEntity> GetActiveQuizAsync();
        void Initialize();
    }
}