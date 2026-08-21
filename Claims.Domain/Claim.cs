using Claims.Domain.Common;
using MongoDB.Bson.Serialization.Attributes;

namespace Claims.Domain
{
    public class Claim
    {
        [BsonId]
        public string Id { get; private set; } = null!;

        [BsonElement("coverId")]
        public string CoverId { get; private set; } = null!;

        [BsonElement("created")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Created { get; private set; }

        [BsonElement("name")]
        public string Name { get; private set; } = null!;

        [BsonElement("claimType")]
        public ClaimType Type { get; private set; }

        [BsonElement("damageCost")]
        public decimal DamageCost { get; private set; }

        private Claim() { }

        /// <summary>
        /// Create a new Claim instance if domain validations pass
        /// </summary>
        public static Result<Claim> Create(string coverId, DateTime created, string name, ClaimType type, decimal damageCost, Cover? relatedCover)
        {
            if (damageCost < 0m)
            {
                return Result<Claim>.Failure(ResultErrorCodes.DamageCostCannotBeNegative);
            }

            if (damageCost > 100_000m)
            {
                return Result<Claim>.Failure(ResultErrorCodes.DamageCostCannotExceedLimit);
            }

            if (relatedCover is null)
            {
                return Result<Claim>.Failure(ResultErrorCodes.CoverForClaimNotFound);
            }

            if (created < relatedCover.StartDate || created > relatedCover.EndDate)
            {
                return Result<Claim>.Failure(ResultErrorCodes.ClaimDateOutsideCoverPeriod);
            }

            Claim claim = new Claim
            {
                Id = Guid.NewGuid().ToString(),
                CoverId = coverId,
                Created = created,
                Name = name,
                Type = type,
                DamageCost = damageCost
            };

            return Result<Claim>.Success(claim);
        }
    }
}
