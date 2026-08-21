using Claims.Domain.Common;

namespace Claims.Domain
{
    /// <summary>
    /// Rules for an insurance period
    /// </summary>
    public static class CoverPeriod
    {
        public static Result Validate(DateTime startDate, DateTime endDate)
        {
            DateTime today = DateTime.UtcNow.Date;

            if (startDate.Date < today)
            {
                return Result.Failure(ResultErrorCodes.StartDateCannotBeInPast);
            }

            if (endDate <= startDate)
            {
                return Result.Failure(ResultErrorCodes.StartDateCannotBeAfterEndDate);
            }

            if (endDate > startDate.AddYears(1))
            {
                return Result.Failure(ResultErrorCodes.InsurancePeriodCannotExceedOneYear);
            }

            return Result.Success();
        }
    }
}
