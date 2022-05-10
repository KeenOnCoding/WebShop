using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WebShop
{
    public class OrderStatus
    {
        public int Id { get;  set; }
        public string Name { get;  set; }

        public static OrderStatus Submitted = new OrderStatus(nameof(Submitted).ToLowerInvariant());
        public static OrderStatus AwaitingValidation = new OrderStatus(nameof(AwaitingValidation).ToLowerInvariant());
        public static OrderStatus StockConfirmed = new OrderStatus(nameof(StockConfirmed).ToLowerInvariant());
        public static OrderStatus Paid = new OrderStatus( nameof(Paid).ToLowerInvariant());
        public static OrderStatus Shipped = new OrderStatus(nameof(Shipped).ToLowerInvariant());
        public static OrderStatus Cancelled = new OrderStatus(nameof(Cancelled).ToLowerInvariant());

        public OrderStatus(string name)
        {
            Name = name;
        }
        public OrderStatus()
        {
        }
        public override string ToString() => Name;

        public static IEnumerable<T> GetAll<T>() =>
            typeof(T).GetFields(BindingFlags.Public |
                                BindingFlags.Static |
                                BindingFlags.DeclaredOnly)
                        .Select(f => f.GetValue(null))
                        .Cast<T>();

        public override bool Equals(object obj)
        {
            if (obj is not OrderStatus otherValue)
            {
                return false;
            }

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public static int AbsoluteDifference(OrderStatus firstValue, OrderStatus secondValue)
        {
            var absoluteDifference = Math.Abs(firstValue.Id - secondValue.Id);
            return absoluteDifference;
        }

        public static OrderStatus FromValue(int value) 
        {
            var matchingItem = Parse<OrderStatus, int>(value, "value", item => item.Id == value);
            return matchingItem;
        }

        public static OrderStatus FromDisplayName (string displayName)
        {
            var matchingItem = Parse<OrderStatus, string>(displayName, "display name", item => item.Name == displayName);
            return matchingItem;
        }

        private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) 
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem == null)
                throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");

            return matchingItem;
        }

        public int CompareTo(object other) => Id.CompareTo(((OrderStatus)other).Id);
    }
}

