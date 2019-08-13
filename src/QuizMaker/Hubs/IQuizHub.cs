using QuizMaker.Models;
using QuizMaker.Models.Quiz;
using System.Threading.Tasks;

namespace QuizMaker.Hubs
{
    public interface IQuizHub
    {
        Task Connected(ConnectionViewModel connection);
        Task Disconnected(ConnectionViewModel connection);
        Task Quiz(QuizViewModel quiz);
    }
}
