using System.ComponentModel.DataAnnotations;

namespace Shopping_Online.Models
{
    public class ContactModel {
        [Key]
        [Required(ErrorMessage = " Nhập tên")]
        public string Name { get; set; }
        [Required(ErrorMessage = " Nhập Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = " Nhập sđt")]
        public string Phone { get; set; }
        [Required(ErrorMessage = " Nhập tiêu đề")]
        public string Subject { get; set; }
        [Required(ErrorMessage = " Nhập tin nhắn")]
        public string Message { get; set; }
    }
}