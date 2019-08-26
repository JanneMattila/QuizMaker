using Microsoft.AspNetCore.SignalR;
using QuizMaker.Data;
using QuizMaker.Models;
using QuizMaker.Models.Quiz;
using QuizMaker.Models.Responses;
using System;
using System.Threading.Tasks;

namespace QuizMaker.Hubs
{
    public class QuizHub : Hub<IQuizHub>
    {
        private readonly IQuizDataContext _quizDataContext;
        private readonly IHubContext<QuizResultsHub> _quizResultsHub;

        public QuizHub(IQuizDataContext quizDataContext, IHubContext<QuizResultsHub> quizResultsHub)
        {
            _quizDataContext = quizDataContext;
            _quizResultsHub = quizResultsHub;
        }

        public override async Task OnConnectedAsync()
        {
            var counter = await _quizDataContext.UpsertUserAsync(Context.ConnectionId);
            await Clients.All.Connected(new ConnectionViewModel()
            {
                Counter = counter
            });

            var activeQuiz = await _quizDataContext.GetActiveQuizAsync();
            if (activeQuiz != null)
            {
                var quiz = QuizViewModel.FromJson(activeQuiz.Json);
                var userHasResponded = await _quizDataContext.UserHasResponseAsync(quiz.ID, this.Context.UserIdentifier);
                if (!userHasResponded)
                {
                    await Clients.Caller.Quiz(quiz);
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var counter = await _quizDataContext.DeleteUserAsync(Context.ConnectionId);
            await Clients.All.Disconnected(new ConnectionViewModel()
            {
                Counter = counter
            });

            await base.OnDisconnectedAsync(exception);
        }

        public async Task QuizResponse(ResponseViewModel quizResponse)
        {
            if (quizResponse == null)
            {
                throw new ArgumentNullException(nameof(quizResponse));
            }

            await _quizDataContext.UpsertResponseAsync(quizResponse);
            await Clients.Caller.Quiz(QuizViewModel.CreateBlank());

            // Submit this response to the reports view
            var resultsBuilder = new QuizResultBuilder(_quizDataContext);
            var results = await resultsBuilder.GetResultsAsync(quizResponse.ID);

            await _quizResultsHub.Clients.Group(quizResponse.ID).SendAsync("Results", results);
            await _quizResultsHub.Clients.Group("active").SendAsync("Results", results);
        }
    }
}
