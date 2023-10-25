using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Auction.Infrastructure.EF.Configurations
{
    internal class AuctionConfigurations : IEntityTypeConfiguration<Entity.Auction>
    {
        public void Configure(EntityTypeBuilder<Entity.Auction> builder)
        {
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.ProductName).HasMaxLength(Entity.Auction.ProductNameMaxLen).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(Entity.Auction.DescriptionMaxLen).IsRequired();
            builder.Property(x => x.StartingBit).IsRequired();
            builder.Property(x => x.EndDate).IsRequired();
            builder.Property(x => x.UserId);
            builder.Property(x => x.IsActive).IsRequired();

            builder.HasOne(x => x.User).WithMany(x => x.Auctions).HasForeignKey(x => x.UserId).HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Restrict);

            //gloabal query filter
            builder.HasQueryFilter(x => x.IsActive);
        }
    }
}