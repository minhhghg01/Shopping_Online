using System.ComponentModel.DataAnnotations;

namespace Shopping_Online.Models
{
    public class CategoryModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Nhập tên Danh mục")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Nhập mô tả Danh mục")]
        public string Description { get; set; }
        public string Slug { get; set; }
        public int Status { get; set; }
    }
}