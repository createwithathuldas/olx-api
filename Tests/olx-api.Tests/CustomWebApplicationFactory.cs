using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using olx_api.Data;
using olx_api.Models;
using olx_api.Services;

namespace olx_api.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public CustomWebApplicationFactory()
        {
            Environment.SetEnvironmentVariable("DB_CONNECTION", "Server=(localdb)\\MSSQLLocalDB;Database=OlxApiTest;Trusted_Connection=True;");
            Environment.SetEnvironmentVariable("AUTO_APPLY_MIGRATIONS", "false");
        }

        public TestEmailService EmailService { get; private set; } = null!;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
                services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("TestDb"));

                services.RemoveAll<IEmailService>();
                var testEmailService = new TestEmailService();
                services.AddSingleton(testEmailService);
                services.AddSingleton<IEmailService>(testEmailService);

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                SeedTestData(db);
                EmailService = testEmailService;
            });
        }

        private static void SeedTestData(ApplicationDbContext db)
        {
            if (db.Categories.Any() || db.Countries.Any())
            {
                return;
            }

            var country = new Country
            {
                Name = "TestCountry",
                States = new List<State>()
            };

            var state = new State
            {
                Name = "TestState",
                Country = country,
                Cities = new List<City>()
            };
            country.States.Add(state);

            var city = new City
            {
                Name = "TestCity",
                State = state
            };
            state.Cities.Add(city);

            var category = new Category
            {
                Name = "Electronics",
                IconUrl = "electronics.png",
                SubCategories = new List<Category>(),
                Listings = new List<Listing>()
            };

            db.Countries.Add(country);
            db.Categories.Add(category);
            db.SaveChanges();
        }
    }
}
