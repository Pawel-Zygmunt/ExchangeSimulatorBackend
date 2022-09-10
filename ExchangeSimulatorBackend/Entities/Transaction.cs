namespace ExchangeSimulatorBackend.Entities
{
    public enum TransactionType
    {
        BUY,
        SELL
    }
    public class Transaction
    {
        public Guid Id { get; set; }
        public TransactionType Type  { get; set; }
        public uint Amount { get; set; }
        public double Price { get; set; }
        public DateTime Date { get; set; }
        public Guid UserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
