using Microsoft.AspNetCore.SignalR;
using QuizMaker.Data;
using QuizMaker.Models;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace QuizMaker.Hubs
{
    public class QuizHub : Hub
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
            await Clients.All.SendAsync(HubConstants.ConnectedMethod, counter);

            var activeQuiz = await _quizDataContext.GetActiveQuizAsync();
            if (activeQuiz != null)
            {
                var quiz = JsonSerializer.Parse<QuizViewModel>(activeQuiz.Json);
                await Clients.Caller.SendAsync(HubConstants.QuizMethod, quiz);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // TODO: Central counter management
            var counter = 1;
            await Clients.All.SendAsync(HubConstants.DisconnectedMethod, counter);
        }
    }
}
