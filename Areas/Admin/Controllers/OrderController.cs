using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Online.Data;

namespace Shopping_Online.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly DataContext _dataContext;
        public OrderController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Orders.OrderByDescending(p => p.Id).ToListAsync());
        }
        public async Task<IActionResult> ViewOrder(string orderCode)
        {
            var DetailsOrder = await _dataContext.OrderDetails.Include(od => od.Product).Where(od => od.OrderCode == orderCode).ToListAsync();
            return View(DetailsOrder);
        }

    }
}