using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Shopping_Online.Data.Components
{
    public class BrandsViewComponent : ViewComponent
    {
        public readonly DataContext _dataContext;
        public BrandsViewComponent(DataContext context)
        {
            _dataContext = context;
        }
        
        public async Task<IViewComponentResult> InvokeAsync()
        => View(await _dataContext.Brands.ToListAsync());
    }
}