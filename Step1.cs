using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Refactoring
{
    internal class step1
    {
        string _connectionString;

        public class UserWithOrdersDto
        {
            // Реализация класса
            public int UserId { get; set; }
            public string Name { get; set; }
            public List<OrderDto> Orders { get; set; }
        }

        public class OrderDto
        {
            public int OrderId { get; set; }
            public decimal Total { get; set; }
        }

        //переменная класса
        public List<UserWithOrdersDto> GetUserWithOrders()
        {
            var result = new List<UserWithOrdersDto>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                const string usersSql = "SELECT Id, Name FROM Users"; 
                using (var userCmd = new SqlCommand(usersSql, connection))
                using (var usersReader = userCmd.ExecuteReader())
                {
                    while (usersReader.Read())
                    {
                        var userId = usersReader.GetInt32(usersReader.GetOrdinal("Id")); // Исправлено: userIds -> userId
                        var userName = usersReader.GetString(usersReader.GetOrdinal("Name"));
                        const string orderSql = "SELECT Id, Total FROM Orders WHERE UserId = @userId"; 
                        using (var ordersCmd = new SqlCommand(orderSql, connection))
                        {
                            ordersCmd.Parameters.AddWithValue("@userId", userId); 
                            using (var ordersReader = ordersCmd.ExecuteReader())
                            {
                                var orders = new List<OrderDto>();
                                while (ordersReader.Read())
                                {
                                    orders.Add(new OrderDto
                                    {
                                        OrderId = ordersReader.GetInt32(ordersReader.GetOrdinal("Id")),
                                        Total = ordersReader.GetDecimal(ordersReader.GetOrdinal("Total"))
                                    });
                                }
                                result.Add(new UserWithOrdersDto 
                                {
                                    UserId = userId,
                                    Name = userName,
                                    Orders = orders
                                });
                            }
                        }
                    }
                }
            }
            return result; 
        }
    }
}