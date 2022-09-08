
using MatchingEngineApp;

class MyTradeListener : ITradeListener
{
    //public void OnAccept(Guid orderId)
    //{
    //    Console.WriteLine($"Order {orderId} accepted");
    //}

    //public void OnAddLimitOrderToBook(Guid orderId, double price, uint amout)
    //{
    //    Console.WriteLine($"LimitOrder {orderId} added to order book at price: {price}, amount: {amout}");
    //}

    //public void OnCancel(Guid orderId, OrderCancelReason orderCancelReason)
    //{
    //    Console.WriteLine($"Order {orderId} cancelled, reason: ${orderCancelReason}");
    //}

    //public void OnTrade(Guid incomingOrderId, Guid restingOrderId, double matchPrice, uint matchQuantity)
    //{
    //    Console.WriteLine($"Order matched.... incomingOrderId: {incomingOrderId}, restingOrderId: {restingOrderId}, executedQuantity: {matchQuantity}, Price: {matchPrice}");
    //}
    public void OnAccept(Guid orderId)
    {
        Console.WriteLine("OnAccept");
    }

    public void OnCancel(Guid orderUserId, Guid orderId, OrderCancelReason orderCancelReason)
    {
        Console.WriteLine("OnCancel");
    }

    public void OnChangePriceLevelSide(double Price, bool isAskSide, IEnumerable<PriceLevelSideOrder> orders)
    {
        Console.WriteLine("OnChangePriceLevelSide");
    }

    public void OnCurrentPriceChanged(double currentPrice)
    {
        Console.WriteLine("OnCurrentPriceChanged");
    }

    public void OnRemovePriceLevelSide(double Price, bool isAskSide)
    {
        Console.WriteLine("OnRemovePriceLevelSide");
    }

    public void OnTrade(Guid incomingOrderUserId, Guid incomingOrderId, Guid restingOrderUserId, Guid restingOrderId, double matchPrice, uint matchQuantity)
    {
        Console.WriteLine("On Trade");
    }
}



class Program
{
    static void Main(string[] args)
    {
        MatchingEngine matchingEngine = new MatchingEngine(new MyTradeListener());

        matchingEngine.AddOrder(new LimitOrder(type: OrderType.SELL, userId: Guid.NewGuid(), price: 10.00, initialQuantity: 10));

        matchingEngine.AddOrder(new LimitOrder(type: OrderType.BUY, userId: Guid.NewGuid(), price: 15.00, initialQuantity: 5));

        //Console.WriteLine($"marketPrice: {matchingEngine.MarketPrice}");

        //matchingEngine.AddOrder(new MarketOrder(type: OrderType.BUY, userId: Guid.NewGuid(), initialQuantity: 100));

        //matchingEngine.AddOrder(new MarketOrder(type: OrderType.SELL, userId: Guid.NewGuid(), initialQuantity: 20));
    }
}