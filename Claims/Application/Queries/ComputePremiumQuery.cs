using Claims.Application.Common;
using Claims.Application.Services;
using Claims.Domain;

namespace Claims.Application.Queries
{
    public class ComputePremiumQuery
    {
        private readonly ComputePremiumService _service;

        public ComputePremiumQuery(ComputePremiumService service)
        {
            _service = service;
        }

        /// <summary>
        /// Computes the premium for explicitly given start and end dates, and cover type
        /// </summary>
        public Result<decimal> Execute(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            Result periodResult = CoverPeriod.Validate(startDate, endDate);

            if (!periodResult.IsSuccess)
            {
                return Result<decimal>.Failure(periodResult.Error);
            }

            decimal premium = _service.ComputePremium(startDate, endDate, coverType);

            return Result<decimal>.Success(premium);
        }
    }
}
