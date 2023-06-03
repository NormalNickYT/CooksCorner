using Microsoft.AspNetCore.Mvc;

namespace CooksCornerAPP.Controllers
{
    public class MealsController : Controller
    {
        public IActionResult Lunch()
        {
            return View();
        }
    }
}
