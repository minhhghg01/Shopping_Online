using Microsoft.AspNetCore.Mvc;

namespace Shopping_Online.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
    }
}