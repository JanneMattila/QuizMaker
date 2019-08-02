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

        public QuizHub(IQuizDataContext quizDataContext)
        {
            _quizDataContext = quizDataContext;
        }

        public override async Task OnConnectedAsync()
        {
            // TODO: Central counter management
            var counter = 2;
            await Clients.All.Connected(new ConnectionViewModel()
            {
                Counter = counter
            });
            await SendActiveQuizAsync();
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
