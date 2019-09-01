using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using QuizMaker.Data;
using QuizMaker.Hubs;
using System.Threading.Tasks;
using System.Linq;
using QuizMaker.Models.Quiz;
using Microsoft.AspNetCore.Authorization;

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
            var quizEntity = await _quizDataContext.GetQuizAsync(id);
            var quiz = QuizViewModel.FromJson(quizEntity.Json);
            return View(quiz);
        }

        public IActionResult Delete()
        {
            return View();
        }
    }
}
