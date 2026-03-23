using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactoring
{
    internal class Program
    {
        public class UserInterface
        {
            private readonly SqlConnection _connection;
            public UserInterface(SqlConnection connection)
            {
                _connection = connection;
            }

            public List<object> GetUsersWithOrder()
            {
                var result = new List <object>();
                var usersCmd = new SqlCommand("SELECT * FROM Users", _connection);
                var usersReader = usersCmd.ExecuteReader();
                while (usersReader.Read())
                {
                    var userId = usersReader.GetInt32(0);
                    var userName = usersReader.GetString(1);
                    var orderCmd = new SqlCommand($"SELECT * FROM Orders where UserId={userId}", _connection);
                    var ordersReader = orderCmd.ExecuteReader();
                    var orders = new List<object>();
                    while (ordersReader.Read())
                    {
                        orders.Add(new { OrderId = ordersReader.GetInt32(0), Total=ordersReader.GetDecimal(2)});
                    }
                    ordersReader.Close();
                    result.Add(new
                    {
                        UserId = userId,
                        Name = userName,
                        Orders = orders
                    });
                }
                usersReader.Close();
                return result;
            }
        }
        static void Main(string[] args)
        {

        }
    }
}