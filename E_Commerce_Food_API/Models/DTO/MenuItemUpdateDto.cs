using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Food_API.Models.DTO
{
    public class MenuItemUpdateDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string SpecialTag { get; set; }
        public string Category { get; set; }
        [Range(1, int.MaxValue)]
        public double Price { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}
