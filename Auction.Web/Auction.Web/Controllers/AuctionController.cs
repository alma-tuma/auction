using Auction.Entity;
using Auction.Entity.Enumerations;
using Auction.Infrastructure.EF;
using Auction.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Security.Claims;

namespace Auction.Web.Controllers
{
    public class AuctionController : Controller
    {
        private readonly ApplicationContext _context;
        public AuctionController(ApplicationContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {

            return View();
        }

        public IActionResult Add() => View(new AuctionAddModel() { EndDate = DateTime.Now.AddDays(1) });

        [HttpPost]
        public async Task<IActionResult> Add(AuctionAddModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var auction = new Entity.Auction
            {
                ProductName = model.ProductName,
                Description = model.Description,
                EndDate = model.EndDate,
                IsActive = true,
                StartingBit = model.StartingBid,
                UserId = userId
            };

            _context.Auctions.Add(auction);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var auction = await _context.Auctions.Where(x => x.UserId == userId && x.Id == id).FirstOrDefaultAsync();
            _context.Auctions.Remove(auction);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Details(int id, string? errorMessage = null)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                ModelState.AddModelError("Amount", errorMessage);
            }

            var auction = await _context.Auctions.Include(x => x.AuctionBids)
                                    .ThenInclude(x => x.User)
                                    .Include(x => x.User)
                                    .Where(x => x.Id == id)
                                    .FirstOrDefaultAsync();

            var model = new AuctionDetailsModel
            {
                Id = auction.Id,
                ProducName = auction.ProductName,
                Description = auction.Description,
                EndTime = auction.EndDate,
                CreatedBy = auction.User.UserName
            };

            var topBid = auction.AuctionBids.OrderByDescending(x => x.Amount).FirstOrDefault();
            if (topBid != null)
            {
                model.TopBid = topBid.Amount;
                model.TopBidCreatedBy = topBid.User.FirstName + " " + topBid.User.LastName;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Bid(AuctionBidAddModel model)
        {
            var dbAuction = await _context.Auctions.Where(x => x.Id == model.AuctionId).FirstOrDefaultAsync();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var balance = _context.Transactions.Where(x => x.UserId == userId).
                    Sum(x => (x.TransactionType == TransactionTypeEnum.Deposit ? x.Amount : -1 * x.Amount));

            if (model.Amount < dbAuction.StartingBit)
                return RedirectToAction(nameof(Details), new { id = model.AuctionId, errorMessage = "Your bid amount must be grater than starting bid amount" });

            if (balance < model.Amount)
                return RedirectToAction(nameof(Details), new { id = model.AuctionId, errorMessage = "Your balance is lower than bid amount" });

            var topBid = await _context.AuctionBids.Where(x => x.AuctionId == model.AuctionId)
            .OrderByDescending(x => x.Amount).FirstOrDefaultAsync();

            if (topBid != null && model.Amount <= topBid.Amount)
                return RedirectToAction(nameof(Details), new { id = model.AuctionId, errorMessage = "Your must be grater than top bid amount" });

            var auctionBid = new AuctionBid
            {
                UserId = userId,
                Amount = model.Amount,
                AuctionId = model.AuctionId
            };

            _context.AuctionBids.Add(auctionBid);

            if (topBid != null)
            {
                var deposit = new Transaction
                {
                    UserId = topBid.UserId,
                    AuctionId= model.AuctionId,
                    Amount = topBid.Amount,
                    TransactionType = TransactionTypeEnum.Deposit
                };

                _context.Transactions.Add(deposit);
            }

            var withdraw = new Transaction
            {
                UserId = userId,
                Amount = model.Amount,
                AuctionId = model.AuctionId,
                TransactionType = TransactionTypeEnum.Withdraw
            };

            _context.Transactions.Add(withdraw);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
