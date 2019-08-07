using Microsoft.AspNetCore.SignalR;
using QuizMaker.Data;
using QuizMaker.Models;
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
            // TODO: Central counter management
            var counter = 2;
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
            // TODO: Central counter management
            var counter = 1;
            await Clients.All.Disconnected(new ConnectionViewModel()
            {
                Counter = counter
            });

            await base.OnDisconnectedAsync(exception);
        }

        public async Task QuizResponse(QuizResponseViewModel quizResponse)
        {
            await _quizDataContext.UpsertResponseAsync(quizResponse);
            await Clients.Caller.Quiz(QuizViewModel.CreateBlank());

            // Submit this response to the reports view
            var resultsBuilder = new QuizResultsBuilder(_quizDataContext);
            var results = await resultsBuilder.GetResultsAsync(quizResponse.ID);

            await _quizResultsHub.Clients.Group(quizResponse.ID).SendAsync("Results", results);
        }

        private async Task SendActiveQuizAsync()
        {
            var activeQuiz = await _quizDataContext.GetActiveQuizAsync();
            if (activeQuiz != null)
            {
                var quiz = QuizViewModel.FromJson(activeQuiz.Json);
                await Clients.All.Quiz(quiz);
            }
        }
    }
}
