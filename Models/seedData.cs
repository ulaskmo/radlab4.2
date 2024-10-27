using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace AdsApi.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AdsDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<AdsDbContext>>()))
            {
                if (context.Ads.Any())
                {
                    return;
                }

                var johnDoe = new Seller { Name = "John Doe", Email = "john@example.com" };
                var janeDoe = new Seller { Name = "Jane Doe", Email = "jane@example.com" };
                context.Sellers.AddRange(johnDoe, janeDoe);

                var electronics = new Category { Name = "Electronics" };
                var vehicles = new Category { Name = "Vehicles" };
                context.Categories.AddRange(electronics, vehicles);

                context.Ads.AddRange(
                    new Ad
                    {
                        Description = "Laptop for sale",
                        Price = 500,
                        Seller = johnDoe,
                        Category = electronics
                    },
                    new Ad
                    {
                        Description = "Car for sale",
                        Price = 10000,
                        Seller = janeDoe,
                        Category = vehicles
                    }
                );

                context.SaveChanges();
            }
        }
    }
}
