using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Food_API.Models.DTO
{
    public class LoginRequestDto
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        //[Required]
        public string Password { get; set; }
    }
}
