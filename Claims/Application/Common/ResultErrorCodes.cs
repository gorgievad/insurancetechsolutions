namespace Claims.Application.Common
{
    public static class ResultErrorCodes
    {
        public const string StartDateCannotBeAfterEndDate =
            "START_DATE_CANNOT_BE_AFTER_END_DATE";

        public const string StartDateCannotBeInPast =
            "START_DATE_CANNOT_BE_IN_PAST";

        public const string InsurancePeriodCannotExceedOneYear =
            "INSURANCE_PERIOD_CANNOT_EXCEED_ONE_YEAR";

        public const string DamageCostCannotBeNegative =
            "DAMAGE_COST_CANNOT_BE_NEGATIVE";

        public const string DamageCostCannotExceedLimit =
            "DAMAGE_COST_CANNOT_EXCEED_LIMIT";

        public const string ClaimDateOutsideCoverPeriod =
            "CLAIM_DATE_OUTSIDE_COVER_PERIOD";

        public const string CoverIdRequired =
            "COVER_ID_REQUIRED";

        public const string CoverNotFound =
            "COVER_NOT_FOUND";

        public const string CoverForClaimNotFound =
            "COVER_FOR_CLAIM_NOT_FOUND";

        public const string ClaimNotFound =
            "CLAIM_NOT_FOUND";

        public const string CoverHasClaims =
            "COVER_HAS_CLAIMS";

        /// <summary>
        /// Fallback for failures that carry a plain validation message instead of one of the codes above.
        /// </summary>
        public const string ValidationFailed =
            "VALIDATION_FAILED";
    }
}
