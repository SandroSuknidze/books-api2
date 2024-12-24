using Oracle.ManagedDataAccess.Client;
using System.Data;
using WebApplication1.Models;

namespace WebApplication1.Packages
{
    public interface IPKG_ORDERS
    {
        public List<Order> GetOrders();
        public void DeleteOrder(int id);
        public Order GetOrder(int id);
        public void CreateOrder(Order order);
        public void UpdateOrder(Order order);
        public void AcceptOrder(int orderId);
    }

    public class PKG_ORDERS : PKG_BASE, IPKG_ORDERS
        {
            public List<Order> GetOrders()
            {
                List<Order> orders = [];
                OracleConnection conn = new()
                {
                    ConnectionString = ConnStr
                };

                conn.Open();

                OracleCommand cmd = new()
                {
                    Connection = conn,
                    CommandText = "olerning.PKG_SANDRO_ORDERS.get_orders",
                    CommandType = CommandType.StoredProcedure,
                };

                cmd.Parameters.Add("p_result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                OracleDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Order order = new()
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        BookId = reader.GetInt32(reader.GetOrdinal("book_id")),
                        Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                        UserName = reader.GetString(reader.GetOrdinal("user_name"))
                    };

                    orders.Add(order);
                }

                reader.Close();
                cmd.Dispose();
                conn.Close();

                return orders;
            }

            public void CreateOrder(Order order)
            {
                using OracleConnection con = new(ConnStr);
                con.Open();

                using OracleCommand cmd = new();
                cmd.Connection = con;
                cmd.CommandText = "olerning.PKG_SANDRO_ORDERS.create_order";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new OracleParameter("p_book_id", OracleDbType.Int32)).Value = order.BookId;
                cmd.Parameters.Add(new OracleParameter("p_quantity", OracleDbType.Int32)).Value = order.Quantity;
                cmd.Parameters.Add(new OracleParameter("p_user_name", OracleDbType.Varchar2)).Value = order.UserName;

                cmd.ExecuteNonQuery();
            }

            public void UpdateOrder(Order order)
            {
                using OracleConnection con = new(ConnStr);
                con.Open();

                using OracleCommand cmd = new();
                cmd.Connection = con;
                cmd.CommandText = "olerning.PKG_SANDRO_ORDERS.update_order";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Int32)).Value = order.Id;
                cmd.Parameters.Add(new OracleParameter("p_quantity", OracleDbType.Int32)).Value = order.Quantity;
                cmd.Parameters.Add(new OracleParameter("p_user_name", OracleDbType.Varchar2)).Value = order.UserName;

                cmd.ExecuteNonQuery();
            }

            public Order GetOrder(int id)
            {
                Order order;

                using OracleConnection con = new(ConnStr);
                con.Open();

                using OracleCommand cmd = new();
                cmd.Connection = con;
                cmd.CommandText = "olerning.PKG_SANDRO_ORDERS.get_order";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Int32)).Value = id;
                cmd.Parameters.Add("p_result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                try
                {
                    using OracleDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        order = new Order
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            BookId = reader.GetInt32(reader.GetOrdinal("book_id")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                            UserName = reader.GetString(reader.GetOrdinal("user_name"))
                        };
                    }
                    else
                    {
                        throw new KeyNotFoundException($"Order with ID {id} not found.");
                    }
                }
                catch (OracleException ex)
                {
                    throw new Exception("An error occurred while getting the order: " + ex.Message);
                }

                return order;
            }

            public void DeleteOrder(int id)
            {
                using OracleConnection conn = new(ConnStr);
                conn.Open();

                using OracleCommand cmd = new();
                cmd.Connection = conn;
                cmd.CommandText = "olerning.PKG_SANDRO_ORDERS.delete_order";
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
                        throw new KeyNotFoundException($"Order with ID {id} not found.");
                    }
                    else
                    {
                        throw new Exception("An error occurred while deleting the order: " + ex.Message);
                    }
                }
            }

        public void AcceptOrder(int orderId)
        {
            using OracleConnection con = new(ConnStr);
            con.Open();

            using OracleCommand cmd = new();
            cmd.Connection = con;
            cmd.CommandText = "olerning.PKG_SANDRO_ORDERS.accept_order";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new OracleParameter("p_order_id", OracleDbType.Int32)).Value = orderId;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (OracleException ex)
            {
                throw new Exception("An error occurred while accepting the order: " + ex.Message);
            }
        }
    }
    
}
