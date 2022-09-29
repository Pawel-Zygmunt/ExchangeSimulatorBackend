using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchingEngineApp.Dtos
{
    public class OrderBookLevel
    {
        public OrderBookSide BidSide { get; set; }
        public OrderBookSide AskSide { get; set; }
    }

    public class OrderBookSide
    {
        public int Quantity { get; set; }
        public IEnumerable<PriceLevelSideOrder> Orders { get; set; } = new List<PriceLevelSideOrder>();
    }

    public class PriceLevelSideOrder
    {
        public Guid UserId { get; set; }
        public uint Quantity { get; set; }
    }

    public class OrderBookDto
    {
        public Dictionary<double, OrderBookLevel> Levels { get; set; }
    }
}
