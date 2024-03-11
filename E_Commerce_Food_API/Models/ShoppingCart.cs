using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_Food_API.Models
{
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; } = null!;
        public ICollection<ItemCart> CartItems { get; set; } = new List<ItemCart>();

        [NotMapped]
        public double Total { get; set; } 
        
    }
}
