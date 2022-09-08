using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchingEngineApp
{
    public class PriceLevelSideOrder
    {
        public Guid UserId { get; set; }
        public uint Quantity { get; set; }
    }

    public interface ITradeListener
    {
        void OnRemovePriceLevelSide(double price, bool isBidSide);
        void OnChangePriceLevelSide(double price, bool isBidSide, IEnumerable<PriceLevelSideOrder> orders);
        void OnCurrentPriceChanged(double currentPrice);


        void OnAccept(Guid orderId);
        void OnTrade(Guid incomingOrderUserId, Guid incomingOrderId, Guid restingOrderUserId, Guid restingOrderId, double matchPrice, uint matchQuantity);
        void OnCancel(Guid orderUserId, Guid orderId, OrderCancelReason orderCancelReason);

        IEnumerable<PriceLevelSideOrder> MapLimitOrders(IEnumerable<LimitOrder> limitOrders) 
            => limitOrders.Select(lo => new PriceLevelSideOrder() { UserId = lo.UserId, Quantity = lo.CurrentQuantity });
    }
}
