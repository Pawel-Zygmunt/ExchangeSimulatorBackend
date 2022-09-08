using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchingEngine
{
    public class Book
    {
        private readonly SortedDictionary<double, PriceLevel> _bidSide;
        private readonly SortedDictionary<double, PriceLevel> _askSide;

        public PriceLevel? _bestBidPriceLevel { get; private set; }
        public PriceLevel? _bestAskPriceLevel { get; private set; }


        public Book()
        {
            var priceComparerAscending = new PriceComparerAscending();
            var priceComparerDescending = new PriceComparerDescending();

            _bidSide = new SortedDictionary<double, PriceLevel>(priceComparerDescending);
            _askSide = new SortedDictionary<double, PriceLevel>(priceComparerAscending);

            _bestBidPriceLevel = null;
            _bestAskPriceLevel = null;
        }

        public LimitOrder? GetBestOrderForSide(OrderType orderType)
        {
            var bestPriceLevel = orderType == OrderType.BUY ? _bestBidPriceLevel : _bestAskPriceLevel;
            return bestPriceLevel?.FirstOrder;
        }

        public void AddOrder(LimitOrder order)
        {
            PriceLevel GetOrAddPriceLevel(double price, SortedDictionary<double, PriceLevel> side)
            {
                if (!side.TryGetValue(price, out PriceLevel? priceLevel))
                {
                    priceLevel = new PriceLevel(price);
                    side.Add(price, priceLevel);
                }
                return priceLevel;
            }

            if (order.Type == OrderType.BUY)
            {
                PriceLevel priceLevel = GetOrAddPriceLevel(order.Price, _bidSide);
                priceLevel.AddOrder(order);

                if (_bestBidPriceLevel == null || order.Price > _bestBidPriceLevel.Price)
                {
                    _bestBidPriceLevel = priceLevel;
                }
            }
            else
            {
                PriceLevel priceLevel = GetOrAddPriceLevel(order.Price, _askSide);
                priceLevel.AddOrder(order);

                if (_bestAskPriceLevel == null || order.Price < _bestAskPriceLevel.Price)
                {
                    _bestAskPriceLevel = priceLevel;
                }
            }
        }

        public void RemoveFilledOrder(LimitOrder order)
        {
            if (!order.IsTotallyFilled)
                throw new InvalidOperationException($"PriceLevelError: trying to remove order which is not filled completely for OrderId={order.OrderId}");

            var side = order.Type == OrderType.BUY ? _bidSide : _askSide;

            if (side.TryGetValue(order.Price, out PriceLevel? priceLevel))
            {
                priceLevel.RemoveOrder(order);
                _RemovePriceLevelIfEmpty(priceLevel, side, order.Type);
            }
        }

        private void _RemovePriceLevelIfEmpty(PriceLevel priceLevel, SortedDictionary<double, PriceLevel> side, OrderType orderType)
        {
            if (priceLevel.OrderCount != 0) return;

            side.Remove(priceLevel.Price);

            if (orderType == OrderType.BUY && _bestBidPriceLevel?.Price == priceLevel.Price)
            {
                _bestBidPriceLevel = null;

                if (side.Count > 0)
                {
                    var keyVal = side.FirstOrDefault();
                    _bestBidPriceLevel = keyVal.Value;
                }
            }

            if (orderType == OrderType.SELL && _bestAskPriceLevel?.Price == priceLevel.Price)
            {
                _bestAskPriceLevel = null;

                if (side.Count > 0)
                {
                    var keyVal = side.FirstOrDefault();
                    _bestAskPriceLevel = keyVal.Value;
                }
            }
        }
    }
}
