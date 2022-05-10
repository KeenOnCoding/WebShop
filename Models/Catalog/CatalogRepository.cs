using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebShop
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly string _connectionString;
        public CatalogRepository(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString)); ;
        }

        public async Task<IEnumerable<CardType>> GetCardTypesAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            return await connection.QueryAsync<CardType>("SELECT * FROM CardTypes");
        }

        public async Task<ClientOrder> GetOrderAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var result = await connection.QueryAsync<dynamic>(@"select o.[Id] as ordernumber,
o.OrderDate as date, 
o.Description as description,
o.City as city,
o.Country as country, 
o.State as state, 
o.Street as street, 
o.ZipCode as zipcode,
os.Name as status, 
oi.ProductName as productname, oi.Units as units, oi.UnitPrice as unitprice, oi.PictureUrl as pictureurl
FROM Orders o
LEFT JOIN Orderitems oi ON o.Id = oi.orderid 
LEFT JOIN orderstatus os on o.OrderStatusId = os.Id
WHERE o.Id=@id", new { id });

            if (result.AsList().Count == 0)
                throw new KeyNotFoundException();

            return MapOrderItems(result);
        }

        public async Task<IEnumerable<OrderSummary>> GetOrdersFromUserAsync(Guid userId)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            return await connection.QueryAsync<OrderSummary>(@"SELECT o.[Id] as ordernumber,o.[OrderDate] as [date],os.[Name] as [status], SUM(oi.units*oi.unitprice) as total
                    FROM [Orders] o
                    LEFT JOIN[orderitems] oi ON  o.Id = oi.orderid 
                    LEFT JOIN[orderstatus] os on o.OrderStatusId = os.Id                     
                    LEFT JOIN[buyers] ob on o.BuyerId = ob.Id
                    WHERE o.Buyer = @userId
                    GROUP BY o.[Id], o.[OrderDate], os.[Name] 
                    ORDER BY o.[Id]", new{ userId});
               
        }
        private ClientOrder MapOrderItems(dynamic result)
        {
            var order = new ClientOrder
            {
                ordernumber = result[0].ordernumber,
                date = result[0].date,
                status = result[0].status,
                description = result[0].description,
                street = result[0].street,
                city = result[0].city,
                zipcode = result[0].zipcode,
                country = result[0].country,
                orderitems = new List<Orderitem>(),
                total = 0
            };

            foreach (dynamic item in result)
            {
                var orderitem = new Orderitem
                {
                    productname = item.productname,
                    units = item.units,
                    unitprice = (double)item.unitprice,
                    pictureurl = item.pictureurl
                };

                order.total += item.units * item.unitprice;
                order.orderitems.Add(orderitem);
            }

            return order;
        }
    }
    public record ClientOrder
    {
        public int ordernumber { get; init; }
        public DateTime date { get; init; }
        public string status { get; init; }
        public string description { get; init; }
        public string street { get; init; }
        public string city { get; init; }
        public string zipcode { get; init; }
        public string country { get; init; }
        public List<Orderitem> orderitems { get; set; }
        public decimal total { get; set; }
    }
    public record Orderitem
    {
        public string productname { get; init; }
        public int units { get; init; }
        public double unitprice { get; init; }
        public string pictureurl { get; init; }
    }
}
