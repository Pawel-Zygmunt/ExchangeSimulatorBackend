using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExchangeSimulatorBackend.Entities
{
    public class AppUser : IdentityUser<Guid>, IEntityTypeConfiguration<AppUser>
    {
        public double AccountBalance { get; set; }
        public uint OwnedStocksAmount { get; set; }
        public List<Transaction> Transactions { get; set; }

        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(p => p.AccountBalance).HasDefaultValue(1000);

            builder
                .HasMany(p => p.Transactions)
                .WithOne(p => p.AppUser)
                .HasForeignKey(p => p.UserId)
                .IsRequired();
        }
    }
}
