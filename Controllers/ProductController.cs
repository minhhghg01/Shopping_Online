using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Online.Data;

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
            var productById = _dataContext.Products.Where(p => p.Id == Id).FirstOrDefault();
            var relatedProducts = await _dataContext.Products
            .Where(p => p.CategoryId == productById.CategoryId && p.Id != productById.Id)
            .Take(4)
            .ToListAsync();
            ViewBag.RelatedProducts = relatedProducts;
            return View(productById); // Trả về view với sản phẩm
        }
    }
}