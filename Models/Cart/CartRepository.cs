
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebShop
{
    public class CartRepository: ICartRepository
    {
        private readonly ILogger<CartRepository> _logger;
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public CartRepository(ILoggerFactory loggerFactory, IConnectionMultiplexer redis)
        {
            _logger = loggerFactory.CreateLogger<CartRepository>();
            _redis = redis;
            _database = redis.GetDatabase();
        }


        public async Task<CustomerCart> GetCartAsync(string customerId)
        {
            var data = await _database.StringGetAsync(customerId);

            if (data.IsNullOrEmpty)
            {
                return null;
            }

            return JsonSerializer.Deserialize<CustomerCart>(data, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<CustomerCart> UpdateCartAsync(CustomerCart basket)
        {
            var created = await _database.StringSetAsync(basket.BuyerId, JsonSerializer.Serialize(basket));

            if (!created)
            {
                return null;
            }

            return await GetCartAsync(basket.BuyerId);
        }

        private IServer GetServer()
        {
            var endpoint = _redis.GetEndPoints();
            return _redis.GetServer(endpoint.First());
        }


        public async Task<bool> DeleteCartAsync(string id)
        {
            return await _database.KeyDeleteAsync(id);
        }

        public IEnumerable<string> GetUsers()
        {
            var server = GetServer();
            var data = server.Keys();

            return data?.Select(k => k.ToString());
        }
    }
}
