using System.ComponentModel.DataAnnotations;

namespace RestaurantMvcUltimatePro.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        public int MenuItemId { get; set; }

        public string Name { get; set; } = "";

        public string ImageUrl { get; set; } = "";

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public decimal Total => Price * Quantity;
    }
}