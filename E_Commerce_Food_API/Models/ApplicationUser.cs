using Microsoft.AspNetCore.Identity;

namespace E_Commerce_Food_API.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string? Name { get; set; } // you can disable the nullable in your project or use ?
    }
}
