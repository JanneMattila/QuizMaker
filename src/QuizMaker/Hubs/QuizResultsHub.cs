using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using QuizMaker.Data;
using QuizMaker.Models;
using QuizMaker.Models.Quiz;
using System.Threading.Tasks;

namespace QuizMaker.Hubs
{
    [Authorize]
    public class QuizResultsHub : Hub
    {
        private readonly IQuizDataContext _quizDataContext;

        public QuizResultsHub(IQuizDataContext quizDataContext)
        {
            _quizDataContext = quizDataContext;
        }

        [HubMethodName("GetResults")]
        public async Task GetResultsAsync(string id)
        {
            if (id == HubConstants.Active)
            {
                var activeQuiz = await _quizDataContext.GetActiveQuizAsync();
                if (activeQuiz != null)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, HubConstants.Active);
                    var quiz = QuizViewModel.FromJson(activeQuiz.Json);
                    id = quiz.ID;

                    var counter = await _quizDataContext.GetConnectionCountAsync();
                    await Clients.Caller.SendAsync(HubConstants.ConnectedMethod, new ConnectionViewModel()
                    {
                        Counter = counter
                    }); 
                }
            }

            await Groups.AddToGroupAsync(this.Context.ConnectionId, id);

            var resultsBuilder = new QuizResultBuilder(_quizDataContext);
            var results = await resultsBuilder.GetResultsAsync(id);

            await Clients.Caller.SendAsync(HubConstants.ResultsMethod, results);
        }
    }
}
