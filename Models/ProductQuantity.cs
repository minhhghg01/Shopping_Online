using System.ComponentModel.DataAnnotations;

namespace Shopping_Online.Models
{
    public class ProductQuantity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Nhập số lượng")]
        public int Quantity { get; set; }
        public DateTime DateCreated { get; set; }
    }
}