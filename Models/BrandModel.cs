using System.ComponentModel.DataAnnotations;

namespace Shopping_Online.Models
{
    public class BrandModel
    {
        [Key]
        public int Id { get; set; }
        [Required, MinLength(4, ErrorMessage = "Nhập tên Thương hiệu")]
        public string Name { get; set; }
        [Required, MinLength(4, ErrorMessage = "Nhập mô tả Thương hiệu")]
        public string Description { get; set; }
        [Required]
        public string Slug { get; set; }
        public int Status { get; set; }
    }
}