using Auction.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Auction.Infrastructure.EF.Configurations
{
    internal class ApplicationUserConfigurations : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(x => x.FirstName).HasMaxLength(ApplicationUser.FirstNameMaxLen).IsRequired();
            builder.Property(x => x.LastName).HasMaxLength(ApplicationUser.LastNameMaxLen).IsRequired();
        }
    }
}
