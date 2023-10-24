namespace Auction.Entity
{
    public class AuctionBid
    {
        public int Id { get; set; }
        public int AuctionId { get; set; }
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public virtual Auction Auction { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
