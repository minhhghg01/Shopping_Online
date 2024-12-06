using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Online.Data;
using Shopping_Online.Models;
using Shopping_Online.Models.ViewModels;

namespace Shopping_Online.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<AppUserModel> _userManager;

        public ProductController(DataContext context, UserManager<AppUserModel> userManager)
        {
            _dataContext = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Search(string searchTerm)
        {
            var products = await _dataContext.Products
                .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
                .ToListAsync();

            ViewBag.Keyword = searchTerm;
            return View(products);
        }
        public async Task<IActionResult> Details(int Id)
        {
            if (Id == null) return RedirectToAction("Index");

            var productById = await _dataContext.Products
                                .Include(p => p.Ratings)
                                .Where(p => p.Id == Id)
                                .FirstOrDefaultAsync();

            var relatedProducts = await _dataContext.Products
                                .Where(p => p.CategoryId == productById.CategoryId && p.Id != productById.Id)
                                .Take(4).ToListAsync();

            var ratings = await _dataContext.Ratings
                 .Where(r => r.ProductId == Id)
                 .ToListAsync();

            ViewBag.RelatedProducts = relatedProducts;

            var currentUser = await _userManager.GetUserAsync(User);
            var userEmail = currentUser?.Email;

            ViewBag.UserEmail = userEmail;

            var ViewModel = new ProductDetailsViewModel
            {
                ProductDetails = productById,
                Ratings = ratings
            };
            return View(ViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CommentProduct(RatingModel rating)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User); // Lấy người dùng từ UserManager

                if (user == null)
                {
                    TempData["error"] = "Người dùng không tồn tại.";
                    return Redirect(Request.Headers["Referer"]);
                }

                var userName = user.UserName;
                var userEmail = user.Email;

                var ratingEnitity = new RatingModel
                {
                    ProductId = rating.ProductId,
                    Name = userName,
                    Email = userEmail,
                    Comment = rating.Comment,
                    Star = rating.Star,
                };

                _dataContext.Ratings.Add(ratingEnitity);
                await _dataContext.SaveChangesAsync();

                TempData["success"] = "Đánh giá thành công";
                return Redirect(Request.Headers["Referer"]);
            }
            else
            {
                TempData["error"] = "Có lỗi xảy ra. Vui lòng thử lại.";
                return RedirectToAction("Details", new { id = rating.ProductId });
            }
        }
    }
}