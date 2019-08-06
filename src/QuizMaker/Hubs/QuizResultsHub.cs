using Microsoft.AspNetCore.SignalR;
using QuizMaker.Data;
using QuizMaker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizMaker.Hubs
{
    public class QuizResultsHub : Hub
    {
        private readonly IQuizDataContext _quizDataContext;

        public QuizResultsHub(IQuizDataContext quizDataContext)
        {
            _quizDataContext = quizDataContext;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task Results(string id)
        {
            var quiz = await _quizDataContext.GetQuizAsync(id);
            var quizModel = QuizViewModel.FromJson(quiz.Json);

            var quizResponses = await _quizDataContext.GetQuizResponsesAsync(id);
        }
    }
}
