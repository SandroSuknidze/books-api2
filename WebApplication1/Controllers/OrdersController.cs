using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Packages;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(IPKG_ORDERS pKG_ORDERS, IPKG_CART pKG_CART) : ControllerBase
    {
        readonly IPKG_ORDERS orders_package = pKG_ORDERS;
        readonly IPKG_CART cart_package = pKG_CART;

        [HttpGet]
        public ActionResult<List<Order>> Get()
        {
            List<Order> orders = orders_package.GetOrders();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public ActionResult<Order> Get(int id)
        {
            try
            {
                Order order;
                order = orders_package.GetOrder(id);
                return Ok(order);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Order with ID {id} not found.");

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Post([FromBody] List<Order> orders)
        {
            if (orders == null || !orders.Any())
            {
                return BadRequest("Order info required");
            }

            try
            {
                foreach (var order in orders)
                {
                    orders_package.CreateOrder(order);
                }

                cart_package.EmptyCart();

                return CreatedAtAction(nameof(Post), new { count = orders.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
        }





        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                orders_package.DeleteOrder(id);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Order with ID {id} not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
        }


        [HttpPost("accept/{id}")]
        public ActionResult AcceptOrder(int id)
        {
            try
            {
                orders_package.AcceptOrder(id);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Order with ID {id} not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
        }
    }
}
