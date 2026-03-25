using Microsoft.AspNetCore.Mvc;
using EhandelsSida.Data;
using EhandelsSida.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EhandelsSida.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId)
        {
            var userID = _userManager.GetUserId(User);

            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();


            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.UserId == userID && o.Status == "Open");
            if (order == null)
            {
                order = new Order
                {
                    UserId = userID,
                    OrderDate = DateTime.Now,
                    Status = "Open",
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

            }

            var existingItem = order.OrderItems.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += 1;
            }
            else
            {
                order.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = productId,
                    Quantity = 1,
                    UnitPrice = product.Price
                });
            }

            await _context.SaveChangesAsync();
            return Redirect(Request.Headers["Referer"].ToString());


        }

    }
}

