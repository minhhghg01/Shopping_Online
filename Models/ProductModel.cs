using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shopping_Online.Data.Validation;

namespace Shopping_Online.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Nhập tên Sản phẩm")]
        public string Name { get; set; }
        public string Slug { get; set; }
        [Required,MinLength(4, ErrorMessage = "Nhập mô tả Sản phẩm")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Nhập giá Sản phẩm")]
        public int Price { get; set; }
        [Required, Range(1, int.MaxValue, ErrorMessage = "Chọn một Thương hiệu")]
        public int BrandId { get; set; }
        [Required, Range(1, int.MaxValue, ErrorMessage = "Chọn một Danh mục")]
        public int CategoryId { get; set; }

        public BrandModel Brand { get; set; }
        public CategoryModel Category { get; set; }
        public string Image { get; set; }

        [NotMapped]
        [FileExtension]
        public IFormFile ImageUpload { get; set; }
    }
}