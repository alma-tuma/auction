using System.ComponentModel.DataAnnotations;

namespace Auction.Web.Models
{
    public class GreaterThanZero : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            decimal i;
            return value != null && (decimal)value > 0;
        }
    }

    public class GreaterThanToday : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var today = DateTime.Now;
            DateTime date;
            return value != null && DateTime.TryParse(value.ToString(), out date) && date > today;
        }
    }
}
