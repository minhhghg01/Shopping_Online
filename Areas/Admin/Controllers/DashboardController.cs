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

        [HttpPost]
        public async Task<IActionResult> GetChartData()
        {
            var data = _dataContext.StatisticalModels
                .Select(s => new
                {
                    date = s.DateCreated.ToString("yyyy-MM-dd"), // Sử dụng định dạng đúng
                    sold = s.Sold,
                    quantity = s.Quantity,
                    revenue = s.Revenue,
                    profit = s.Profit
                })
                .ToList();

            return Json(data);
        }
    }
}