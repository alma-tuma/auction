using Auction.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auction.Infrastructure.EF.Configurations
{
    internal class AuctionBidConfigurations : IEntityTypeConfiguration<AuctionBid>
    {
        public void Configure(EntityTypeBuilder<AuctionBid> builder)
        {
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.AuctionId).IsRequired();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.Amount).IsRequired();

            builder.HasOne(x => x.Auction).WithMany(x => x.AuctionBids).HasForeignKey(x => x.AuctionId).HasPrincipalKey(x=>x.Id).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
