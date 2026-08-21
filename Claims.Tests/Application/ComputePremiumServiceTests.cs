using Claims.Application.Services;
using Claims.Domain;
using Xunit;

namespace Claims.Tests.Application
{
    public class ComputePremiumServiceTests
    {
        private static readonly DateTime StartDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private readonly ComputePremiumService _service = new ComputePremiumService();

        [Fact]
        public void YachtFor30Days_UsesOnlyTheFirstTier()
        {
            // Arrange
            const decimal expected = 30 * 1375m;

            // Act
            decimal premium = _service.ComputePremium(StartDate, StartDate.AddDays(30), CoverType.Yacht);

            // Assert
            Assert.Equal(expected, premium);
        }

        [Fact]
        public void YachtFor31Days_StartsTheSecondTierOnDay31()
        {
            // Arrange
            const decimal expected = (30 * 1375m) + (1 * 1375m * 0.95m);

            // Act
            decimal premium = _service.ComputePremium(StartDate, StartDate.AddDays(31), CoverType.Yacht);

            // Assert
            Assert.Equal(expected, premium);
        }

        [Fact]
        public void YachtFor181Days_StartsTheThirdTierOnDay181()
        {
            // Arrange
            const decimal expected = (30 * 1375m) + (150 * 1375m * 0.95m) + (1 * 1375m * 0.92m);

            // Act
            decimal premium = _service.ComputePremium(StartDate, StartDate.AddDays(181), CoverType.Yacht);

            // Assert
            Assert.Equal(expected, premium);
        }

        [Fact]
        public void TankerFor181Days_UsesItsMultiplierAndTheNonYachtDiscounts()
        {
            // Arrange
            const decimal expected = (30 * 1875m) + (150 * 1875m * 0.98m) + (1 * 1875m * 0.97m);

            // Act
            decimal premium = _service.ComputePremium(StartDate, StartDate.AddDays(181), CoverType.Tanker);

            // Assert
            Assert.Equal(expected, premium);
        }

        [Fact]
        public void PassengerShipFor30Days_UsesItsMultiplier()
        {
            // Arrange
            const decimal expected = 30 * 1500m;

            // Act
            decimal premium = _service.ComputePremium(StartDate, StartDate.AddDays(30), CoverType.PassengerShip);

            // Assert
            Assert.Equal(expected, premium);
        }

        [Fact]
        public void ContainerShipFor30Days_UsesTheDefaultMultiplier()
        {
            // Arrange
            const decimal expected = 30 * 1625m;

            // Act
            decimal premium = _service.ComputePremium(StartDate, StartDate.AddDays(30), CoverType.ContainerShip);

            // Assert
            Assert.Equal(expected, premium);
        }
    }
}
