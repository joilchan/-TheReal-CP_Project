using BroShopAPI.Data;
using BroShopAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BroShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CartsController(AppDbContext context) => _context = context;

        // Получить корзину пользователя
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetCart(int userId)
        {
            return await _context.Carts
                .Where(c => c.UserId == userId)
                .Select(c => new
                {
                    c.ProductVariantId,
                    c.Quantity,
                    Size = c.ProductVariant.Size,
                    ProductName = c.ProductVariant.Product.Name,
                    Price = c.ProductVariant.Product.Price,
                    Image = c.ProductVariant.Product.ImageUrl
                }).ToListAsync();
        }

        // Добавить или обновить товар в корзине
        [HttpPost]
        public async Task<IActionResult> AddToCart(Cart cart)
        {
            var existing = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == cart.UserId && c.ProductVariantId == cart.ProductVariantId);

            if (existing != null)
            {
                existing.Quantity += cart.Quantity;
            }
            else
            {
                _context.Carts.Add(cart);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
