using System.ComponentModel.DataAnnotations;

namespace Shopping_Online.Models
{
    public class CategoryModel
    {
        [Key]
        public int Id { get; set; }
        [Required, MinLength(4, ErrorMessage = "Nhập tên Danh mục")]
        public string Name { get; set; }
        [Required, MinLength(4, ErrorMessage = "Nhập mô tả Danh mục")]
        public string Description { get; set; }
        [Required]
        public string Slug { get; set; }
        public int Status { get; set; }
    }
}