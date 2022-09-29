using MatchingEngineApp.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchingEngineApp
{
    internal class Book
    {
        private readonly SortedDictionary<double, PriceLevel> _bidSide;
        private readonly SortedDictionary<double, PriceLevel> _askSide;

        public PriceLevel? _bestBidPriceLevel { get; private set; }
        public PriceLevel? _bestAskPriceLevel { get; private set; }

        private readonly ITradeListener _tradeListener;

        public Book(ITradeListener tradeListener)
        {
            _tradeListener = tradeListener;
            var priceComparerAscending = new PriceComparerAscending();
            var priceComparerDescending = new PriceComparerDescending();

            _bidSide = new SortedDictionary<double, PriceLevel>(priceComparerDescending);
            _askSide = new SortedDictionary<double, PriceLevel>(priceComparerAscending);

            _bestBidPriceLevel = null;
            _bestAskPriceLevel = null;
        }

        public PriceLevel GetPriceLevel(double price, bool isBidSide)
        {
            var side = isBidSide ? _bidSide : _askSide;

            if (side.TryGetValue(price, out var level))
            {
                return level;
            }

            throw new InvalidOperationException("GetPriceLevel - no level found");
        }

        public Dictionary<double, OrderBookLevel> GetAllPriceLevels()
        {
            var keys = new HashSet<double>(_bidSide.Keys);
            keys.UnionWith(_askSide.Keys);

            return keys.Select((k) =>
            {
                var orderBookBidSide = new OrderBookSide();
                var orderBookAskSide = new OrderBookSide();

                if (_bidSide.TryGetValue(k, out var level))
                {
                    var sideOrders = _tradeListener.MapLimitOrders(level.GetOrders());
                    orderBookBidSide.Orders = sideOrders;
                    orderBookBidSide.Quantity = (int)sideOrders.Sum(o => o.Quantity);
                }

                if (_askSide.TryGetValue(k, out var asklevel))
                {
                    var sideOrders = _tradeListener.MapLimitOrders(asklevel.GetOrders());
                    orderBookAskSide.Orders = sideOrders;
                    orderBookAskSide.Quantity = (int)sideOrders.Sum(o => o.Quantity);
                }

                var orderBookLevel = new OrderBookLevel()
                {
                    AskSide = orderBookAskSide,
                    BidSide = orderBookBidSide,
                };

                return new KeyValuePair<double, OrderBookLevel>(k, orderBookLevel);
            }).ToDictionary(keySelector: k => k.Key, elementSelector: k => k.Value);   
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
                _tradeListener.OnPriceLevelSideChange(order.Price, true, _tradeListener.MapLimitOrders(priceLevel.GetOrders()));

                if (_bestBidPriceLevel == null || order.Price > _bestBidPriceLevel.Price)
                {
                    _bestBidPriceLevel = priceLevel;
                }
            }
            else
            {
                PriceLevel priceLevel = GetOrAddPriceLevel(order.Price, _askSide);
                priceLevel.AddOrder(order);
                _tradeListener.OnPriceLevelSideChange(order.Price, false, _tradeListener.MapLimitOrders(priceLevel.GetOrders()));

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
               
                if (_RemovePriceLevelIfEmpty(priceLevel, side, order.Type))
                {
                    _tradeListener.OnRemovePriceLevelSide(priceLevel.Price, order.Type == OrderType.BUY);
                    return;
                }

                _tradeListener.OnPriceLevelSideChange(order.Price, order.Type == OrderType.BUY, _tradeListener.MapLimitOrders(priceLevel.GetOrders()));
            }
        }

        private bool _RemovePriceLevelIfEmpty(PriceLevel priceLevel, SortedDictionary<double, PriceLevel> side, OrderType orderType)
        {
            if (priceLevel.OrderCount != 0) return false;

            var removed = side.Remove(priceLevel.Price);
                
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

            return removed;
        }
    }
}
