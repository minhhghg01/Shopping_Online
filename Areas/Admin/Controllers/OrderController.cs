using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Online.Data;
using Shopping_Online.Models;

namespace Shopping_Online.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly DataContext _dataContext;
        public OrderController(DataContext context)
        {
            _dataContext = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int pg = 1)
        {
            List<OrderModel> order = await _dataContext.Orders.ToListAsync();
            const int pageSize = 10;
            if (pg < 1)
            {
                pg = 1;
            }
            int recsCount = order.Count();
            var pager = new Paginate(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = order.Skip(recSkip).Take(pager.PageSize).ToList();
            ViewBag.Pager = pager;
            return View(data);
        }
        [HttpGet]
        public async Task<IActionResult> ViewOrder(string orderCode)
        {
            var DetailsOrder = await _dataContext.OrderDetails.Include(od => od.Product)
            .Where(od => od.OrderCode == orderCode).ToListAsync();
            var Order = _dataContext.Orders.Where(o => o.OrderCode == orderCode).First();
            ViewBag.Status = Order.Status;
            return View(DetailsOrder);
        }
        [HttpGet]
        public async Task<IActionResult> EditOrder(string ordercode)
        {
            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == ordercode);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateOrder(string ordercode, int status)
        {
            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == ordercode);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = status;
            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Cập nhật thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi gì đó");
            }
        }
    }
}