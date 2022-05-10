using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebShop
{
    public interface ICartRepository
    {
        Task<CustomerCart> GetCartAsync(string customerId);
        IEnumerable<string> GetUsers();
        Task<CustomerCart> UpdateCartAsync(CustomerCart basket);
        Task<bool> DeleteCartAsync(string id);
    }
}
