using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Online.Areas.Admin.Repository;
using Shopping_Online.Data;
using Shopping_Online.Models;

namespace Shopping_Online.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IEmailSender _emailSender;
        public CheckoutController(DataContext context, IEmailSender emailSender)
        {
            _dataContext = context;
            _emailSender = emailSender;
        }
        // [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var orderCode = Guid.NewGuid().ToString();
                var orderItem = new OrderModel();
                orderItem.OrderCode = orderCode;
                orderItem.UserName = userEmail;
                orderItem.Status = 1;
                orderItem.CreateDate = DateTime.Now;
                _dataContext.Add(orderItem);
                await _dataContext.SaveChangesAsync();
                List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
                foreach (var item in cartItems)
                {
                    var orderDetail = new OrderDetails();
                    orderDetail.UserName = userEmail;
                    orderDetail.OrderCode = orderCode;
                    orderDetail.ProductId = item.ProductId;
                    orderDetail.Price = item.Price;
                    orderDetail.Quantity = item.Quantity;

                    var product = await _dataContext.Products.Where(p => p.Id == item.ProductId).FirstAsync();
                    product.Quantity -= item.Quantity;
                    product.Sold += item.Quantity;

                    _dataContext.Add(orderDetail);
                    await _dataContext.SaveChangesAsync();
                }
                HttpContext.Session.Remove("Cart");
                // Send Email
                var receiver = userEmail;
                var subject = "Test | Shop_Onl | Đặt hàng thành công";
                var message = "Đơn hàng của bạn đã được đặt thành công. \nCảm ơn quý khách. \nMã đơn hàng:" + orderCode + "\nTổng thanh toán: $" + cartItems.Sum(x => x.Price * x.Quantity) ;
                await _emailSender.SendEmailAsync(receiver, subject, message);
                TempData["success"] = "Đặt hàng thành công";
                return RedirectToAction("History", "Account");
            }
        }

    }
}