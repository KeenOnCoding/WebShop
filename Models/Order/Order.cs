using System;
using System.Collections.Generic;
using System.Linq;

namespace WebShop
{
    public class Order
    {
        public int Id { get; set; }
        public int? BuyerId { get; set; }
        public int OrderStatusId { get; set; }
        public DateTime OrderDate { get; set; }
        public int CardTypeId { get; set; }
        public string Buyer { get; set; }
        public bool IsDraft { get; set; }
        public int? PaymentMethod { get; set; }
        public string Description { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public DateTime CardExpiration { get; set; }
        public string CardSecurityNumber { get; set; }

        public OrderStatus OrderStatus { get; set; }
        public List<OrderItem> OrderItems { get; set; }

        public static Order NewDraft()
        {
            var order = new Order();
            order.IsDraft = true;
            return order;
        }

        public Order()
        {
            OrderItems = new List<OrderItem>();
            IsDraft = false;
        }

        public Order(string userId, string userName, Address address, int cardTypeId, string cardNumber, string cardSecurityNumber,string cardHolderName, DateTime cardExpiration, int? buyerId = null, int? paymentMethodId = null) : this()
        {
            BuyerId = buyerId;
            PaymentMethod = paymentMethodId;
            OrderStatusId = OrderStatus.Submitted.Id;
            OrderDate = DateTime.UtcNow;
            Street = address.Street;
            City = address.City;
            State = address.State;
            Country = address.Country;
            ZipCode = address.ZipCode;

            // Add the OrderStarterDomainEvent to the domain events collection  to be raised/dispatched when comitting changes into the Database [ After DbContext.SaveChanges() ]
            //AddOrderStartedDomainEvent(userId, userName, cardTypeId, cardNumber,cardSecurityNumber, cardHolderName, cardExpiration);
        }

        // DDD Patterns comment
        // This Order AggregateRoot's method "AddOrderitem()" should be the only way to add Items to the Order,
        // so any behavior (discounts, etc.) and validations are controlled by the AggregateRoot 
        // in order to maintain consistency between the whole Aggregate. 


        public void AddOrderItem(int productId, string productName, decimal unitPrice, decimal discount, string pictureUrl, int units = 1)
        {
            var existingOrderForProduct = OrderItems.Where(o => o.ProductId == productId)
                .SingleOrDefault();

            if (existingOrderForProduct != null)
            {
                //if previous line exist modify it with higher discount  and units..

                if (discount > existingOrderForProduct.GetCurrentDiscount())
                {
                    existingOrderForProduct.SetNewDiscount(discount);
                }

                existingOrderForProduct.AddUnits(units);
            }
            else
            {
                //add validated new order item

                var orderItem = new OrderItem(productId, productName, unitPrice, discount, pictureUrl, units);
                OrderItems.Add(orderItem);
            }
        }

        public void SetPaymentId(int id)
        {
            PaymentMethod = id;
        }

        public void SetBuyerId(int id)
        {
            BuyerId = id;
        }

        public void SetAwaitingValidationStatus()
        {
            if (OrderStatusId == OrderStatus.Submitted.Id)
            {
                //AddDomainEvent(new OrderStatusChangedToAwaitingValidationDomainEvent(Id, OrderItems));
                OrderStatusId = OrderStatus.AwaitingValidation.Id;
            }
        }

        public void SetStockConfirmedStatus()
        {
            if (OrderStatusId == OrderStatus.AwaitingValidation.Id)
            {
                //AddDomainEvent(new OrderStatusChangedToStockConfirmedDomainEvent(Id));

                OrderStatusId = OrderStatus.StockConfirmed.Id;
                Description = "All the items were confirmed with available stock.";
            }
        }

        public void SetPaidStatus()
        {
            if (OrderStatusId == OrderStatus.StockConfirmed.Id)
            {
                //AddDomainEvent(new OrderStatusChangedToPaidDomainEvent(Id, OrderItems));

                OrderStatusId = OrderStatus.Paid.Id;
                Description = "The payment was performed at a simulated \"American Bank checking bank account ending on XX35071\"";
            }
        }

        public void SetShippedStatus()
        {
            if (OrderStatusId != OrderStatus.Paid.Id)
            {
                StatusChangeException(OrderStatus.Shipped);
            }

            OrderStatusId = OrderStatus.Shipped.Id;
            Description = "The order was shipped.";
            //AddDomainEvent(new OrderShippedDomainEvent(this));
        }

        public void SetCancelledStatus()
        {
            if (OrderStatusId == OrderStatus.Paid.Id ||
                OrderStatusId == OrderStatus.Shipped.Id)
            {
                StatusChangeException(OrderStatus.Cancelled);
            }

            OrderStatusId = OrderStatus.Cancelled.Id;
            Description = $"The order was cancelled.";
            //AddDomainEvent(new OrderCancelledDomainEvent(this));
        }

        public void SetCancelledStatusWhenStockIsRejected(IEnumerable<int> orderStockRejectedItems)
        {
            if (OrderStatusId == OrderStatus.AwaitingValidation.Id)
            {
                OrderStatusId = OrderStatus.Cancelled.Id;

                var itemsStockRejectedProductNames = OrderItems
                    .Where(c => orderStockRejectedItems.Contains(c.ProductId))
                    .Select(c => c.GetOrderItemProductName());

                var itemsStockRejectedDescription = string.Join(", ", itemsStockRejectedProductNames);
                Description = $"The product items don't have stock: ({itemsStockRejectedDescription}).";
            }
        }

        /*
        private void AddOrderStartedDomainEvent(string userId, string userName, int cardTypeId, string cardNumber,
                string cardSecurityNumber, string cardHolderName, DateTime cardExpiration)
        {
            //var orderStartedDomainEvent = new OrderStartedDomainEvent(this, userId, userName, cardTypeId,cardNumber, cardSecurityNumber,cardHolderName, cardExpiration);

            //this.AddDomainEvent(orderStartedDomainEvent);
        }
        */

        private void StatusChangeException(OrderStatus orderStatusToChange)
        {
            //throw new OrderingDomainException($"Is not possible to change the order status from {OrderStatus.Name} to {orderStatusToChange.Name}.");
        }

        public decimal GetTotal()
        {
            return OrderItems.Sum(o => o.GetUnits() * o.GetUnitPrice());
        }
    }
}
