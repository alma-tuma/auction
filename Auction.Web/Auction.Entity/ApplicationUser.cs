using Microsoft.AspNetCore.Identity;

namespace Auction.Entity
{
    public class ApplicationUser : IdentityUser
    {
        public const int FirstNameMaxLen = 50;
        public const int LastNameMaxLen = 50;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<Auction> Auctions { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
