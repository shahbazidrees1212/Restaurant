using Microsoft.AspNetCore.Mvc;
using RestaurantMvcUltimatePro.Data;
using RestaurantMvcUltimatePro.Models;

namespace RestaurantMvcUltimatePro.Controllers
{
    public class FavoritesController : Controller
    {
        private readonly AppDbContext _context;

        public FavoritesController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var favorites = _context.Favorites.ToList();
            return View(favorites);
        }

        [HttpPost]
        public IActionResult Add(int menuItemId, string foodName, string imageUrl, decimal price)
        {
            var favorite = new Favorite
            {
                UserName = User.Identity?.Name ?? "Guest",
                MenuItemId = menuItemId,
                FoodName = foodName,
                ImageUrl = imageUrl,
                Price = price
            };

            _context.Favorites.Add(favorite);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Remove(int id)
        {
            var favorite = _context.Favorites.FirstOrDefault(x => x.Id == id);

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}