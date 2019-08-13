using QuizMaker.Models.Quiz;
using QuizMaker.Models.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizMaker.Data
{
    public class QuizResultBuilder
    {
        private readonly IQuizDataContext _quizDataContext;

        public QuizResultBuilder(IQuizDataContext quizDataContext)
        {
            _quizDataContext = quizDataContext;
        }

        public async Task<ResultViewModel> GetResultsAsync(string id)
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

            var results = new ResultViewModel
            {
                ID = quiz.ID,
                Title = quiz.Title
            };

            foreach (var question in quiz.Questions)
            {
                var list = new List<ResultQuestionAnswerViewModel>();
                foreach (var option in question.Options)
                {
                    var key = $"{question.ID}={option.OptionId}";
                    var count = responses.ContainsKey(key) ? responses[key] : 0;

                    var row = new ResultQuestionAnswerViewModel()
                    {
                        Name = option.OptionText,
                        Count = count
                    };

                    list.Add(row);
                }

                results.Results.Add(new ResultQuestionViewModel()
                {
                    ID = question.ID,
                    Title = question.Title,
                    Answers = list
                });
            }

            return results;
        }
    }
}
