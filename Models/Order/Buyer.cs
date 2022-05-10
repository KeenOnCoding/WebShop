using System;
using System.Collections.Generic;
using System.Linq;

namespace WebShop
{
    public class Buyer
    {
        public int Id { get; set; }
        public string IdentityGuid { get; private set; }

        public string Name { get; private set; }

        private IList<PaymentMethod> PaymentMethods;


        protected Buyer()
        {

            PaymentMethods = new List<PaymentMethod>();
        }

        public Buyer(string identity, string name) : this()
        {
            IdentityGuid = !string.IsNullOrWhiteSpace(identity) ? identity : throw new ArgumentNullException(nameof(identity));
            Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentNullException(nameof(name));
        }

        public PaymentMethod VerifyOrAddPaymentMethod(
            int cardTypeId, string alias, string cardNumber,
            string securityNumber, string cardHolderName, DateTime expiration, int orderId)
        {
            var existingPayment = PaymentMethods
                .SingleOrDefault(p => p.IsEqualTo(cardTypeId, cardNumber, expiration));

            if (existingPayment != null)
            {
               // AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, existingPayment, orderId));

                return existingPayment;
            }

            var payment = new PaymentMethod(cardTypeId, alias, cardNumber, securityNumber, cardHolderName, expiration);

            PaymentMethods.Add(payment);

            //AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, payment, orderId));

            return payment;
        }
    }
}
