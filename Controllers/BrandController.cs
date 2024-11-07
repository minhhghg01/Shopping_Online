using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Online.Data;
using Shopping_Online.Models;

namespace Shopping_Online.Controllers
{
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;
        public BrandController(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<IActionResult> Index(String Slug = "")
        {
            BrandModel brand = _dataContext.Brands.Where(c => c.Slug == Slug).FirstOrDefault();
            if (brand == null)
            {
                return RedirectToAction("Index");
            }
            var productByBrand = _dataContext.Products.Where(p => p.BrandId == brand.Id);
            return View(await productByBrand.OrderBy(p => p.Id).ToListAsync() );
        }
    }
}