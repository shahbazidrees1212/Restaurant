using RestaurantMvcUltimatePro.Models;

namespace RestaurantMvcUltimatePro.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (!db.MenuItems.Any())
        {
            db.MenuItems.AddRange(
                new MenuItem { Name="Royal Beef Steak", Category="Main Course", Description="Grilled steak with pepper sauce and herbs.", Price=2850, ImageUrl="https://images.unsplash.com/photo-1544025162-d76694265947?auto=format&fit=crop&w=900&q=80", IsPopular=true, IsChefSpecial=true },
                new MenuItem { Name="Creamy Alfredo Pasta", Category="Pasta", Description="Fettuccine pasta in rich parmesan cream sauce.", Price=1450, ImageUrl="https://images.unsplash.com/photo-1645112411341-6c4fd023714a?auto=format&fit=crop&w=900&q=80", IsPopular=true },
                new MenuItem { Name="Signature Zinger Burger", Category="Burgers", Description="Crispy chicken fillet, house sauce and fresh salad.", Price=950, ImageUrl="https://images.unsplash.com/photo-1568901346375-23c9450c58cd?auto=format&fit=crop&w=900&q=80", IsPopular=true },
                new MenuItem { Name="Mediterranean Salad", Category="Healthy", Description="Fresh greens, feta, olives and lemon dressing.", Price=780, ImageUrl="https://images.unsplash.com/photo-1512621776951-a57141f2eefd?auto=format&fit=crop&w=900&q=80", IsVegetarian=true },
                new MenuItem { Name="Chocolate Lava Cake", Category="Desserts", Description="Warm chocolate cake with molten center.", Price=650, ImageUrl="https://images.unsplash.com/photo-1606313564200-e75d5e30476c?auto=format&fit=crop&w=900&q=80", IsChefSpecial=true },
                new MenuItem { Name="Mint Margarita", Category="Drinks", Description="Refreshing mint, lemon and crushed ice.", Price=380, ImageUrl="https://images.unsplash.com/photo-1513558161293-cdaf765ed2fd?auto=format&fit=crop&w=900&q=80" }
            );
        }
        if (!db.Chefs.Any()) db.Chefs.AddRange(
            new Chef{Name="Ayaan Malik", Role="Executive Chef", Bio="Expert in premium continental and fusion cuisine.", ImageUrl="https://images.unsplash.com/photo-1577219491135-ce391730fb2c?auto=format&fit=crop&w=700&q=80"},
            new Chef{Name="Sara Khan", Role="Pastry Chef", Bio="Creates elegant desserts and signature cakes.", ImageUrl="https://images.unsplash.com/photo-1583394293214-28ded15ee548?auto=format&fit=crop&w=700&q=80"},
            new Chef{Name="Hamza Ali", Role="Grill Master", Bio="Known for steaks, BBQ platters and smoky flavors.", ImageUrl="https://images.unsplash.com/photo-1600565193348-f74bd3c7ccdf?auto=format&fit=crop&w=700&q=80"}
        );
        if (!db.Testimonials.Any()) db.Testimonials.AddRange(
            new Testimonial{CustomerName="Maham R.", Review="Beautiful place, quick service and amazing taste.", Rating=5, ImageUrl="https://images.unsplash.com/photo-1494790108377-be9c29b29330?auto=format&fit=crop&w=300&q=80"},
            new Testimonial{CustomerName="Usman K.", Review="Best family dinner experience. The steak was perfect.", Rating=5, ImageUrl="https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=crop&w=300&q=80"},
            new Testimonial{CustomerName="Hira S.", Review="Modern interior, fresh food and professional staff.", Rating=5, ImageUrl="https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=300&q=80"}
        );
        if (!db.Offers.Any()) db.Offers.AddRange(
            new Offer{Title="Family Feast", Description="Perfect dinner deal for 4 people.", DiscountPercent=20, ValidUntil=DateTime.Today.AddMonths(1)},
            new Offer{Title="Lunch Saver", Description="Flat discount on weekdays 12 PM to 4 PM.", DiscountPercent=15, ValidUntil=DateTime.Today.AddMonths(2)}
        );
        if (!db.RestaurantEvents.Any()) db.RestaurantEvents.AddRange(
            new RestaurantEvent{Title="Birthday Celebration", Description="Decor, custom menu and private table setup.", EventDate=DateTime.Today.AddDays(10), StartingPrice=12000, ImageUrl="https://images.unsplash.com/photo-1530103862676-de8c9debad1d?auto=format&fit=crop&w=900&q=80"},
            new RestaurantEvent{Title="Corporate Dinner", Description="Professional setup for company dinners and meetings.", EventDate=DateTime.Today.AddDays(18), StartingPrice=25000, ImageUrl="https://images.unsplash.com/photo-1519167758481-83f550bb49b3?auto=format&fit=crop&w=900&q=80"}
        );
       
        db.SaveChanges();
    }
}
