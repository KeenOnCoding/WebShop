

using System;

namespace WebShop
{

    public class OrderSummary
    {
        public int OrderNumber { get; init; }
        public DateTime Date { get; init; }
        public string Status { get; init; }
        public double Total { get; init; }
    }
}
