﻿using System.Collections.Generic;
using System.Threading.Tasks;
using QuizMaker.Models.Quiz;
using QuizMaker.Models.Results;

namespace QuizMaker.Data;

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
        var quiz = quizEntity != null ?
            QuizViewModel.FromJson(quizEntity.Json) :
            QuizViewModel.CreateBlank();

        var results = new ResultViewModel
        {
            ID = quiz.ID,
            Title = quiz.Title
        };

        if (quizEntity != null)
        {
            var responseCount = 0;
            var responses = new Dictionary<string, int>();
            await foreach (var response in _quizDataContext.GetQuizResponsesAsync(id))
            {
                // Process: "q1=option1,option2;q2=optionA"
                responseCount++;
                var userResponses = response.Split(';');
                foreach (var userResponse in userResponses)
                {
                    var userResponseString = userResponse.Split('=');
                    var key = userResponseString[0];
                    var options = userResponseString[1].Split(',');

                    foreach (var option in options)
                    {
                        var answerKey = $"{key}={option}";
                        if (responses.ContainsKey(answerKey))
                        {
                            responses[answerKey]++;
                        }
                        else
                        {
                            responses[answerKey] = 1;
                        }
                    }
                }
            }

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

            results.Responses = responseCount;
        }

        return results;
    }
}
