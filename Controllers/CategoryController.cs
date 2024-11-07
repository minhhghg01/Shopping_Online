using Microsoft.AspNetCore.Mvc;

namespace Shopping_Online.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}