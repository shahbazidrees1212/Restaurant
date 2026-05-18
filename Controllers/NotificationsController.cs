using Microsoft.AspNetCore.Mvc;
using RestaurantMvcUltimatePro.Data;

namespace RestaurantMvcUltimatePro.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly AppDbContext _context;

        public NotificationsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var notifications = _context.Notifications
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return View(notifications);
        }
    }
}