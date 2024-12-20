using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Online.Areas.Admin.Repository;
using Shopping_Online.Data;
using Shopping_Online.Models;

namespace Shopping_Online.Controllers;

public class HomeController : Controller
{
    private readonly DataContext _dataContext;
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<AppUserModel> _userManager;
    private readonly IEmailSender _emailSender;
    public HomeController(ILogger<HomeController> logger, DataContext context, UserManager<AppUserModel> userManager, IEmailSender emailSender)
    {
        _logger = logger;
        _dataContext = context;
        _userManager = userManager;
        _emailSender = emailSender;
    }

    public IActionResult Index(string sort_by = "", string startprice = "", string endprice = "")
    {
        IQueryable<ProductModel> productsQuery = _dataContext.Products.Include("Category").Include("Brand");

        // Đếm số lượng sản phẩm
        var count = productsQuery.Count();
        if (count > 0)
        {
            switch (sort_by)
            {
                case "price_increase":
                    productsQuery = productsQuery.OrderBy(p => p.Price);
                    break;
                case "price_decrease":
                    productsQuery = productsQuery.OrderByDescending(p => p.Price);
                    break;
                case "price_newest":
                    productsQuery = productsQuery.OrderByDescending(p => p.Id);
                    break;
                case "price_oldest":
                    productsQuery = productsQuery.OrderBy(p => p.Id);
                    break;
                default:
                    if (decimal.TryParse(startprice, out decimal startPriceValue) &&
                        decimal.TryParse(endprice, out decimal endPriceValue))
                    {
                        productsQuery = productsQuery.Where(p => p.Price >= startPriceValue && p.Price <= endPriceValue);
                    }
                    productsQuery = productsQuery.OrderByDescending(p => p.Id);
                    break;
            }
        }
        ViewBag.SortBy = sort_by;
        var products = productsQuery.ToList();
        return View(products);
    }

    [HttpGet]
    public async Task<IActionResult> Wishlist()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var currentUserId = currentUser?.Id;

        var wishlist = await (from w in _dataContext.Wishlists
                              join p in _dataContext.Products on w.ProductId equals p.Id
                              join u in _dataContext.Users on w.UserId equals u.Id
                              where w.UserId == currentUserId
                              select new
                              {
                                  User = u,
                                  Product = p,
                                  Wishlist = w
                              }).ToListAsync();

        return View(wishlist);
    }

    [HttpPost]
    public async Task<IActionResult> AddWishlist(int productId)
    {
        var user = await _userManager.GetUserAsync(User);

        // Kiểm tra xem sản phẩm đã có trong wishlist chưa
        var existingWishlistItem = await _dataContext.Wishlists
            .FirstOrDefaultAsync(w => w.ProductId == productId && w.UserId == user.Id);

        // Nếu sản phẩm đã có trong wishlist, trả về thông báo
        if (existingWishlistItem != null)
        {
            return Ok(new { success = false, message = "Sản phẩm đã có trong danh sách yêu thích của bạn." });
        }

        var wishlist = new WishlistModel
        {
            ProductId = productId,
            UserId = user.Id
        };

        Console.WriteLine($"Heloo ProductId: {wishlist.ProductId}, UserId: {wishlist.UserId}");

        _dataContext.Wishlists.Add(wishlist);

        try
        {
            await _dataContext.SaveChangesAsync();
            return Ok(new { success = true, message = "Thêm yêu thích thành công" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            return StatusCode(500, "Thêm yêu thích thất bại");
        }
    }

    public async Task<IActionResult> DeleteWishlist(int Id)
    {
        WishlistModel wishlist = await _dataContext.Wishlists.FindAsync(Id);

        if (wishlist != null)
        {
            _dataContext.Wishlists.Remove(wishlist);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Xóa yêu thích thành công";
        }

        return RedirectToAction("Wishlist");
    }
    public IActionResult Privacy()
    {
        return View();
    }
    [HttpGet]
    public async Task<IActionResult> Contact()
    {
        ViewData["HideSearchBox"] = true;

        var user = await _userManager.GetUserAsync(User); 
        var model = new AppUserModel
        {
            Email = user?.Email,
            PhoneNumber = user?.PhoneNumber
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> SendContact(string phoneNumber, string subject, string message)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var email = currentUser?.Email;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(message))
        {
            TempData["error"] = "Vui lòng điền đầy đủ thông tin.";
            return RedirectToAction("Contact");
        }

        try
        {
            // Địa chỉ email của shop
            var shopEmail = "nguyenminhhghg01@gmail.com";

            // Nội dung email
            var emailSubject = $"Shop-Onl | Liên hệ từ khách hàng: {subject}";
            var emailMessage = $"Email người gửi: {email}\n" +
                               $"Số điện thoại: {phoneNumber}\n" +
                               $"Nội dung:\n{message}";

            // Gửi email
            await _emailSender.SendEmailAsync(shopEmail, emailSubject, emailMessage);

            TempData["success"] = "Gửi liên hệ thành công. Chúng tôi sẽ phản hồi sớm nhất!";
            return RedirectToAction("Contact");
        }
        catch
        {
            TempData["error"] = "Có lỗi xảy ra trong quá trình gửi liên hệ. Vui lòng thử lại.";
            return RedirectToAction("Contact");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int statuscode)
    {
        if (statuscode == 404)
        {
            return View("NotFound");
        }
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
