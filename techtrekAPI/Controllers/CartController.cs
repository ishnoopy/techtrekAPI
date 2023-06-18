using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using techtrekAPI.DTO.Cart;
using techtrekAPI.Entities;

namespace techtrekAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : Controller
    {
        private readonly TechtrekContext _context;

        public CartController(TechtrekContext context)
        {
            _context = context;
        }


        [HttpGet("showCart/{id}")]
        [Authorize]
        public async Task<ActionResult<List<CartDTO>>> GetProductsInCart(int id)
        {
            var productsInCart = await _context.cart_items
            .Where(cartItem => cartItem.user_id == id)
            .Select(cartItem => cartItem.product_id)
            .ToListAsync();


            var products = await _context.products
                .Where(product => productsInCart.Contains(product.id))
                .Join(_context.cart_items, product => product.id, cart_item => cart_item.product_id,
                (product, cart_item) => new CartDTO
                {
                    id = cart_item.id,                    
                    product_id = product.id,
                    name = product.name,
                    price = product.price,
                    qty = cart_item.qty,
                    img_url = product.img_url
                }
                ).ToListAsync();

            return products;
        }

        [HttpPost("addCartItem")]
        [Authorize]
        public async Task<HttpStatusCode> AddToCart([FromBody] CartDTO cartitem)
        {
            var existingItem = await _context.cart_items
                .Where(existingItem => existingItem.user_id == cartitem.user_id && existingItem.product_id == cartitem.product_id)
                .SingleOrDefaultAsync();

            if (existingItem == null)
            {
                // Item does not exist in cart, so add it
                var item = new Cart()
                {
                    user_id = cartitem.user_id,
                    product_id = cartitem.product_id,
                    qty = cartitem.qty
                };

                _context.cart_items.Add(item);
                await _context.SaveChangesAsync();
                return HttpStatusCode.Created;
            }
            else
            {
                // Item already exists in cart, so increment quantity
                existingItem.qty = existingItem.qty + cartitem.qty;
                await _context.SaveChangesAsync();
                return HttpStatusCode.OK;
            }
        }

        [HttpPut("updateCartItemQty/{id}")]
        [Authorize]
        public async Task<HttpStatusCode> UpdateCartItem(int id, CartDTO cartitem)
        {
            var item = await _context.cart_items.SingleOrDefaultAsync(cartitem => cartitem.id == id);
            if (item == null)
            {
                return HttpStatusCode.NotFound;
            }

            item.qty = cartitem.qty;

            await _context.SaveChangesAsync();
            return HttpStatusCode.OK;
        }

        [HttpDelete("deleteCartItem/{id}")]
        [Authorize]
        public async Task<HttpStatusCode> RemoveFromCart(int id)
        {
            var item = new Cart()
            {
                id = id
            };

            _context.cart_items.Remove(item);
            await _context.SaveChangesAsync();
            return HttpStatusCode.OK;
        }
    }
}
