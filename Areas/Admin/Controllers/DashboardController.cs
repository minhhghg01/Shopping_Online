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
        [Route("GetChartData")]
        public async Task<IActionResult> GetChartData()
        {
            var data = _dataContext.StatisticalModels
                .Select(s => new
                {
                    date = s.DateCreated.ToString("yyyy-MM-dd"),
                    sold = s.Sold,
                    quantity = s.Quantity,
                    revenue = s.Revenue,
                    profit = s.Profit
                })
                .ToList();

            return Json(data);
        }

        [HttpPost]
        [Route("GetChartDataBySelect")]
        public IActionResult GetChartDataBySelect(DateTime startDate, DateTime endDate)
        {
            var data = _dataContext.StatisticalModels
                .Where(s => s.DateCreated >= startDate && s.DateCreated <= endDate)
                .Select(s => new
                {
                    date = s.DateCreated.ToString("yyyy-MM-dd"),
                    sold = s.Sold,
                    quantity = s.Quantity,
                    revenue = s.Revenue,
                    profit = s.Profit
                })
                .ToList();

            return Json(data);
        }

        [HttpPost]
        [Route("FilterData")]
        public IActionResult FilterData(DateTime? fromDate, DateTime? toDate)
        {
            var query = _dataContext.StatisticalModels.AsQueryable();
            if (fromDate.HasValue)
            {
                query = query.Where(s => s.DateCreated >= fromDate);
            }

            if (toDate.HasValue)
            {
                toDate = toDate.Value.AddDays(1);
                query = query.Where(s => s.DateCreated < toDate);
            }

            var data = query.Select(s => new
            {
                date = s.DateCreated.ToString("yyyy-MM-dd"),
                sold = s.Sold,
                quantity = s.Quantity,
                revenue = s.Revenue,
                profit = s.Profit
            }).ToList();

            return Json(data);
        }
    }
}