using Microsoft.AspNetCore.Identity;

namespace Shopping_Online.Models
{
    public class AppUserModel : IdentityUser
    {
        public string Occupation { get; set; }
    }
}