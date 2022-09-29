
using MatchingEngineApp;
using MatchingEngineApp.Dtos;

class MyTradeListener : ITradeListener
{
    public void OnAccept(Order order)
    {
        Console.WriteLine("OnAccept");
    }

    public void OnCancel(Guid orderUserId, Guid orderId,
                         OrderCancelReason orderCancelReason)
    {
        Console.WriteLine("OnCancel");
    }

    public void OnPriceLevelSideChange(double Price, bool isAskSide,
                                        IEnumerable<PriceLevelSideOrder> orders)
    {
        Console.WriteLine("OnChangePriceLevelSide");
    }

    public void OnCurrentPriceChange(double currentPrice)
    {
        Console.WriteLine("OnCurrentPriceChanged");
    }

    public void OnRemovePriceLevelSide(double Price, bool isAskSide)
    {
        Console.WriteLine("OnRemovePriceLevelSide");
    }

    public void OnTrade(Guid incomingOrderUserId, Guid incomingOrderId,
                        Guid restingOrderUserId, Guid restingOrderId,
                        double matchPrice, uint matchQuantity)
    {
        Console.WriteLine("On Trade");
    }
}



class Program
{
    static void Main(string[] args)
    {
        //1
        MatchingEngine matchingEngine = new MatchingEngine(new MyTradeListener());

        //2
        matchingEngine.AddOrder(new LimitOrder(type: OrderType.SELL,
                                               userId: Guid.NewGuid(),
                                               price: 10.00, initialQuantity: 10));

        //3
        matchingEngine.AddOrder(new LimitOrder(type: OrderType.BUY, userId: Guid.NewGuid(),
                                               price: 15.00, initialQuantity: 5));

        //4
        Console.WriteLine($"marketPrice: {matchingEngine.MarketPrice}");


        //5
        matchingEngine.AddOrder(new MarketOrder(type: OrderType.BUY, 
                                                userId: Guid.NewGuid(),
                                                initialQuantity: 100));

        
        //6
        matchingEngine.AddOrder(new MarketOrder(type: OrderType.SELL,
                                                userId: Guid.NewGuid(),
                                                initialQuantity: 20));
    }
}