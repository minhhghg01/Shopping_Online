using System.ComponentModel.DataAnnotations;

namespace Shopping_Online.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }
        [Required, MinLength(4, ErrorMessage = "Nhập tên Sản phẩm")]
        public string Name { get; set; }
        [Required, MinLength(4, ErrorMessage = "Nhập mô tả Sản phẩm")]
        public string Description { get; set; }
        public string Slug { get; set; }
        [Required, MinLength(4, ErrorMessage = "Nhập giá Sản phẩm")]
        public int Price { get; set; }
        public string Image { get; set; }

        public int BrandId { get; set; }
        public int CategoryId { get; set; }

        public BrandModel Brand { get; set; }
        public CategoryModel Category { get; set; }
    }
}