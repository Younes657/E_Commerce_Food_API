using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Food_API.Models.DTO
{
    public class RegisterRequestDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        public string Name { get; set; }
        public string Role { get; set; }
    }
}
