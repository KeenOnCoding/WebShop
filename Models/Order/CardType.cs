using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace WebShop
{
    public class CardType
    {     
        public int Id { get; set; }
        public string Name { get;  set; }

        public static CardType Amex = new(nameof(Amex));
        public static CardType Visa = new(nameof(Visa));
        public static CardType MasterCard = new(nameof(MasterCard));

        public CardType(string name)
        {
            Name = name;    
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
            if (obj is not CardType otherValue)
            {
                return false;
            }

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public static int AbsoluteDifference(CardType firstValue, CardType secondValue)
        {
            var absoluteDifference = Math.Abs(firstValue.Id - secondValue.Id);
            return absoluteDifference;
        }

        public static CardType FromValue(int value)
        {
            var matchingItem = Parse<CardType, int>(value, "value", item => item.Id == value);
            return matchingItem;
        }

        public static CardType FromDisplayName(string displayName)
        {
            var matchingItem = Parse<CardType, string>(displayName, "display name", item => item.Name == displayName);
            return matchingItem;
        }

        private static T Parse<T, K>(K value, string description, Func<T, bool> predicate)
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem == null)
                throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");

            return matchingItem;
        }

        public int CompareTo(object other) => Id.CompareTo(((CardType)other).Id);
    }

}

