using Oracle.ManagedDataAccess.Client;
using System.Data;
using WebApplication1.Models;

namespace WebApplication1.Packages
{
    public interface IPKG_CART
    {
        public List<Cart> GetCartItems();
        public void DeleteCartItem(int id);
        public Cart GetCartItem(int id);
        public void CreateCartItem(Cart cart);
        public void UpdateCartItem(Cart cart);
        public Cart GetCartItemByBookId(int bookId);
        public List<CartWithBook> GetCartItemsWithBooks();
        public void EmptyCart();

    }

    public class PKG_CART : PKG_BASE, IPKG_CART
    {
        public List<Cart> GetCartItems()
        {
            List<Cart> cartItems = [];
            OracleConnection conn = new()
            {
                ConnectionString = ConnStr
            };

            conn.Open();

            OracleCommand cmd = new()
            {
                Connection = conn,
                CommandText = "olerning.PKG_SANDRO_CART.get_cart_items",
                CommandType = CommandType.StoredProcedure,
            };

            cmd.Parameters.Add("p_result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Cart cartItem = new()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    BookId = reader.GetInt32(reader.GetOrdinal("book_id")),
                    Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                };

                cartItems.Add(cartItem);
            }

            reader.Close();
            cmd.Dispose();
            conn.Close();

            return cartItems;
        }

        public void CreateCartItem(Cart cart)
        {
            using OracleConnection con = new(ConnStr);
            con.Open();

            using OracleCommand cmd = new();
            cmd.Connection = con;
            cmd.CommandText = "olerning.PKG_SANDRO_CART.create_cart_item";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new OracleParameter("p_book_id", OracleDbType.Int32)).Value = cart.BookId;
            cmd.Parameters.Add(new OracleParameter("p_quantity", OracleDbType.Int32)).Value = cart.Quantity;

            cmd.ExecuteNonQuery();
        }

        public void UpdateCartItem(Cart cart)
        {
            using OracleConnection con = new(ConnStr);
            con.Open();

            using OracleCommand cmd = new();
            cmd.Connection = con;
            cmd.CommandText = "olerning.PKG_SANDRO_CART.update_cart_item";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Int32)).Value = cart.Id;
            cmd.Parameters.Add(new OracleParameter("p_quantity", OracleDbType.Int32)).Value = cart.Quantity;

            cmd.ExecuteNonQuery();
        }

        public Cart GetCartItem(int id)
        {
            Cart cart;

            using OracleConnection con = new(ConnStr);
            con.Open();

            using OracleCommand cmd = new();
            cmd.Connection = con;
            cmd.CommandText = "olerning.PKG_SANDRO_CART.get_cart_item";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Int32)).Value = id;
            cmd.Parameters.Add("p_result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            try
            {
                using OracleDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    cart = new Cart
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        BookId = reader.GetInt32(reader.GetOrdinal("book_id")),
                        Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                    };
                }
                else
                {
                    throw new KeyNotFoundException($"Cart item with ID {id} not found.");
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("An error occurred while getting the cart item: " + ex.Message);
            }

            return cart;
        }

        public void DeleteCartItem(int id)
        {
            using OracleConnection conn = new(ConnStr);
            conn.Open();

            using OracleCommand cmd = new();
            cmd.Connection = conn;
            cmd.CommandText = "olerning.PKG_SANDRO_CART.delete_cart_item";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Int32)).Value = id;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (OracleException ex)
            {
                if (ex.Number == 20001)
                {
                    throw new KeyNotFoundException($"Cart item with ID {id} not found.");
                }
                else
                {
                    throw new Exception("An error occurred while deleting the cart item: " + ex.Message);
                }
            }
        }

        public Cart GetCartItemByBookId(int bookId)
        {
            Cart cart = null;
            using OracleConnection con = new(ConnStr);
            con.Open();

            using OracleCommand cmd = new();
            cmd.Connection = con;
            cmd.CommandText = "olerning.PKG_SANDRO_CART.get_cart_item_by_book_id";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new OracleParameter("p_book_id", OracleDbType.Int32)).Value = bookId;
            cmd.Parameters.Add("p_result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            try
            {
                using OracleDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    cart = new Cart
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        BookId = reader.GetInt32(reader.GetOrdinal("book_id")),
                        Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                    };
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("An error occurred while fetching the cart item by book ID: " + ex.Message);
            }

            return cart;
        }

        public List<CartWithBook> GetCartItemsWithBooks()
        {
            List<CartWithBook> cartWithBooks = new();
            using OracleConnection conn = new(ConnStr);
            conn.Open();

            using OracleCommand cmd = new()
            {
                Connection = conn,
                CommandText = "olerning.PKG_SANDRO_CART.get_cart_items_with_books",
                CommandType = CommandType.StoredProcedure,
            };

            cmd.Parameters.Add("p_result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            using OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                CartWithBook cartWithBook = new()
                {
                    BookId = reader.GetInt32(reader.GetOrdinal("book_id")),
                    Title = reader.GetString(reader.GetOrdinal("title")),
                    Author = reader.GetString(reader.GetOrdinal("author")),
                    Price = reader.GetDecimal(reader.GetOrdinal("price")),
                    StockQuantity = reader.GetInt32(reader.GetOrdinal("stock_quantity")),
                    CartQuantity = reader.GetInt32(reader.GetOrdinal("cart_quantity")),
                };

                cartWithBooks.Add(cartWithBook);
            }

            return cartWithBooks;
        }

        public void EmptyCart()
        {
            using OracleConnection conn = new(ConnStr);
            conn.Open();

            using OracleCommand cmd = new();
            cmd.Connection = conn;
            cmd.CommandText = "olerning.PKG_SANDRO_CART.empty_cart";
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (OracleException ex)
            {
                throw new Exception("An error occurred while deleting the cart items: " + ex.Message);
            }
        }



    }
}