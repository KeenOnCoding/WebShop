using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebShop
{
    public interface ICatalogRepository
    {
        Task<IEnumerable<OrderSummary>> GetOrdersFromUserAsync(Guid userId);
        Task<ClientOrder> GetOrderAsync(int id);
        Task<IEnumerable<CardType>> GetCardTypesAsync();
    }
}
