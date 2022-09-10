

using ExchangeSimulatorBackend.Entities;

namespace ExchangeSimulatorBackend.Dtos
{
    public class UserDto
    {
        public string Email { get; set; }
        public double AccountBalance { get; set; }
        public uint OwnedStocksAmount { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}
