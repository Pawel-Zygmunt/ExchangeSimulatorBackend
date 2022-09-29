using ExchangeSimulatorBackend.Entities;
using ExchangeSimulatorBackend.HubConfig;
using MatchingEngineApp;
using MatchingEngineApp.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace ExchangeSimulatorBackend.Services
{
    public class MatchingEngineService
    {
        private MatchingEngine MatchingEngine { get; set; }

        public void AddOrder(Order order) => MatchingEngine.AddOrder(order);

        public Dictionary<double, OrderBookLevel> GetOrderBook() => MatchingEngine.GetOrderBook();

        public MatchingEngineService(IServiceProvider services)
        {            
            MatchingEngine = new MatchingEngine(new MatchingEngineListener(services));
        }

        private class MatchingEngineListener : ITradeListener
        {
            private readonly IServiceProvider _services;
            private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

            public MatchingEngineListener(IServiceProvider services)
            {
                _services = services;
            }

            private async void LockedScopeProvider(Func<AppDbContext, IUserContextService,
                                                   IHubContext<MatchingEngineHub>, Task> engineAction)
            {
                await _semaphore.WaitAsync();
                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var userContextService = scope.ServiceProvider.GetRequiredService<IUserContextService>();
                        var matchingEngineHub = scope.ServiceProvider.GetRequiredService<IHubContext<MatchingEngineHub>>();
                        await engineAction(dbContext, userContextService, matchingEngineHub);
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            public void OnAccept(Order order)
            {
                LockedScopeProvider(async (dbContext, userContextService, matchingEngineHub) =>
                {
                    if(order is LimitOrder)
                    {
                        var message = $"OnAccept - Zlecenie z limitem typu {order.Type}, ilość: {order.InitialQuantity}, cena: {(order as LimitOrder)!.Price} zaakceptowane";
                        await matchingEngineHub.Clients.User(order.UserId.ToString()).SendAsync("onOrderAccepted", message);
                    }
                });
            }

            public void OnCancel(Guid orderUserId, Guid orderId, OrderCancelReason orderCancelReason)
            {
                LockedScopeProvider(async (dbContext, userContextService, matchingEngineHub) =>
                {
                    await matchingEngineHub.Clients.User(orderUserId.ToString()).SendAsync("onCancel", orderCancelReason.ToString());
                    Console.Write($"OnCancel");
                });
            }

            public void OnPriceLevelSideChange(double price, bool isBidSide, IEnumerable<PriceLevelSideOrder> orders)
            {
                LockedScopeProvider(async (dbContext, userContextService, matchingEngineHub) =>
                {
                    await matchingEngineHub.Clients.All.SendAsync("onPriceLevelSideChange", price, isBidSide, orders, orders.Sum(o=>o.Quantity));
                    Console.Write($"OnChangePriceLevelSide");
                });
            }

            public void OnCurrentPriceChange(double currentPrice)
            {
                LockedScopeProvider(async (dbContext, userContextService, matchingEngineHub) =>
                {
                    await matchingEngineHub.Clients.All.SendAsync("onCurrentPriceChange", currentPrice);
                    Console.Write($"OnCurrentPriceChange");
                });
            }

            public void OnRemovePriceLevelSide(double price, bool isBidSide)
            {
                LockedScopeProvider(async (dbContext, userContextService, matchingEngineHub) =>
                {
                    await matchingEngineHub.Clients.All.SendAsync("onRemovePriceLevel", price, isBidSide);
                    Console.Write($"OnRemovePriceLevelSide");
                });
            }

            public void OnTrade(Guid incomingOrderUserId, Guid incomingOrderId, Guid restingOrderUserId, Guid restingOrderId, double matchPrice, uint matchQuantity)
            {
                LockedScopeProvider(async (dbContext, userContextService, matchingEngineHub) =>
                {
                    Console.Write($"OnTrade");
                });
            }
        }
    }
}
