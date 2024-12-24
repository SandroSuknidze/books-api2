using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Packages;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController(IPKG_CART pKG_CART, IPKG_BOOKS pKG_BOOKS) : ControllerBase
    {
        readonly IPKG_CART carts_package = pKG_CART;
        readonly IPKG_BOOKS books_package = pKG_BOOKS;

        [HttpGet]
        public ActionResult<List<Cart>> Get()
        {
            List<Cart> cartItems = carts_package.GetCartItems();
            return Ok(cartItems);
        }

        [HttpGet("{id}")]
        public ActionResult<Cart> Get(int id)
        {
            try
            {
                Cart cartItem;
                cartItem = carts_package.GetCartItem(id);
                return Ok(cartItem);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Cart item with ID {id} not found.");

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Post(Cart cart)
        {
            if (cart == null)
            {
                return BadRequest("Cart Item info required");
            } 
            else if (cart.Quantity == 0)
            {
                return BadRequest("Quantity can't be 0");
            }
            else
            {
                try
                {
                    Book book = books_package.GetBook(cart.BookId);
                    if (book.Quantity < cart.Quantity)
                    {
                        return BadRequest("Quantity can't be increased");
                    }

                    Cart cartItem = carts_package.GetCartItemByBookId(cart.BookId);
                    if ((cartItem != null && (cart.Quantity + cartItem.Quantity > book.Quantity)))
                    {
                        return BadRequest("Quantity can not be increased");
                    }

                    carts_package.CreateCartItem(cart);
                    return Created();

                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "An error post occurred: " + ex.Message);
                }
            }
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, Cart cart)
        {
            try
            {
                if (cart.Id != id)
                {
                    return BadRequest("Cart ID in the URL does not match the ID in the request body.");
                }

                Book book = books_package.GetBook(cart.BookId);
                if (book.Quantity < cart.Quantity)
                {
                    return BadRequest("Quantity can't be increased");
                }

                Cart cartItem = carts_package.GetCartItemByBookId(cart.BookId);
                if ((cartItem != null && (cart.Quantity + cartItem.Quantity > book.Quantity)))
                {
                    return BadRequest("Quantity can not be increased");
                }
                carts_package.UpdateCartItem(cart);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Cart item with ID {cart.Id} not found.");
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
                carts_package.DeleteCartItem(id);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Cart Item with ID {id} not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [HttpGet("with-books")]
        public ActionResult<List<CartWithBook>> GetCartItemsWithBooks()
        {
            try
            {
                List<CartWithBook> cartWithBooks = carts_package.GetCartItemsWithBooks();
                return Ok(cartWithBooks);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
        }
    }
}
