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
            List<OrderModel> order = await _dataContext.Orders
                                    // .OrderByDescending(o => o.CreateDate)
                                    .ToListAsync();
            // const int pageSize = 10;
            // if (pg < 1)
            // {
            //     pg = 1;
            // }
            // int recsCount = order.Count();
            // var pager = new Paginate(recsCount, pg, pageSize);
            // int recSkip = (pg - 1) * pageSize;
            // var data = order.Skip(recSkip).Take(pager.PageSize).ToList();
            // ViewBag.Pager = pager;
            // return View(data);
            return View(order);
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
            _dataContext.Update(order);

            if (status == 0)
            {
                var DetailsOrder = await _dataContext.OrderDetails
                                    .Include(od => od.Product)
                                    .Where(od => od.OrderCode == ordercode)
                                    .Select(od => new 
                                    {
                                        od.Quantity,
                                        od.Product.Price,
                                        od.Product.CapitalPrice
                                    }).ToListAsync();
                
                var statisticalModel = await _dataContext.StatisticalModels
                                        .FirstOrDefaultAsync(s => s.DateCreated.Date == order.CreateDate.Date);
                
                if (statisticalModel != null)
                {
                    foreach (var orderDetail in DetailsOrder)
                    {
                        statisticalModel.Quantity += 1;
                        statisticalModel.Sold += orderDetail.Quantity;
                        statisticalModel.Revenue += orderDetail.Price * orderDetail.Quantity;
                        statisticalModel.Profit += orderDetail.Price - orderDetail.CapitalPrice;
                    }
                    _dataContext.Update(statisticalModel);
                }
                else {
                    int new_quantity = 0;
                    int new_sold = 0;
                    int new_profit = 0;
                    foreach (var orderDetail in DetailsOrder)
                    {
                        new_quantity += 1;
                        new_sold += orderDetail.Quantity;
                        new_profit += orderDetail.Price - orderDetail.CapitalPrice;

                        statisticalModel = new StatisticalModel
                        {
                            DateCreated = order.CreateDate,
                            Quantity = new_quantity,
                            Sold = new_sold,
                            Revenue = orderDetail.Price * orderDetail.Quantity,
                            Profit = new_profit
                        };
                    }
                    _dataContext.Add(statisticalModel);
                }
            }

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