using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Online.Data;
using Shopping_Online.Data.Components;
using Shopping_Online.Models;

namespace Shopping_Online.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;
        public CategoryController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index(String Slug = "")
        {
            CategoryModel category = _dataContext.Categories.Where(c => c.Slug == Slug).FirstOrDefault();
            if (category == null)
            {
                return RedirectToAction("Index");
            }
            var productByCategory = _dataContext.Products.Where(p => p.CategoryId == category.Id);
            return View(await productByCategory.OrderBy(p => p.Id).ToListAsync() );
        }
        
    }
}