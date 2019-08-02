using Microsoft.AspNetCore.SignalR;

namespace QuizMaker.Hubs
{
    public class UniqueIdentifierUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            var context = connection.GetHttpContext();
            return context.Request.Query["access_token"];
        }
    }
}
