using System.ComponentModel.DataAnnotations;

namespace Auction.Entity
{
    public class Auction
    {
        public const int ProductNameMaxLen = 50;
        public const int ProductNameMinLen = 4;

        public const int DescriptionMaxLen = 500;
        public const int DescriptionMinLen = 11;

        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal StartingBit { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string UserId { get; set; }

        public ICollection<AuctionBid> AuctionBids { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
