using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using QuizMaker.Data;
using QuizMaker.Hubs;
using System.Threading.Tasks;
using System.Linq;
using QuizMaker.Models.Quiz;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;

namespace QuizMaker.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly IQuizDataContext _quizDataContext;
        private readonly IHubContext<QuizHub, IQuizHub> _quizHub;
        private readonly IHubContext<QuizResultsHub> _quizResultsHub;

        public ManageController(IQuizDataContext quizDataContext, 
            IHubContext<QuizHub, IQuizHub> quizHub, IHubContext<QuizResultsHub> quizResultsHub)
        {
            _quizDataContext = quizDataContext;
            _quizHub = quizHub;
            _quizResultsHub = quizResultsHub;
        }

        public async Task<IActionResult> Index()
        {
            _quizDataContext.Initialize();
            var list = (await _quizDataContext.GetQuizzesAsync())
                .Select(entity => QuizViewModel.FromJson(entity.Json));
            return View(list);
        }

        public IActionResult Results()
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

            if (activeQuiz != null)
            {
                var resultsBuilder = new QuizResultBuilder(_quizDataContext);
                var results = await resultsBuilder.GetResultsAsync(quiz.ID);

                await _quizResultsHub.Clients.Group(HubConstants.Active).SendAsync(HubConstants.ResultsMethod, results);
            }

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

        public async Task<IActionResult> Details(string id)
        {
            var quiz = await GetQuizAsync(id);
            return View(quiz);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var quiz = await GetQuizAsync(id);
            return View(quiz);
        }

        private async Task<QuizViewModel> GetQuizAsync(string id)
        {
            var quizEntity = await _quizDataContext.GetQuizAsync(id);
            return quizEntity != null ?
                QuizViewModel.FromJson(quizEntity.Json) :
                QuizViewModel.CreateBlank();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuizViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (ModelState.IsValid)
            {
                await _quizDataContext.CreateQuizAsync(model);
                return RedirectToAction("Details", new { id = model.ID });
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult DeleteAllResponses()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAllResponses(IFormCollection form)
        {
            if (ModelState.IsValid)
            {
                await _quizDataContext.DeleteAllResponsesAsync();
                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<IActionResult> DeleteQuiz(string id)
        {
            var quiz = await GetQuizAsync(id);
            return View(quiz);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteQuizResponses(string id)
        {
            var quiz = await GetQuizAsync(id);
            return View(quiz);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuizResponses(string id, IFormCollection form)
        {
            await _quizDataContext.DeleteResponsesAsync(id);
            return RedirectToAction("Details", new { id });
        }

        public IActionResult Delete()
        {
            return View();
        }
    }
}
