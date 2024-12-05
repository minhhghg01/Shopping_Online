using System.ComponentModel.DataAnnotations;

namespace Shopping_Online.Models.ViewModels
{
    public class ProductDetailsViewModel
    {
        public ProductModel ProductDetails { get; set; }
        [Required(ErrorMessage ="Nhập cmt")]
        public string Comment { get; set; }
        [Required(ErrorMessage ="Nhập tên")]
        public string Name { get; set; }
        [Required(ErrorMessage ="Nhập email")]
        public string Email { get; set; }
        public List<RatingModel> Ratings { get; set; }
    }
}