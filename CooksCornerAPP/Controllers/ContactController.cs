using Microsoft.AspNetCore.Mvc;

namespace CooksCornerAPP.Controllers
{
    public class ContactController : Controller
    {

        public IActionResult ContactForm()
        {
            return View();
        }
    }
}
