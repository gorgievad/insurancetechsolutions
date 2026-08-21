using Claims.Domain;

namespace Claims.Application.Services
{
    public class ComputePremiumService
    {
        public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            const decimal baseDayRate = 1250;

            decimal multiplier = 1.3m;
            if (coverType == CoverType.Yacht)
            {
                multiplier = 1.10m;
            }
            else if (coverType == CoverType.PassengerShip)
            {
                multiplier = 1.20m;
            }
            else if (coverType == CoverType.Tanker)
            {
                multiplier = 1.50m;
            }

            decimal premiumPerDay = baseDayRate * multiplier;

            // use whole days only
            int totalDays = (int)(endDate - startDate).TotalDays;
            if (totalDays <= 0) return 0m;

            // separated: first 30 days, next up to 150 days (31-180), remaining after 180
            int daysFirst = Math.Min(totalDays, 30);
            int daysSecond = Math.Min(Math.Max(totalDays - 30, 0), 150);
            int daysThird = Math.Max(totalDays - 180, 0);

            // discounts: second segment 5% for Yacht else 2%
            // third segment adds 3% for Yacht (total 8%), adds 1% for others (total 3%)
            decimal secondDiscount = coverType == CoverType.Yacht ? 0.05m : 0.02m;
            decimal thirdDiscount = coverType == CoverType.Yacht ? 0.08m : 0.03m;

            decimal totalPremium = 0m;
            totalPremium += daysFirst * premiumPerDay;
            totalPremium += daysSecond * premiumPerDay * (1 - secondDiscount);
            totalPremium += daysThird * premiumPerDay * (1 - thirdDiscount);

            return totalPremium;
        }
    }
}
