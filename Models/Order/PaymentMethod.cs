using System;

namespace WebShop
{
    public class PaymentMethod
    {
        public int Id { get; set; }
        public string Alias { get; set; }
        public string CardNumber { get; set; }
        public string SecurityNumber { get; set; }
        public string CardHolderName { get; set; }
        public DateTime Expiration { get; set; }

        public int CardTypeId { get; set; }
        public CardType CardType { get; set; }


        public PaymentMethod() { }

        public PaymentMethod(int cardTypeId, string alias, string cardNumber, string securityNumber, string cardHolderName, DateTime expiration)
        {

            CardNumber = !string.IsNullOrWhiteSpace(cardNumber) ? cardNumber : throw new Exception(nameof(cardNumber));
            SecurityNumber = !string.IsNullOrWhiteSpace(securityNumber) ? securityNumber : throw new Exception(nameof(securityNumber));
            CardHolderName = !string.IsNullOrWhiteSpace(cardHolderName) ? cardHolderName : throw new Exception(nameof(cardHolderName));

            if (expiration < DateTime.UtcNow)
            {
                throw new Exception(nameof(expiration));
            }

            Alias = alias;
            Expiration = expiration;
            CardTypeId = cardTypeId;
        }

        public bool IsEqualTo(int cardTypeId, string cardNumber, DateTime expiration)
        {
            return CardTypeId == cardTypeId
                && CardHolderName == cardNumber
                && Expiration == expiration;
        }
    }
}
