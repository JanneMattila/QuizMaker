﻿using Microsoft.Azure.Cosmos.Table;

namespace QuizMaker.Data
{
    public class QuizEntity : TableEntity
    {
        public QuizEntity(string quizKey, string quizQuestionKey) : base(quizKey, quizQuestionKey)
        {
        }
    }
}