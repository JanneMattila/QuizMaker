using Microsoft.AspNetCore.Mvc;
using QuizMaker.Models;
using System.Collections.Generic;

namespace QuizMaker.Controllers
{
    public class ManageController : Controller
    {
        public IActionResult Index()
        {
            var list = new List<QuizViewModel>();
            return View(list);
        }

        public IActionResult Activate()
        {
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
