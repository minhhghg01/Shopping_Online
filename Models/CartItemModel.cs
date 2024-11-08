using System.ComponentModel.DataAnnotations;

namespace Shopping_Online.Models
{
    public class CartItemModel
    {
        [Key]
        public int ProductId { get; set; }
        [Required, MinLength(4, ErrorMessage = "Nhập tên Thương hiệu")]
        public string ProductName { get; set; }
        [Required, MinLength(4, ErrorMessage = "Nhập mô tả Thương hiệu")]
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { 
            get {return Quantity * Price;}
        }
        public string Image { get; set; }
        public CartItemModel() { }

        public CartItemModel(ProductModel product) {
            ProductId = product.Id;
            ProductName = product.Name;
            Price = product.Price;
            Quantity = 1;
            Image = product.Image;
         }
    }
}