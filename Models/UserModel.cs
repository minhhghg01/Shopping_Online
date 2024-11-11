using System.ComponentModel.DataAnnotations;

namespace Shopping_Online.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Nhập UserName")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Nhập Email"), EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password), Required(ErrorMessage = "Nhập Password")]
        public string Password { get; set; }
    }
}