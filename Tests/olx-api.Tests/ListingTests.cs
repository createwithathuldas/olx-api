using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using olx_api.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace olx_api.Tests
{
    public class ListingTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public ListingTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateListing_AsDraft_AndGetMyListings_Works()
        {
            var auth = await RegisterAndVerifyUserAsync("listinguser@example.com", "+15550000007");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.Token);

            var createDto = new CreateListingDto(
                Title: "Test Phone",
                Description: "A new test phone.",
                Price: 199.99m,
                IsNegotiable: false,
                CityId: 1,
                CategoryId: 1,
                Condition: "New",
                SpecificationsJson: null,
                Status: "Draft");

            var createResponse = await _client.PostAsJsonAsync("/api/listings", createDto);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var createdListing = await createResponse.Content.ReadFromJsonAsync<ListingResponseDto>();
            Assert.NotNull(createdListing);
            Assert.Equal("Draft", createdListing!.Status);

            // Promote the draft to Active so it appears in public listings by adding a primary image directly in the test DB.
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<olx_api.Data.ApplicationDbContext>();
                var listingEntity = db.Listings.Find(createdListing.Id);
                if (listingEntity != null)
                {
                    db.ListingImages.Add(new olx_api.Models.ListingImage { ListingId = listingEntity.Id, ImageUrl = "/test.jpg", IsPrimary = true });
                    listingEntity.Status = "Active";
                    db.SaveChanges();
                }
            }

            var myListings = await _client.GetFromJsonAsync<IEnumerable<ListingResponseDto>>("/api/listings/my");
            Assert.NotNull(myListings);
            Assert.Contains(myListings, x => x.Id == createdListing.Id);
        }

        [Fact]
        public async Task CreateListing_WithActiveStatusWithoutImages_ReturnsBadRequest()
        {
            var auth = await RegisterAndVerifyUserAsync("activecreate@example.com", "+15550000008");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.Token);

            var createDto = new CreateListingDto(
                Title: "Instant Active",
                Description: "Should fail because there are no images.",
                Price: 10m,
                IsNegotiable: true,
                CityId: 1,
                CategoryId: 1,
                Condition: "Used",
                SpecificationsJson: null,
                Status: "Active");

            var createResponse = await _client.PostAsJsonAsync("/api/listings", createDto);
            Assert.Equal(HttpStatusCode.BadRequest, createResponse.StatusCode);
        }

        [Fact]
        public async Task GetListings_ReturnsPagedResult_AfterCreation()
        {
            var auth = await RegisterAndVerifyUserAsync("pagedlistings@example.com", "+15550000009");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.Token);

            var createDto = new CreateListingDto(
                Title: "Paged Item",
                Description: "List this item as draft.",
                Price: 49.99m,
                IsNegotiable: true,
                CityId: 1,
                CategoryId: 1,
                Condition: "Good",
                SpecificationsJson: null,
                Status: "Draft");

            var createResponse = await _client.PostAsJsonAsync("/api/listings", createDto);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var createdListing = await createResponse.Content.ReadFromJsonAsync<ListingResponseDto>();
            Assert.NotNull(createdListing);

            // Promote the draft to Active by adding a ListingImage in the test DB so it shows up in public listings
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<olx_api.Data.ApplicationDbContext>();
                var listingEntity = db.Listings.Find(createdListing!.Id);
                if (listingEntity != null)
                {
                    db.ListingImages.Add(new olx_api.Models.ListingImage { ListingId = listingEntity.Id, ImageUrl = "/test.jpg", IsPrimary = true });
                    listingEntity.Status = "Active";
                    db.SaveChanges();
                }
            }

            var queryResponse = await _client.GetAsync("/api/listings?page=1&pageSize=10");
            Assert.Equal(HttpStatusCode.OK, queryResponse.StatusCode);

            var paged = await queryResponse.Content.ReadFromJsonAsync<PagedResultDto<ListingResponseDto>>();
            Assert.NotNull(paged);
            Assert.True(paged!.TotalCount >= 1);
            Assert.Contains(paged.Items, item => item.Title == "Paged Item");
        }

        private async Task<AuthResponseDto> RegisterAndVerifyUserAsync(string email, string phone)
        {
            var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", new RegisterDto(
                FullName: "Listing User",
                Email: email,
                Password: "Password1!",
                ConfirmPassword: "Password1!",
                PhoneNumber: phone));

            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

            var verificationCode = _factory.EmailService.GetRegistrationOtp(email);
            Assert.False(string.IsNullOrWhiteSpace(verificationCode));

            var verifyResponse = await _client.PostAsJsonAsync("/api/auth/verify-otp", new VerifyOtpDto(email, verificationCode!));
            Assert.Equal(HttpStatusCode.OK, verifyResponse.StatusCode);

            var auth = await verifyResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
            Assert.NotNull(auth);
            return auth!;
        }
    }
}
