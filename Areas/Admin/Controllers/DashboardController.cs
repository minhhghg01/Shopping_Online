using Microsoft.AspNetCore.Mvc;
using Shopping_Online.Data;

namespace Shopping_Online.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public DashboardController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var count_product = _dataContext.Products.Count();
            var count_category = _dataContext.Categories.Count();
            var count_order = _dataContext.Orders.Count();
            var count_user = _dataContext.Users.Count();
            ViewBag.countProduct = count_product;
            ViewBag.countCategory = count_category;
            ViewBag.countOrder = count_order;
            ViewBag.countUser = count_user;
            return View();
        }
    }
}