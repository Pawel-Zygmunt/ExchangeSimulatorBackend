using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchingEngineApp
{
    internal class PriceLevel
    {
        private readonly List<LimitOrder> _orders;

        public double Price { get; private set; }

        public uint PriceLevelCumulativeQuantity { get; private set; }

        public int OrderCount => _orders.Count;

        public LimitOrder FirstOrder => _orders.First();

        public IEnumerable<LimitOrder> GetOrders() => _orders;

        public PriceLevel(double price)
        {
            PriceLevelCumulativeQuantity = 0;
            Price = price;
            _orders = new List<LimitOrder>();
        }

        public void AddOrder(LimitOrder order)
        {
            PriceLevelCumulativeQuantity += order.InitialQuantity;
            _orders.Add(order);
        }

        public void RemoveOrder(Order order)
        {
            if (PriceLevelCumulativeQuantity < order.InitialQuantity)
                throw new InvalidOperationException($"PriceLevelError: order.InitialQuantity > PriceLevel._quantity");

            PriceLevelCumulativeQuantity -= order.InitialQuantity;

            var index = _orders.FindIndex(order => order.OrderId == order.OrderId);

            if(index== -1)
            {
                throw new InvalidOperationException("Trying to remove order which does not exists");
            }

            if (index != 0)
                throw new InvalidOperationException("Trying to remove order from PriceLevel which is not first");

            _orders.RemoveAt(index);
        }
    }
}
