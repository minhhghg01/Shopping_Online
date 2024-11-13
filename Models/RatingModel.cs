using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping_Online.Models
{
    public class RatingModel
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        [Required(ErrorMessage ="Nhập bình luận")]
        public string Comment { get; set; }
        [Required(ErrorMessage ="Nhập tên")]
        public string Name { get; set; }
        [Required(ErrorMessage ="Nhập Email")]
        public string Email { get; set; }
        public int Star { get; set; }

        [ForeignKey("ProductId")]
        public ProductModel Product { get; set; }

    }
}