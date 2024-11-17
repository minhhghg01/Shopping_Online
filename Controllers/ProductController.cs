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
        public ProductController(DataContext context)
        {
            _dataContext = context;
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

            var productById = _dataContext.Products
                                .Include(p => p.Ratings)
                                .Where(p => p.Id == Id)
                                .FirstOrDefault();

            var relatedProducts = await _dataContext.Products
                                .Where(p => p.CategoryId == productById.CategoryId && p.Id != productById.Id)
                                .Take(4).ToListAsync();

            ViewBag.RelatedProducts = relatedProducts;
            
            var ViewModel = new ProductDetailsViewModel
            {
                ProductDetails = productById,
            };
            return View(ViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CommentProduct(RatingModel rating)
        {
            if (ModelState.IsValid)
            {
                var ratingEnitity = new RatingModel
                {
                    ProductId = rating.ProductId,
                    Name = rating.Name,
                    Email = rating.Email,
                    Comment = rating.Comment,
                    Star = rating.Star,
                };
                    
                _dataContext.Ratings.Add(ratingEnitity);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cmt thành công";
                return Redirect(Request.Headers["Referer"]);
            }
            else
            {
                return RedirectToAction("Details", new { id = rating.ProductId });
            }
        }
    }
}