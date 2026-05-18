using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantMvcUltimatePro.Data;

namespace RestaurantMvcUltimatePro.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Orders()
        {
            return RedirectToAction(nameof(History));
        }

        public IActionResult History()
        {
            var orders = _context.Orders
                .Include(x => x.OrderItems)
                .OrderByDescending(x => x.OrderDate)
                .ToList();

            return View(orders);
        }

        public IActionResult Details(int id)
        {
            var order = _context.Orders
                .Include(x => x.OrderItems)
                .FirstOrDefault(x => x.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        public IActionResult Success(int id)
        {
            var order = _context.Orders
                .Include(x => x.OrderItems)
                .FirstOrDefault(x => x.Id == id);

            if (order == null)
                return RedirectToAction(nameof(History));

            return View(order);
        }
    }
}