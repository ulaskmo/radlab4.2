using AdsApi;
using AdsApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configure SQLite
builder.Services.AddDbContext<AdsDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("AdsDbContext") ?? throw new InvalidOperationException("Connection string 'AdsDbContext' not found.")));

// Enable JSON Reference Handling to avoid circular references
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve; // Handles cycles
        options.JsonSerializerOptions.WriteIndented = true; // Optional for pretty print
    });

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Initialize seed data
    SeedData.Initialize(services);
}

// Set up routing and endpoints for the Ads API
var ads = app.MapGroup("/ads");

ads.MapGet("/", GetAllAds);
ads.MapGet("/{id}", GetAdById);
ads.MapPost("/", CreateAd);
ads.MapPut("/{id}", UpdateAd);
ads.MapDelete("/{id}", DeleteAd);
ads.MapGet("/seller/{sellerId}", GetAdsBySeller);
ads.MapGet("/category/{categoryId}", GetAdsByCategory);

app.Run();

static async Task<IResult> GetAllAds(AdsDbContext db)
{
    return TypedResults.Ok(await db.Ads.Include(a => a.Seller).Include(a => a.Category).ToListAsync());
}

static async Task<IResult> GetAdById(int id, AdsDbContext db)
{
    var ad = await db.Ads.Include(a => a.Seller).Include(a => a.Category).FirstOrDefaultAsync(a => a.Id == id);
    return ad is not null ? TypedResults.Ok(ad) : TypedResults.NotFound();
}

static async Task<IResult> CreateAd(Ad ad, AdsDbContext db)
{
    var existingCategory = await db.Categories.FindAsync(ad.Category.Id);
    if (existingCategory != null)
    {
        ad.Category = existingCategory;
    }

    var existingSeller = await db.Sellers.FindAsync(ad.Seller.Id);
    if (existingSeller != null)
    {
        ad.Seller = existingSeller;
    }

    db.Ads.Add(ad);
    await db.SaveChangesAsync();

    return TypedResults.Created($"/ads/{ad.Id}", ad);
}


static async Task<IResult> UpdateAd(int id, Ad updatedAd, AdsDbContext db)
{
    var ad = await db.Ads.FindAsync(id);
    if (ad is null) return TypedResults.NotFound();

    ad.Description = updatedAd.Description;
    ad.Price = updatedAd.Price;
    ad.Seller = updatedAd.Seller;
    ad.Category = updatedAd.Category;

    await db.SaveChangesAsync();
    return TypedResults.NoContent();
}

static async Task<IResult> DeleteAd(int id, AdsDbContext db)
{
    var ad = await db.Ads.FindAsync(id);
    if (ad is not null)
    {
        db.Ads.Remove(ad);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}

static async Task<IResult> GetAdsBySeller(int sellerId, AdsDbContext db)
{
    var ads = await db.Ads
        .Include(a => a.Seller)
        .Where(a => a.Seller.Id == sellerId)
        .ToListAsync();
    return TypedResults.Ok(ads);
}

static async Task<IResult> GetAdsByCategory(int categoryId, AdsDbContext db)
{
    var ads = await db.Ads
        .Include(a => a.Category)
        .Where(a => a.Category.Id == categoryId)
        .OrderBy(a => a.Description)
        .ToListAsync();
    return TypedResults.Ok(ads);
}
