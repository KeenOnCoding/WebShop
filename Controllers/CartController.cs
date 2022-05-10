using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebShop
{
    [ApiController]
    [Route("[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _repository;

        private readonly ApplicationDbContext _catalogContext;
        public CartController(ICartRepository repository, ApplicationDbContext context)
        {
            _repository = repository;
            _catalogContext = context;  
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerCart>> GetCartByIdAsync(string id)
        {
            var basket = await _repository.GetCartAsync(id);

            return Ok(basket ?? new CustomerCart(id));
        }

        [HttpPost]
        [ProducesResponseType(typeof(CustomerCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerCart>> UpdateCartAsync([FromBody] CustomerCart value)
        {
            return Ok(await _repository.UpdateCartAsync(value));
        }

        
        [Route("checkout")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public  IActionResult CheckoutAsync([FromBody] CartCheckout basketCheckout, [FromHeader(Name = "x-requestid")] string requestId)
        {
            //var userId = _identityService.GetUserIdentity();

            //basketCheckout.RequestId = (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty) ? guid : basketCheckout.RequestId;

            _catalogContext.Orders.Add(new Order
            {
                City = basketCheckout.City,
                Street = basketCheckout.Street,
                State = basketCheckout.State,
                Country = basketCheckout.Country,
                ZipCode = basketCheckout.ZipCode,
                CardNumber = basketCheckout.CardNumber,
                CardHolderName = basketCheckout.CardHolderName,
                CardExpiration = basketCheckout.CardExpiration,
                CardSecurityNumber = basketCheckout.CardSecurityNumber,
                CardTypeId = basketCheckout.CardTypeId,
                Buyer = basketCheckout.Buyer,
                OrderStatus = new OrderStatus() { Name = "paid" },
                OrderItems = basketCheckout.OrderItems
            });

            _catalogContext.SaveChanges();

            //var basket =  _repository.GetCartAsync(userId);
            //if (basket == null)
            //{
            //    return BadRequest();
            //}

            //var userName = this.HttpContext.User.FindFirst(x => x.Type == ClaimTypes.Name).Value;

            //var eventMessage = new UserCheckoutAcceptedIntegrationEvent(userId, userName, basketCheckout.City, basketCheckout.Street,
            //   basketCheckout.State, basketCheckout.Country, basketCheckout.ZipCode, basketCheckout.CardNumber, basketCheckout.CardHolderName,
            //   basketCheckout.CardExpiration, basketCheckout.CardSecurityNumber, basketCheckout.CardTypeId, basketCheckout.Buyer, basketCheckout.RequestId, basket);

            // Once basket is checkout, sends an integration event to
            // ordering.api to convert basket to order and proceeds with
            // order creation process
            try
            {
                //_eventBus.Publish(eventMessage);
            }
            catch (Exception)
            {
                //_logger.LogError(ex, "ERROR Publishing integration event: {IntegrationEventId} from {AppName}", eventMessage.Id, Program.AppName);

                throw ;
            }

            return Accepted();
        }
        
        // DELETE api/values/5
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task DeleteCartByIdAsync(string id)
        {
            await _repository.DeleteCartAsync(id);
        }
    }
}
