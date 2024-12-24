using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using WebApplication1.Models;

namespace WebApplication1.Packages
{
    public interface IPKG_BOOKS
    {
        public List<Book> GetBooks();
        public void DeleteBook(int id);
        public Book GetBook(int id);
        public void CreateBook(Book book);
        public void UpdateBook(Book book);
        public List<Book> GetFilteredBooks(string? title, string? author, int? price, int? quantity);
    }
    public class PKG_BOOKS : PKG_BASE, IPKG_BOOKS
    {
        public List<Book> GetBooks()
        {
            List<Book> books = [];
            OracleConnection conn = new()
            {
                ConnectionString = ConnStr
            };

            conn.Open();

            OracleCommand cmd = new()
            {
                Connection = conn,
                CommandText = "olerning.PKG_SANDRO_BOOKS.get_books",
                CommandType = CommandType.StoredProcedure,
            };

            cmd.Parameters.Add("p_result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Book book = new()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Title = reader.GetString(reader.GetOrdinal("title")),
                    Author = reader.GetString(reader.GetOrdinal("author")),
                    Price = reader.GetInt32(reader.GetOrdinal("price")),
                    Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                };

                books.Add(book);

            }

            reader.Close();
            cmd.Dispose();
            conn.Close();

            return books;
        }

        public void CreateBook(Book book)
        {
            using OracleConnection con = new(ConnStr);
            con.Open();

            using OracleCommand cmd = new();
            cmd.Connection = con;
            cmd.CommandText = "olerning.PKG_SANDRO_BOOKS.create_book";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new OracleParameter("p_title", OracleDbType.Varchar2)).Value = book.Title;
            cmd.Parameters.Add(new OracleParameter("p_author", OracleDbType.Varchar2)).Value = book.Author;
            cmd.Parameters.Add(new OracleParameter("p_price", OracleDbType.Varchar2)).Value = book.Price;
            cmd.Parameters.Add(new OracleParameter("p_quantity", OracleDbType.Varchar2)).Value = book.Quantity;

            cmd.ExecuteNonQuery();
        }



        public void UpdateBook(Book book)
        {
            using OracleConnection con = new(ConnStr);
            con.Open();

            using OracleCommand cmd = new();
            cmd.Connection = con;
            cmd.CommandText = "olerning.PKG_SANDRO_BOOKS.update_book";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Int32)).Value = book.Id;
            cmd.Parameters.Add(new OracleParameter("p_name", OracleDbType.Varchar2)).Value = book.Title;
            cmd.Parameters.Add(new OracleParameter("p_author", OracleDbType.Varchar2)).Value = book.Author;
            cmd.Parameters.Add(new OracleParameter("p_price", OracleDbType.Varchar2)).Value = book.Price;
            cmd.Parameters.Add(new OracleParameter("p_quantity", OracleDbType.Varchar2)).Value = book.Quantity;

            cmd.ExecuteNonQuery();
        }



        public Book GetBook(int id)
        {
            Book book;

            using OracleConnection con = new(ConnStr);
            con.Open();

            using OracleCommand cmd = new();
            cmd.Connection = con;
            cmd.CommandText = "olerning.PKG_SANDRO_BOOKS.get_book";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Int32)).Value = id;
            cmd.Parameters.Add("p_result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            try
            {
                using OracleDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    book = new Book
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Title = reader.GetString(reader.GetOrdinal("title")),
                        Author = reader.GetString(reader.GetOrdinal("author")),
                        Price = reader.GetInt32(reader.GetOrdinal("price")),
                        Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                    };
                }
                else
                {
                    throw new KeyNotFoundException($"Card with ID {id} not found.");
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("An error occurred while getting the card: " + ex.Message);
            }

            return book;

        }


        public void DeleteBook(int id)
        {
            using OracleConnection conn = new(ConnStr);
            conn.Open();

            using OracleCommand cmd = new();
            cmd.Connection = conn;
            cmd.CommandText = "olerning.PKG_SANDRO_BOOKS.delete_book";
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
                    throw new KeyNotFoundException($"Card with ID {id} not found.");
                }
                else
                {
                    throw new Exception("An error occurred while deleting the card: " + ex.Message);
                }
            }
        }

        public List<Book> GetFilteredBooks( string? title,  string? author,  int? price,  int? quantity)
        {
            using OracleConnection conn = new()
            {
                ConnectionString = ConnStr
            };

            conn.Open();

            using OracleCommand cmd = new()
            {
                Connection = conn,
                CommandText = "olerning.PKG_SANDRO_BOOKS.get_filtered_books",
                CommandType = CommandType.StoredProcedure,
            };

            cmd.Parameters.Add("p_books", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_title", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(title) ? DBNull.Value : (object)title;
            cmd.Parameters.Add("p_author", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(author) ? DBNull.Value : (object)author;
            cmd.Parameters.Add("p_price", OracleDbType.Decimal).Value = price ?? (object)DBNull.Value;
            cmd.Parameters.Add("p_quantity", OracleDbType.Int32).Value = quantity ?? (object)DBNull.Value;

            List<Book> books = new();

            try
            {
                using OracleDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Book book = new()
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Title = reader.GetString(reader.GetOrdinal("title")),
                        Author = reader.GetString(reader.GetOrdinal("author")),
                        Price = reader.GetInt32(reader.GetOrdinal("price")),
                        Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                    };
                    books.Add(book);
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("An error occurred while getting the doctors: " + ex.Message);
            }

            return books;
        }
    }
}
