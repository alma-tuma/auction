using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace Auction.Infrastructure.EF
{
    public class MigrationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
    {
        public ApplicationContext CreateDbContext(string[] args)
        {

            var connString = "Server=DESKTOP-R3D3QR1;Database=AuctionDB;Trusted_Connection=True;TrustServerCertificate=True";
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            optionsBuilder.UseSqlServer(connString);

            return new ApplicationContext(optionsBuilder.Options);
        }
    }
}
