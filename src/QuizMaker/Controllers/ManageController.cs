using Microsoft.AspNetCore.Mvc;

namespace QuizMaker.Controllers
{
    public class ManageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
