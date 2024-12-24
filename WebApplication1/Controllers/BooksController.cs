using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Packages;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController(IPKG_BOOKS pKG_BOOKS) : ControllerBase
    {
        readonly IPKG_BOOKS books_package = pKG_BOOKS;
        [HttpGet]
        public ActionResult<List<Book>> Get()
        {
            List<Book> books = books_package.GetBooks();
            return Ok(books);
        }

        [HttpGet("{id}")]
        public ActionResult<Book> Get(int id)
        {
            try
            {
                Book book;
                book = books_package.GetBook(id);
                return Ok(book);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Book with ID {id} not found.");

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Post(Book book)
        {
            if (book == null)
            {
                return BadRequest("Card info required");
            }
            else
            {
                try
                {
                    books_package.CreateBook(book);
                    return Created();

                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
                }
            }
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, Book book)
        {
            try
            {
                if (book.Id != id)
                {
                    return BadRequest("Book ID in the URL does not match the ID in the request body.");
                }
                books_package.UpdateBook(book);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Book with ID {book.Id} not found.");
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
                books_package.DeleteBook(id);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Book with ID {id} not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [HttpGet("filtered")]
        public ActionResult<List<Book>> Get([FromQuery] string? title, [FromQuery] string? author, [FromQuery] int? price, [FromQuery] int? quantity)
        {
            var books = books_package.GetFilteredBooks(title, author, price, quantity);
            return books;
        }
    }
}
