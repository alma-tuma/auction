using Auction.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Auction.Infrastructure.EF.Configurations
{
    internal class TransactionConfigurations : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.TransactionType).IsRequired();
            builder.Property(x => x.Amount).IsRequired();

            builder.HasOne(x => x.User).WithMany(x => x.Transactions).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Auction).WithMany(x => x.Transactions).HasForeignKey(x => x.AuctionId).HasPrincipalKey(x => x.Id).IsRequired(false).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
