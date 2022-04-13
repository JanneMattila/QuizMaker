using System.Threading.Tasks;
using QuizMaker.Models;
using QuizMaker.Models.Quiz;

namespace QuizMaker.Hubs;

public interface IQuizHub
{
    Task Connected(ConnectionViewModel connection);
    Task Disconnected(ConnectionViewModel connection);
    Task Quiz(QuizViewModel quiz);
}
