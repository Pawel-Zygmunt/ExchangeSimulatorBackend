using MatchingEngineApp.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchingEngineApp
{
   

    public interface ITradeListener
    {
        void OnRemovePriceLevelSide(double price, bool isBidSide);
        void OnPriceLevelSideChange(double price, bool isBidSide, IEnumerable<PriceLevelSideOrder> orders);
        void OnCurrentPriceChange(double currentPrice);

        void OnAccept(Order order);
        void OnTrade(Guid incomingOrderUserId, Guid incomingOrderId, Guid restingOrderUserId, Guid restingOrderId, double matchPrice, uint matchQuantity);
        void OnCancel(Guid orderUserId, Guid orderId, OrderCancelReason orderCancelReason);

        IEnumerable<PriceLevelSideOrder> MapLimitOrders(IEnumerable<LimitOrder> limitOrders) 
            => limitOrders.Select(lo => new PriceLevelSideOrder() { UserId = lo.UserId, Quantity = lo.CurrentQuantity });
    }
}
