using System.ComponentModel.DataAnnotations;

namespace Auction.Web.Models
{
    public class AuctionIndexModel
    {
        public List<AuctionModel> Auctions { get; set; }
        public decimal Balance { get; set; }
        public string BalanceString => Balance.ToString("###,###.00");
    }

    public class AuctionModel
    {
        public int Id { get; set; }
        public string ProducName { get; set; }
        public bool IsMyAuction { get; set; }
        public string Seller { get; set; }
        public DateTime EndTime { get; set; }
        public int TimeRemaining => (EndTime - DateTime.Now.Date).Days;
        public decimal TopBid { get; set; }
    }

    public class AuctionAddModel
    {
        [Required]
        [MinLength(4)]
        public string ProductName { get; set; }
        [Required]
        [MinLength(11)]
        public string Description { get; set; }
        [GreaterThanZero(ErrorMessage ="StartingBid must be grater then zero")]
        public decimal StartingBid { get; set; }
        [GreaterThanToday(ErrorMessage = "EndDate must be some time in the future")]
        public DateTime EndDate { get; set; }
    }

    public class AuctionDetailsModel
    {
        public int Id { get; set; }
        public string ProducName { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime EndTime { get; set; }
        public int TimeRemaining => (EndTime - DateTime.Now).Days;
        public decimal TopBid { get; set; }
        public string TopBidCreatedBy { get; set; }
    }

    public class AuctionBidAddModel
    {
        public int AuctionId { get; set; }
        public decimal Amount { get; set; }
    }
}
