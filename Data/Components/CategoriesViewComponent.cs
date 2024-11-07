using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Shopping_Online.Data.Components
{
    public class CategoriesViewComponent : ViewComponent
    {
        public readonly DataContext _dataContext;
        public CategoriesViewComponent(DataContext context)
        {
            _dataContext = context;
        }
        
        public async Task<IViewComponentResult> InvokeAsync()
        => View(await _dataContext.Categories.ToListAsync());
    }
}