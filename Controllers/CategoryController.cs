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
                if (sort_by == "price_increase")
                {
                    productByCategory = productByCategory.OrderBy(p => p.Price);
                }
                else if (sort_by == "price_decrease")
                {
                    productByCategory = productByCategory.OrderByDescending(p => p.Price);
                }
                else if (sort_by == "price_newest")
                {
                    productByCategory = productByCategory.OrderByDescending(p => p.Id);
                }
                else if (sort_by == "price_oldest")
                {
                    productByCategory = productByCategory.OrderBy(p => p.Id);
                }
                else if (startprice != "" && endprice != "")
                {
                    decimal startPriceValue;
                    decimal endPriceValue;

                    if (decimal.TryParse(startprice, out startPriceValue)
                        && decimal.TryParse(endprice, out endPriceValue))
                    {
                        productByCategory = productByCategory.Where(p => p.Price >= startPriceValue && p.Price <= endPriceValue);
                    }
                    else
                    {
                        productByCategory = productByCategory.OrderByDescending(p => p.Id);
                    }
                }
                else
                {
                    productByCategory = productByCategory.OrderByDescending(p => p.Id);
                }
            }

            return View(await productByCategory.ToListAsync());
        }

    }
}