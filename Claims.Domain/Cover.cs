using Claims.Domain.Common;
using MongoDB.Bson.Serialization.Attributes;

namespace Claims.Domain;

public class Cover
{
    [BsonId]
    public string Id { get; private set; } = null!;

    [BsonElement("startDate")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime StartDate { get; private set; }

    [BsonElement("endDate")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime EndDate { get; private set; }

    [BsonElement("claimType")]
    public CoverType Type { get; private set; }

    [BsonElement("premium")]
    public decimal Premium { get; private set; }

    private Cover() { }

    /// <summary>
    /// Assigns the premium computed for this cover's period and type.
    /// </summary>
    public void SetPremium(decimal premium)
    {
        Premium = premium;
    }

    /// <summary>
    /// Create a Cover if domain validations pass
    /// </summary>
    public static Result<Cover> Create(DateTime startDate, DateTime endDate, CoverType type)
    {
        Result periodResult = CoverPeriod.Validate(startDate, endDate);

        if (!periodResult.IsSuccess)
        {
            return Result<Cover>.Failure(periodResult.Error);
        }

        Cover cover = new Cover
        {
            Id = Guid.NewGuid().ToString(),
            StartDate = startDate,
            EndDate = endDate,
            Type = type
        };

        return Result<Cover>.Success(cover);
    }
}
