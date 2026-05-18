namespace RestaurantMvcUltimatePro.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string CustomerName { get; set; } = "";
        public string CustomerEmail { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";

        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Placed";
        public decimal TotalAmount { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new();
    }
}