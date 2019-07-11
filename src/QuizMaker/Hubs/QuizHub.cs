using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace QuizMaker.Hubs
{
    public class QuizHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            // TODO: Central counter management
            var counter = 2;
            await Clients.All.SendAsync(HubConstants.ConnectedMethod, counter);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // TODO: Central counter management
            var counter = 1;
            await Clients.All.SendAsync(HubConstants.DisconnectedMethod, counter);
        }
    }
}
