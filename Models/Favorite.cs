namespace RestaurantMvcUltimatePro.Models
{
    public class Favorite
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public int MenuItemId { get; set; }

        public string FoodName { get; set; }

        public string ImageUrl { get; set; }

        public decimal Price { get; set; }
    }
}