using ExchangeSimulatorBackend.Entities;
using ExchangeSimulatorBackend.HubConfig;
using MatchingEngineApp;
using Microsoft.AspNetCore.SignalR;

namespace ExchangeSimulatorBackend.Services
{
    public class MatchingEngineService
    {
        private MatchingEngine MatchingEngine { get; set; }

        public void AddOrder(Order order) => MatchingEngine.AddOrder(order);

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

            private async void LockedScopeProvider(Func<AppDbContext, IUserContextService, IHubContext<MatchingEngineHub>, Task> engineAction)
            {
                await _semaphore.WaitAsync();
                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var userContextService = scope.ServiceProvider.GetRequiredService<IUserContextService>();
                        var matchingEngineHubContext = scope.ServiceProvider.GetRequiredService<IHubContext<MatchingEngineHub>>();
                        await engineAction(dbContext, userContextService, matchingEngineHubContext);
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            public void OnAccept(Guid orderId)
            {
                LockedScopeProvider(async (dbContext, userContextService, matchingEngineHub) =>
                {
                    Console.Write($"OnAccept");
                });
            }

            public void OnCancel(Guid orderUserId, Guid orderId, OrderCancelReason orderCancelReason)
            {
                LockedScopeProvider(async (dbContext, userContextService, matchingEngineHub) =>
                {
                    Console.Write($"OnCancel");
                });
            }

            public void OnChangePriceLevelSide(double price, bool isBidSide, IEnumerable<PriceLevelSideOrder> orders)
            {
                LockedScopeProvider(async (dbContext, userContextService, matchingEngineHub) =>
                {
                    Console.Write($"OnChangePriceLevelSide");
                });
            }

            public void OnCurrentPriceChanged(double currentPrice)
            {
                LockedScopeProvider(async (dbContext, userContextService, matchingEngineHub) =>
                {
                    Console.Write($"CurrentPriceChanged: ${currentPrice}");
                });
            }

            public void OnRemovePriceLevelSide(double price, bool isBidSide)
            {
                LockedScopeProvider(async (dbContext, userContextService, matchingEngineHub) =>
                {
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
