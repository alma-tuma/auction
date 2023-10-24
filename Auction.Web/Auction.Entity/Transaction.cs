using Auction.Entity.Enumerations;

namespace Auction.Entity
{
    public class Transaction
    {
        public int Id { get; set; }
        public TransactionTypeEnum TransactionType { get; set; }
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public int? AuctionId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Auction Auction { get; set; }
    }
}
