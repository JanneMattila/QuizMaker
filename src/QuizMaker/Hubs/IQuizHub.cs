using QuizMaker.Models;
using System.Threading.Tasks;

namespace QuizMaker.Hubs
{
    public interface IQuizHub
    {
        Task Connected(ConnectionViewModel connection);
        Task Disconnected(ConnectionViewModel connection);
        Task Quiz(QuizViewModel quiz);
        Task SendQuizResponse(QuizViewModel quiz);
    }
}
