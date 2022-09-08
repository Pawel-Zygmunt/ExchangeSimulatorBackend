using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchingEngineApp
{
    public abstract class Order
    {
        public Order(Guid userId, uint initialQuantity, OrderType type)
        {
            if (initialQuantity == 0)
                throw new ArgumentOutOfRangeException("initialQuantity cannot be 0, Order constructor");

            OrderId = Guid.NewGuid();
            UserId = userId;
            InitialQuantity = initialQuantity;
            CurrentQuantity = initialQuantity;
            Type = type;
        }

        public OrderType Type { get; private set; }

        public int timestamp;
        public Guid OrderId { get; private set; }
        public uint InitialQuantity { get; private set; }
        public uint CurrentQuantity { get; private set; }

        public bool IsTotallyFilled => CurrentQuantity == 0;

        public bool IsPartiallyFilled => CurrentQuantity < InitialQuantity && CurrentQuantity > 0;

        public bool IsNotFilledAtAll => CurrentQuantity == InitialQuantity;

        public Guid UserId { get; private set; }

        public void DecreaseQuantity(uint quantityDelta)
        {
            if (quantityDelta > CurrentQuantity)
                throw new InvalidOperationException($"Quantity delta > currentQuantity for OrderId={OrderId}");

            CurrentQuantity -= quantityDelta;
        }
    }

    public class LimitOrder : Order
    {
        public LimitOrder(Guid userId, uint initialQuantity, double price, OrderType type) : base(userId, initialQuantity, type)
        {
            Price = price;
        }

        public double Price { get; private set; }
    }

    public class MarketOrder : Order
    {
        public MarketOrder(Guid userId, uint initialQuantity, OrderType type) : base(userId, initialQuantity, type) { }
    }
}
