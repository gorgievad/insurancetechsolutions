using Claims.Domain;
using Claims.Domain.Common;
using Xunit;

namespace Claims.Tests.Domain
{
    public class CoverTests
    {
        [Fact]
        public void CreateSucceedsForValidCover()
        {
            // Arrange
            DateTime startDate = DateTime.UtcNow.Date;
            DateTime endDate = startDate.AddDays(30);

            // Act
            Result<Cover> result = Cover.Create(startDate, endDate, CoverType.Yacht);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(startDate, result.Value.StartDate);
            Assert.Equal(endDate, result.Value.EndDate);
            Assert.Equal(CoverType.Yacht, result.Value.Type);
        }

        [Fact]
        public void CreateFailsWhenStartDateIsInThePast()
        {
            // Arrange
            DateTime startDate = DateTime.UtcNow.Date.AddDays(-1);
            DateTime endDate = startDate.AddDays(30);

            // Act
            Result<Cover> result = Cover.Create(startDate, endDate, CoverType.Yacht);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ResultErrorCodes.StartDateCannotBeInPast, result.Error);
        }

        [Fact]
        public void CreateFailsWhenEndDateEqualsStartDate()
        {
            // Arrange
            DateTime startDate = DateTime.UtcNow.Date;

            // Act
            Result<Cover> result = Cover.Create(startDate, startDate, CoverType.Yacht);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ResultErrorCodes.StartDateCannotBeAfterEndDate, result.Error);
        }

        [Fact]
        public void CreateFailsWhenEndDateIsBeforeStartDate()
        {
            // Arrange
            DateTime startDate = DateTime.UtcNow.Date.AddDays(10);
            DateTime endDate = startDate.AddDays(-1);

            // Act
            Result<Cover> result = Cover.Create(startDate, endDate, CoverType.Yacht);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ResultErrorCodes.StartDateCannotBeAfterEndDate, result.Error);
        }

        [Fact]
        public void CreateFailsWhenPeriodExceedsOneCalendarYear()
        {
            // Arrange
            DateTime startDate = DateTime.UtcNow.Date;
            DateTime endDate = startDate.AddYears(1).AddDays(1);

            // Act
            Result<Cover> result = Cover.Create(startDate, endDate, CoverType.Yacht);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ResultErrorCodes.InsurancePeriodCannotExceedOneYear, result.Error);
        }

        [Fact]
        public void CreateSucceedsForExactlyOneCalendarYear()
        {
            // Arrange
            DateTime startDate = DateTime.UtcNow.Date;
            DateTime endDate = startDate.AddYears(1);

            // Act
            Result<Cover> result = Cover.Create(startDate, endDate, CoverType.Yacht);

            // Assert
            Assert.True(result.IsSuccess);
        }
    }
}
