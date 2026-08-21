using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Claims.Application.DTO;
using Claims.Domain;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Claims.Tests.Integration
{
    public class ClaimsControllerTests
    {
        private static readonly JsonSerializerOptions SerializerOptions = CreateSerializerOptions();

        [Fact]
        public async Task Get_Claims()
        {
            // Arrange
            using WebApplicationFactory<Program> application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(_ =>
                {});

            HttpClient client = application.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync("/Claims", TestContext.Current.CancellationToken);

            // Assert
            response.EnsureSuccessStatusCode();

            List<ClaimDto>? claims = await response.Content.ReadFromJsonAsync<List<ClaimDto>>(
                SerializerOptions,
                TestContext.Current.CancellationToken);

            Assert.NotNull(claims);
        }

        [Fact]
        public async Task PostClaim_ReturnsTheCreatedClaim()
        {
            // Arrange
            using WebApplicationFactory<Program> application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(_ =>
                {});

            HttpClient client = application.CreateClient();
            CoverDto cover = await CreateCoverAsync(client);

            CreateClaimRequest request = new CreateClaimRequest
            {
                CoverId = cover.Id,
                Created = cover.StartDate.AddDays(1),
                Name = "Claim1",
                Type = ClaimType.Collision,
                DamageCost = 5_000m
            };

            // Act
            HttpResponseMessage response = await client.PostAsJsonAsync(
                "/Claims",
                request,
                SerializerOptions,
                TestContext.Current.CancellationToken);

            // Assert
            response.EnsureSuccessStatusCode();

            ClaimDto? claim = await response.Content.ReadFromJsonAsync<ClaimDto>(
                SerializerOptions,
                TestContext.Current.CancellationToken);

            Assert.NotNull(claim);
            Assert.Equal(cover.Id, claim.CoverId);
            Assert.Equal(5_000m, claim.DamageCost);
            Assert.False(string.IsNullOrEmpty(claim.Id));
        }

        [Fact]
        public async Task GetClaimByUnknownId_ReturnsNotFound()
        {
            // Arrange
            using WebApplicationFactory<Program> application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(_ =>
                {});

            HttpClient client = application.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync(
                $"/Claims/{Guid.NewGuid()}",
                TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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
