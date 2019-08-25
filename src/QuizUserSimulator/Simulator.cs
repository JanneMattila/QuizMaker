using Microsoft.AspNetCore.SignalR.Client;
using QuizUserSimulator.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace QuizUserSimulator
{
    public class Simulator
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private ConcurrentQueue<Quiz> _quizzes = new ConcurrentQueue<Quiz>();
        private DateTime _responseTime = DateTime.MinValue;
        private Random _random = new Random();
        private string _userID = Guid.NewGuid().ToString("B");

        public Simulator()
        {
            Console.CancelKeyPress += (sender, a) =>
            {
                a.Cancel = true;
                Console.WriteLine("Closing down...");
                _cancellationTokenSource.Cancel();
            };
        }

        public async Task SimulateUserAsync(string quizHubEndpoint)
        {
            Console.WriteLine("Starting quiz user simulator");
            var quizHubUri = new Uri(quizHubEndpoint);
            var connection = new HubConnectionBuilder()
                .WithUrl(quizHubEndpoint, options =>
                {
                    options.Cookies.Add(new System.Net.Cookie()
                    {
                        Name = "QuizUserId",
                        Value = _userID,
                        Domain = quizHubUri.Host
                    });
                    options.AccessTokenProvider = () => Task.FromResult(_userID);
                })
                .WithAutomaticReconnect()
                .Build();

            connection.On<Quiz>("Quiz", QuizReceived);

            await connection.StartAsync(_cancellationTokenSource.Token);
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                if (_responseTime < DateTime.Now &&
                    _quizzes.TryDequeue(out Quiz quiz))
                {
                    if (quiz.Questions.Count > 0)
                    {
                        Console.WriteLine($"Submitting response to '{quiz.Title}'");
                        var response = new Response
                        {
                            ID = quiz.ID,
                            UserID = _userID
                        };

                        foreach (var question in quiz.Questions)
                        {
                            var selectedOption = question.Options[_random.Next(question.Options.Count)];
                            var responseQuestion = new ResponseQuestion()
                            {
                                ID = question.ID,
                            };
                            responseQuestion.Options.Add(selectedOption.OptionId);
                            response.Responses.Add(responseQuestion);

                            Console.WriteLine($"For question '{question.Title}' my answer is '{selectedOption.OptionText}'.");
                        }

                        await connection.InvokeAsync<Response>("QuizResponse", response, _cancellationTokenSource.Token);
                    }
                }

                await Task.Delay(500, _cancellationTokenSource.Token);
            }
        }

        private void QuizReceived(Quiz quiz)
        {
            Console.WriteLine($"Quiz received: '{quiz.Title}'");
            _quizzes.Enqueue(quiz);
            _responseTime = DateTime.Now.AddMilliseconds(_random.Next(100, 4000));
        }
    }
}
