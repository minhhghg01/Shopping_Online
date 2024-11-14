using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Online.Data;
using Shopping_Online.Models;

namespace Shopping_Online.Controllers;

public class HomeController : Controller
{
    private readonly DataContext _dataContext;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, DataContext context)
    {
        _logger = logger;
        _dataContext = context;
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

        var products = productsQuery.ToList();
        return View(products);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
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
