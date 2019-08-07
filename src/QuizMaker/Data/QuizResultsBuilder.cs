using QuizMaker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizMaker.Data
{
    public class QuizResultsBuilder
    {
        private readonly IQuizDataContext _quizDataContext;

        public QuizResultsBuilder(IQuizDataContext quizDataContext)
        {
            _quizDataContext = quizDataContext;
        }

        public async Task<QuizResultsViewModel> GetResultsAsync(string id)
        {
            var quizEntity = await _quizDataContext.GetQuizAsync(id);
            var quiz = QuizViewModel.FromJson(quizEntity.Json);

            var responses = new Dictionary<string, int>();
            var quizResponses = await _quizDataContext.GetQuizResponsesAsync(id);
            foreach (var response in quizResponses)
            {
                // Process: "q1=option1,option2;q2=optionA"
                var userResponses = response.Response.Split(';');
                foreach (var userResponse in userResponses)
                {
                    if (responses.ContainsKey(userResponse))
                    {
                        responses[userResponse]++;
                    }
                    else
                    {
                        responses[userResponse] = 1;
                    }
                }
            }

            var results = new QuizResultsViewModel
            {
                ID = quiz.ID,
                Title = quiz.Title
            };

            foreach (var question in quiz.Questions)
            {
                var list = new List<QuizResultsRowViewModel>();
                foreach (var option in question.Options)
                {
                    var key = $"{question.ID}={option.OptionId}";
                    var count = responses.ContainsKey(key) ? responses[key] : 0;

                    var row = new QuizResultsRowViewModel()
                    {
                        Name = option.OptionText,
                        Count = count
                    };

                    list.Add(row);
                }

                results.Results.Add(new QuizQuestionResultsViewModel()
                {
                    ID = question.ID,
                    Title = question.Title,
                    Results = list
                });
            }

            return results;
        }
    }
}
