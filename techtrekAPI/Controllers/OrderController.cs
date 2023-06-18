using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net;
using techtrekAPI.DTO.Order;
using techtrekAPI.Entities;
using techtrekAPI.Controllers;
using Newtonsoft.Json;

namespace techtrekAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly TechtrekContext _context;

        public OrderController(TechtrekContext context)
        {
            _context = context;
        }


        [HttpGet("showOrders")]
        [Authorize (Roles = "admin")]
        public async Task<ActionResult<List<OrderDTO>>> GetOrders()
        {
            var list = await _context.orders
                .Join(_context.users, order => order.user_id, user => user.id,
                (order, user) => new { Order = order, User = user })
                .Select(joinResult => new OrderDTO
                {
                    id = joinResult.Order.id,
                    user_id = joinResult.Order.user_id,
                    first_name = joinResult.User.first_name,
                    last_name = joinResult.User.last_name,
                    order_items = joinResult.Order.order_items,
                    total_cost = joinResult.Order.total_cost,
                    status = joinResult.Order.status,
                    created_at = joinResult.Order.created_at,
                }
                ).ToListAsync();

            if (list.Count == 0)
            {
                return NotFound();
            }
            else
            {
                return Ok(list);
            }
        }


        [HttpGet("showOrder/{id}")]
        [Authorize]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {

            var order = await _context.orders
               .Join(_context.users, order => order.user_id, user => user.id,
               (order, user) => new { Order = order, User = user }).
               Where(joinResult => joinResult.Order.id == id)
               .Select(joinResult => new OrderDTO
               {
                   id = joinResult.Order.id,
                   user_id = joinResult.Order.user_id,
                   first_name = joinResult.User.first_name,
                   last_name = joinResult.User.last_name,
                   order_items = joinResult.Order.order_items,
                   total_cost = joinResult.Order.total_cost,
                   status = joinResult.Order.status,
                   created_at = joinResult.Order.created_at,
               }
               ).SingleOrDefaultAsync();


            if (order == null)
            {
                return NotFound(); // Return a 404 Not Found response if the order is not found
            }

            return Ok(order);
        }

        [HttpGet("showUserOrders/{id}")]
        [Authorize]
        public async Task<ActionResult<List<OrderDTO>>> GetOrdersById(int id)
        {
            var list = await _context.orders
                .Join(_context.users, order => order.user_id, user => user.id,
                (order, user) => new { Order = order, User = user }).
                Where(joinResult => joinResult.Order.user_id == id)
                .Select(joinResult => new OrderDTO
                {
                    id = joinResult.Order.id,
                    user_id = joinResult.Order.user_id,
                    first_name = joinResult.User.first_name,
                    last_name = joinResult.User.last_name,
                    order_items = joinResult.Order.order_items,
                    total_cost = joinResult.Order.total_cost,             
                    status = joinResult.Order.status,
                    created_at = joinResult.Order.created_at,
                }
                ).ToListAsync();

            if (list.Count == 0)
            {
                return NotFound();
            }
            else
            {
                return Ok(list);
            }
        }


        [HttpPost("createOrder")]
        [Authorize]
        public async Task<HttpStatusCode> createOrder(OrderDTO order)
        {
            var entity = new Order()
            {
                user_id = order.user_id,
                total_cost = order.total_cost,
                status = order.status,
                order_items = order.order_items,
                created_at = DateTime.Now,
            };

            _context.orders.Add(entity);
            await _context.SaveChangesAsync();

            // Update product sold quantities and remove cart items
            var cartItems = _context.cart_items.Where(item => item.user_id == order.user_id).ToList();
            foreach (var item in cartItems)
            {
                var itemInDb = await _context.products.SingleOrDefaultAsync(product => product.id == item.product_id);
                if (itemInDb != null)
                {
                    itemInDb.sold += item.qty;
                }
            }

            _context.cart_items.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return HttpStatusCode.Created;
        }

        [HttpPut("updateStatus/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<HttpStatusCode> UpdateOrderStatus(int id, OrderDTO order)
        {
            var orderFromDb = await _context.orders.SingleOrDefaultAsync(order => order.id == id);

            if (orderFromDb == null)
            {
                return HttpStatusCode.NotFound;
            }

            orderFromDb.status = order.status;
            await _context.SaveChangesAsync();
            return HttpStatusCode.OK;
        }

        [HttpGet("generateReceipt/{orderId}")]
        [Authorize]
        public IActionResult GetReceipt(int orderId)
        {
            // Fetch the order details from the database based on the orderId
            var order = FetchOrderFromDatabase(orderId);

            if (order == null)
            {
                return NotFound(); // Return 404 Not Found if the order is not found in the database
            }

            // Generate the receipt
            var receiptContent = GenerateReceiptContent(order);

            // Return the receipt as a downloadable file
            var fileContent = System.Text.Encoding.UTF8.GetBytes(receiptContent);
            return File(fileContent, "text/plain", "receipt.txt");
        }

        // Simulated method to fetch order details from the database
        private OrderModel FetchOrderFromDatabase(int orderId)
        {
            // Simulated data retrieval from the database

            var order = _context.orders
                .Join(_context.users,
                    order => order.user_id,
                    user => user.id,
                    (order, user) => new { Order = order, User = user })
                .FirstOrDefault(o => o.Order.id == orderId);

            if (order == null)
            {
                return null; // Return null if the order is not found in the database
            }

            var orderItems = JsonConvert.DeserializeObject<List<OrderItem>>(order.Order.order_items);

            // Map the fetched order entity to the OrderModel
            var orderModel = new OrderModel
            {
                OrderId = order.Order.id,
                OrderDate = DateTime.Now,
                CustomerName = $"{order.User.first_name} {order.User.last_name}",
                TotalCost = (decimal)order.Order.total_cost,
                OrderItems = orderItems
            };

            return orderModel;
        }



        // Simulated method to generate the receipt content
        private string GenerateReceiptContent(OrderModel order)
        {
            var receiptContent = $"Receipt for Order #{order.OrderId}\n" +
                $"Order Date: {order.OrderDate}\n" +
                $"Customer: {order.CustomerName}\n\n" +
                "Order Items:\n";

            foreach (var item in order.OrderItems)
            {
                var itemTotal = item.qty * item.price;
                receiptContent += $"{item.Name} x {item.qty} - {itemTotal:C}\n";
            }

            receiptContent += $"\nTotal Cost: {order.TotalCost:C}";

            return receiptContent;
        }


    }
}
