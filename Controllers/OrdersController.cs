
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebShop.Models;

namespace WebShop.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ICatalogRepository _orderContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(ICatalogRepository catalogContext, UserManager<ApplicationUser> userManager)
        {
            _orderContext = catalogContext;
            _userManager = userManager;
        }

        public async Task<string> GetCurrentUserId()
        {
            var usr = await _userManager.GetUserAsync(HttpContext.User);
            return usr?.Id;
        }

        /*
        [Route("cancel")]
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CancelOrderAsync([FromBody] CancelOrderCommand command, [FromHeader(Name = "x-requestid")] string requestId)
        {
            bool commandResult = false;

            if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
            {
                var requestCancelOrder = new IdentifiedCommand<CancelOrderCommand, bool>(command, guid);

                _logger.LogInformation(
                    "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                    requestCancelOrder.GetGenericTypeName(),
                    nameof(requestCancelOrder.Command.OrderNumber),
                    requestCancelOrder.Command.OrderNumber,
                    requestCancelOrder);

                commandResult = await _mediator.Send(requestCancelOrder);
            }

            if (!commandResult)
            {
                return BadRequest();
            }
            
            return Ok();
        }
        */
        /*
        [Route("ship")]
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ShipOrderAsync([FromBody] ShipOrderCommand command, [FromHeader(Name = "x-requestid")] string requestId)
        {
            bool commandResult = false;

            if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
            {
                var requestShipOrder = new IdentifiedCommand<ShipOrderCommand, bool>(command, guid);

                _logger.LogInformation(
                    "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                    requestShipOrder.GetGenericTypeName(),
                    nameof(requestShipOrder.Command.OrderNumber),
                    requestShipOrder.Command.OrderNumber,
                    requestShipOrder);

                commandResult = await _mediator.Send(requestShipOrder);
            }

            if (!commandResult)
            {
                return BadRequest();
            }

            return Ok();
        }
        */

        [Route("{orderId:int}")]
        [HttpGet]
        [ProducesResponseType(typeof(Order), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetOrderAsync(int orderId)
        {
            try
            {
                //Todo: It's good idea to take advantage of GetOrderByIdQuery and handle by GetCustomerByIdQueryHandler
                //var order customer = await _mediator.Send(new GetOrderByIdQuery(orderId));
                var order = await _orderContext.GetOrderAsync(orderId);   

                return Ok(order);
            }
            catch
            {
                return NotFound();
            }
        }
        
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderSummary>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<OrderSummary>>> GetOrdersAsync()
        {
            var userId = "32c0cc83-ee3d-4b9a-8f21-fddcb1a15ea4";
            //var userId = await GetCurrentUserId();
            //var = userId = _context.Users
            //var userid = _identityService.GetUserIdentity();
            var orders = await _orderContext.GetOrdersFromUserAsync(Guid.Parse(userId));

            return Ok(orders);
        }
        
        [Route("cardtypes")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CardType>), (int)HttpStatusCode.OK)]
        public  IActionResult GetCardTypesAsync()
        {
            var cardTypes = _orderContext.GetCardTypesAsync();

            return Ok(cardTypes);
        }

        /*
        [Route("draft")]
        [HttpPost]
        public async Task<ActionResult<OrderDraftDTO>> CreateOrderDraftFromBasketDataAsync([FromBody] CreateOrderDraftCommand createOrderDraftCommand)
        {
            _logger.LogInformation(
                "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                createOrderDraftCommand.GetGenericTypeName(),
                nameof(createOrderDraftCommand.BuyerId),
                createOrderDraftCommand.BuyerId,
                createOrderDraftCommand);

            return await _mediator.Send(createOrderDraftCommand);
        }
        */
    }
}
