using Microsoft.AspNetCore.Identity;

namespace ExchangeSimulatorBackend.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public double AccountBalance { get; set; }
    }
}
