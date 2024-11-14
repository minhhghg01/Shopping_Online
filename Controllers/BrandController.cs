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

        public async Task<IActionResult> Index(String Slug = "", string sort_by = "", string startprice = "", string endprice = "")
        {
            BrandModel brand = _dataContext.Brands.Where(c => c.Slug == Slug).FirstOrDefault();
            if (brand == null)
            {
                return RedirectToAction("Index");
            }
            IQueryable<ProductModel> productByBrand = _dataContext.Products.Where(p => p.BrandId == brand.Id);
            var count = await productByBrand.CountAsync();
            if (count > 0)
            {
                switch (sort_by)
                {
                    case "price_increase":
                        productByBrand = productByBrand.OrderBy(p => p.Price);
                        break;
                    case "price_decrease":
                        productByBrand = productByBrand.OrderByDescending(p => p.Price);
                        break;
                    case "price_newest":
                        productByBrand = productByBrand.OrderByDescending(p => p.Id);
                        break;
                    case "price_oldest":
                        productByBrand = productByBrand.OrderBy(p => p.Id);
                        break;
                    default:
                        if (decimal.TryParse(startprice, out decimal startPriceValue) && 
                            decimal.TryParse(endprice, out decimal endPriceValue))
                        {
                            productByBrand = productByBrand.Where(p => p.Price >= startPriceValue && p.Price <= endPriceValue);
                        }
                        productByBrand = productByBrand.OrderByDescending(p => p.Id);
                        break;
                }
            }
            return View(await productByBrand.ToListAsync() );
        }
    }
}