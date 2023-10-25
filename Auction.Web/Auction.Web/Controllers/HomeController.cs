using Auction.Entity;
using Auction.Entity.Enumerations;
using Auction.Infrastructure.EF;
using Auction.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Security.Claims;

namespace Auction.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            await CheckAndCloseAuctions();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var dbAuctions = await _context.Auctions.Include(x => x.User)
                .Include(x => x.AuctionBids).ToListAsync();

            var auctions = new List<AuctionModel>();

            if (dbAuctions != null && dbAuctions.Any())
            {
                auctions = dbAuctions
               .Select(x => new AuctionModel
               {
                   Id = x.Id,
                   ProducName = x.ProductName,
                   EndTime = x.EndDate,
                   Seller = x.User.UserName,
                   IsMyAuction = x.UserId == userId,
                   TopBid = !x.AuctionBids.Any() ? 0 : x.AuctionBids.Max(p => p.Amount)
               }).OrderBy(x => x.EndTime).ToList();
            }

            var balance = _context.Transactions.Where(x => x.UserId == userId).
                Sum(x => (x.TransactionType == TransactionTypeEnum.Deposit ? x.Amount : -1 * x.Amount));

            var model = new AuctionIndexModel
            {
                Auctions = auctions,
                Balance = balance
            };

            return View(model);
        }

        public async Task CheckAndCloseAuctions()
        {
            var today = DateTime.Now.Date;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var myExpiredAuctions = await _context.Auctions.Include(x => x.AuctionBids)
                                        .Where(x => x.EndDate < today).ToListAsync();

            var transactions = new List<Transaction>();
            foreach (var auction in myExpiredAuctions)
            {
                auction.IsActive = false;

                if (auction.AuctionBids.Any())
                {
                    var topBid = auction.AuctionBids.Max(x => x.Amount);
                    var transaction = new Transaction
                    {
                        UserId = auction.UserId,
                        TransactionType = TransactionTypeEnum.Deposit,
                        Amount = topBid,
                        AuctionId= auction.Id
                    };

                    transactions.Add(transaction);
                }
            }

            if (transactions.Any())
            {
                _context.Transactions.AddRange(transactions);
            }

            await _context.SaveChangesAsync();
        }
    }
}