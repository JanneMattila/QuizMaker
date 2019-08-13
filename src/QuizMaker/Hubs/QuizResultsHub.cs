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

        [HubMethodName("GetResults")]
        public async Task GetResultsAsync(string id)
        {
            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, id);

            var resultsBuilder = new QuizResultBuilder(_quizDataContext);
            var results = await resultsBuilder.GetResultsAsync(id);

            await Clients.Caller.SendAsync("Results", results);
        }
    }
}
