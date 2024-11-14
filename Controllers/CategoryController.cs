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
        public async Task<IActionResult> Index(String Slug = "", string sort_by = "", string startprice = "", string endprice = "")
        {
            CategoryModel category = _dataContext.Categories.Where(c => c.Slug == Slug).FirstOrDefault();

            if (category == null)
            {
                return RedirectToAction("Index");
            }

            IQueryable<ProductModel> productByCategory = _dataContext.Products.Where(p => p.CategoryId == category.Id);
            var count = await productByCategory.CountAsync();
            if (count > 0)
            {
                switch (sort_by)
                {
                    case "price_increase":
                        productByCategory = productByCategory.OrderBy(p => p.Price);
                        break;
                    case "price_decrease":
                        productByCategory = productByCategory.OrderByDescending(p => p.Price);
                        break;
                    case "price_newest":
                        productByCategory = productByCategory.OrderByDescending(p => p.Id);
                        break;
                    case "price_oldest":
                        productByCategory = productByCategory.OrderBy(p => p.Id);
                        break;
                    default:
                        if (decimal.TryParse(startprice, out decimal startPriceValue) && 
                            decimal.TryParse(endprice, out decimal endPriceValue))
                        {
                            productByCategory = productByCategory.Where(p => p.Price >= startPriceValue && p.Price <= endPriceValue);
                        }
                        productByCategory = productByCategory.OrderByDescending(p => p.Id);
                        break;
                }
            }

            return View(await productByCategory.ToListAsync());
        }

    }
}