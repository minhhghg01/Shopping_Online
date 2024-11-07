using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Shopping_Online.Models;

namespace Shopping_Online.Data
{
    public class SeedData
    {
        public static void SeedingData(DataContext _context)
        {
            _context.Database.Migrate();
            if (!_context.Products.Any())
            {
                CategoryModel macbook = new CategoryModel
                {
                    Name = "Macbook",
                    Slug = "macbook",
                    Description = "Macbook is luxury",
                    Status = 1
                };
                CategoryModel pc = new CategoryModel
                {
                    Name = "Pc",
                    Slug = "pc",
                    Description = "Pc is ...",
                    Status = 1
                };

                BrandModel apple = new BrandModel
                {
                    Name = "Apple",
                    Slug = "apple",
                    Description = "Apple is .....",
                    Status = 1
                };
                BrandModel samsung = new BrandModel
                {
                    Name = "Samsung",
                    Slug = "samsung",
                    Description = "Samsung isn't Apple",
                    Status = 1
                };
                _context.Products.AddRange(
                    new ProductModel
                    {
                        Name = "Macbook",
                        Slug = "macbook",
                        Description = "Macbook is best",
                        Image = "1.jpg",
                        Price = 1231,
                        Category = macbook,
                        Brand = apple
                    },
                    new ProductModel
                    {
                        Name = "Pc",
                        Slug = "pc",
                        Description = "Pc is ...",
                        Image = "1.jpg",
                        Price = 1199,
                        Category = pc,
                        Brand = samsung
                    }
                );
                _context.SaveChanges();
            }
        }
    }
}