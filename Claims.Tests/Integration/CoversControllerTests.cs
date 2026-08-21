using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Claims.Api.Common;
using Claims.Application.DTO;
using Claims.Domain;
using Claims.Domain.Common;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Claims.Tests.Integration
{
    public class CoversControllerTests
    {
        private static readonly JsonSerializerOptions SerializerOptions = CreateSerializerOptions();

        [Fact]
        public async Task PostCover_ReturnsTheCreatedCoverWithAPremium()
        {
            // Arrange
            using WebApplicationFactory<Program> application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(_ =>
                {});

            HttpClient client = application.CreateClient();

            CreateCoverRequest request = new CreateCoverRequest
            {
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddDays(30),
                Type = CoverType.Yacht
            };

            // Act
            HttpResponseMessage response = await client.PostAsJsonAsync(
                "/Covers",
                request,
                SerializerOptions,
                TestContext.Current.CancellationToken);

            // Assert
            response.EnsureSuccessStatusCode();

            CoverDto? cover = await response.Content.ReadFromJsonAsync<CoverDto>(
                SerializerOptions,
                TestContext.Current.CancellationToken);

            Assert.NotNull(cover);
            Assert.False(string.IsNullOrEmpty(cover.Id));
            Assert.Equal(CoverType.Yacht, cover.Type);
            Assert.Equal(30 * 1375m, cover.Premium);
        }

        [Fact]
        public async Task DeleteCoverWithARelatedClaim_ReturnsConflict()
        {
            // Arrange
            using WebApplicationFactory<Program> application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(_ =>
                {});

            HttpClient client = application.CreateClient();
            CoverDto cover = await CreateCoverAsync(client);

            CreateClaimRequest claimRequest = new CreateClaimRequest
            {
                CoverId = cover.Id,
                Created = cover.StartDate.AddDays(1),
                Name = "Claim1",
                Type = ClaimType.Collision,
                DamageCost = 5_000m
            };

            HttpResponseMessage claimResponse = await client.PostAsJsonAsync(
                "/Claims",
                claimRequest,
                SerializerOptions,
                TestContext.Current.CancellationToken);

            claimResponse.EnsureSuccessStatusCode();

            // Act
            HttpResponseMessage response = await client.DeleteAsync(
                $"/Covers/{cover.Id}",
                TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

            ErrorResponse? error = await response.Content.ReadFromJsonAsync<ErrorResponse>(
                SerializerOptions,
                TestContext.Current.CancellationToken);

            Assert.NotNull(error);
            Assert.Equal(ResultErrorCodes.CoverHasClaims, error.Code);
        }

        private static async Task<CoverDto> CreateCoverAsync(HttpClient client)
        {
            CreateCoverRequest request = new CreateCoverRequest
            {
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddDays(30),
                Type = CoverType.Yacht
            };

            HttpResponseMessage response = await client.PostAsJsonAsync(
                "/Covers",
                request,
                SerializerOptions,
                TestContext.Current.CancellationToken);

            response.EnsureSuccessStatusCode();

            CoverDto? cover = await response.Content.ReadFromJsonAsync<CoverDto>(
                SerializerOptions,
                TestContext.Current.CancellationToken);

            Assert.NotNull(cover);

            return cover;
        }

        private static JsonSerializerOptions CreateSerializerOptions()
        {
            JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            options.Converters.Add(new JsonStringEnumConverter());

            return options;
        }
    }
}
