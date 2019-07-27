using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using QuizMaker.Data;
using QuizMaker.Hubs;
using QuizMaker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public IActionResult Index()
        {
            var list = new List<QuizViewModel>();
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
            return View();
        }

        public IActionResult Deactivate()
        {
            return View();
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
