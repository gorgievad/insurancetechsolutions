using Claims.Application.Common;
using Claims.Domain;
using Xunit;

namespace Claims.Tests.Domain
{
    public class ClaimTests
    {
        private static readonly DateTime CoverStart = DateTime.UtcNow.Date;
        private static readonly DateTime CoverEnd = CoverStart.AddDays(30);

        private static Cover ValidCover()
        {
            Result<Cover> result = Cover.Create(CoverStart, CoverEnd, CoverType.Yacht);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);

            return result.Value;
        }

        [Fact]
        public void CreateSucceedsForValidClaimWithExistingCover()
        {
            // Arrange
            Cover cover = ValidCover();

            // Act
            Result<Claim> result = Claim.Create(coverId: cover.Id,
                                                created: CoverStart.AddDays(5),
                                                name: "Claim1",
                                                type: ClaimType.Collision,
                                                damageCost: 5_000m,
                                                relatedCover: cover);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(cover.Id, result.Value.CoverId);
            Assert.Equal(5_000m, result.Value.DamageCost);
        }

        [Fact]
        public void CreateFailsWhenDamageCostIsNegative()
        {
            // Arrange
            Cover cover = ValidCover();

            // Act
            Result<Claim> result = Claim.Create(coverId: cover.Id,
                                                created: CoverStart.AddDays(5),
                                                name: "Claim2",
                                                type: ClaimType.Collision,
                                                damageCost: -1m,
                                                relatedCover: cover);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ResultErrorCodes.DamageCostCannotBeNegative, result.Error);
        }

        [Fact]
        public void CreateFailsWhenDamageCostExceedsLimit()
        {
            // Arrange
            Cover cover = ValidCover();

            // Act
            Result<Claim> result = Claim.Create(coverId: cover.Id,
                                                created: CoverStart.AddDays(5),
                                                name: "Claim3",
                                                type: ClaimType.Collision,
                                                damageCost: 100_001m,
                                                relatedCover: cover);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ResultErrorCodes.DamageCostCannotExceedLimit, result.Error);
        }

        [Fact]
        public void CreateSucceedsWhenDamageCostIsExactlyTheLimit()
        {
            // Arrange
            Cover cover = ValidCover();

            // Act
            Result<Claim> result = Claim.Create(coverId: cover.Id,
                                                created: CoverStart.AddDays(5),
                                                name: "Claim6",
                                                type: ClaimType.Collision,
                                                damageCost: 100_000m,
                                                relatedCover: cover);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void CreateFailsWhenCreatedIsAfterTheCoverPeriod()
        {
            // Arrange
            Cover cover = ValidCover();

            // Act
            Result<Claim> result = Claim.Create(coverId: cover.Id,
                                                created: CoverEnd.AddDays(1),
                                                name: "Claim5",
                                                type: ClaimType.Collision,
                                                damageCost: 5_000m,
                                                relatedCover: cover);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ResultErrorCodes.ClaimDateOutsideCoverPeriod, result.Error);
        }

        [Fact]
        public void CreateFailsWhenCreatedIsBeforeTheCoverPeriod()
        {
            // Arrange
            Cover cover = ValidCover();

            // Act
            Result<Claim> result = Claim.Create(coverId: cover.Id,
                                                created: CoverStart.AddDays(-1),
                                                name: "Claim4",
                                                type: ClaimType.Collision,
                                                damageCost: 5_000m,
                                                relatedCover: cover);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ResultErrorCodes.ClaimDateOutsideCoverPeriod, result.Error);
        }
    }
}
