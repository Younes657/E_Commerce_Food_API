using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_Food_API.Models
{
    public class ItemCart
    {
        public int Id { get; set; }
        public int MenuItemId { get; set; }
        [ForeignKey("MenuItemId")]
        public MenuItem MenuItem { get; set; } = null!;
        public int Quantity { get; set; }
        public int ShoppingCartId { get; set; }
        [ForeignKey("ShoppingCartId")]
        public ShoppingCart ShoppingCart { get; set; } = null!;
    }
}
