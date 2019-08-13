using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using QuizMaker.Data;
using QuizMaker.Hubs;
using QuizMaker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using QuizMaker.Models.Quiz;

namespace QuizMaker.Controllers
{
    public class ManageController : Controller
    {
        private readonly IQuizDataContext _quizDataContext;
        private readonly IHubContext<QuizHub, IQuizHub> _quizHub;

        public ManageController(IQuizDataContext quizDataContext, IHubContext<QuizHub, IQuizHub> quizHub)
        {
            _quizDataContext = quizDataContext;
            _quizHub = quizHub;
        }

        public async Task<IActionResult> Index()
        {
            var list = (await _quizDataContext.GetQuizzesAsync())
                .Select(entity => QuizViewModel.FromJson(entity.Json));
            return View(list);
        }

        public IActionResult Results(string id)
        {
            return View();
        }

        public async Task<IActionResult> Activate(string id)
        {
            var activeQuiz = await _quizDataContext.ActivateQuizAsync(id);
            var quiz = activeQuiz != null ?
                QuizViewModel.FromJson(activeQuiz.Json) :
                QuizViewModel.CreateBlank();

            await _quizHub.Clients.All.Quiz(quiz);
            return View(quiz);
        }

        public async Task<IActionResult> Deactivate()
        {
            return await Activate(string.Empty);
        }

        public IActionResult Edit()
        {
            return View();
        }

        public IActionResult Details()
        {
            return View();
        }

        public IActionResult Delete()
        {
            return View();
        }
    }
}
