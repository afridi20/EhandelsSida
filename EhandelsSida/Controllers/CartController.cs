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

        [HttpGet]
        public async Task<IActionResult> GetCartItems()
        {
            var userId = _userManager.GetUserId(User);
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.UserId == userId && o.Status == "Open");

            if (order == null) return Json(new List<object>());

            var items = order.OrderItems.Select(i => new
            {
                name = i.Product.Name,
                quantity = i.Quantity,
                price = i.UnitPrice
            });

            return Json(items);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.UserId == userId && o.Status == "Open");

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            var item = await _context.OrderItems.FindAsync(itemId);
            if (item == null) return NotFound();

            _context.OrderItems.Remove(item);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> IncreaseQuantity(int itemId)
        {
            var item = await _context.OrderItems.FindAsync(itemId);
            if (item == null) return NotFound();

            item.Quantity += 1;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DecreaseQuantity(int itemId)
        {
            var item = await _context.OrderItems.FindAsync(itemId);
            if (item == null) return NotFound();

            if (item.Quantity > 1)
                item.Quantity -= 1;
            else
                _context.OrderItems.Remove(item);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }



    }
}

