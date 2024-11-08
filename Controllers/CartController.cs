using Microsoft.AspNetCore.Mvc;
using Shopping_Online.Data;
using Shopping_Online.Models;
using Shopping_Online.Models.ViewModels;

namespace Shopping_Online.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _dataContext;
        public CartController(DataContext _context)
        {
            _dataContext = _context;
        }
        public IActionResult Index()
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemViewModel cartVM = new()
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(x => x.Price * x.Quantity)
            };
            return View(cartVM);
        }

        public IActionResult Checkout()
        {
            return View("~/Views/Checkout/Index.cshtml");
        }

        public async Task<IActionResult> Add(int Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();

            if(cartItem == null) {
                cart.Add(new CartItemModel(product));
            }
            else {
                cartItem.Quantity++;
            }
            HttpContext.Session.SetJson("Cart", cart);
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}