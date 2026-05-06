using EhandelsSida.Data;
using EhandelsSida.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EhandelsSida.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Checkout()
        {
            var userId = _userManager.GetUserId(User);

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.UserId == userId && o.Status == "Open");

            if (order == null)
                return RedirectToAction("Index", "Cart");

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return NotFound();

         
            order.Status = "Paid";
            order.PaymentDate = DateTime.Now;

            await _context.SaveChangesAsync();

            // Skicka användaren till kvittosidan
            return RedirectToAction("Receipt", new { id = order.Id });
        }

        public async Task<IActionResult> Receipt(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

    }
}
    