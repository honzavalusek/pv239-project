using Geolocation;
using MalyFarmar.Api.BusinessLayer.DTOs.Input;
using MalyFarmar.Api.BusinessLayer.DTOs.Output;
using MalyFarmar.Api.BusinessLayer.Mappers;
using MalyFarmar.Api.DAL.Data;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace MalyFarmar.Api.BusinessLayer.Services;

public class ProductService : IProductService
{
    private readonly MalyFarmarDbContext _context;

    public ProductService(MalyFarmarDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDetailViewDto?> GetProduct(int productId)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productId);

        return product?.MapToDetailViewDto();
    }

    public async Task<ProductsListDto> GetProducts(ProductSearchDto searchDto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == searchDto.UserSearchingId);

        if (user == null || user.LocationLatitude == null || user.LocationLongitude == null)
        {
            throw new InvalidOperationException("User location is not set.");
        }

        Coordinate userLocation = new Coordinate(user.LocationLatitude.Value, user.LocationLongitude.Value);
        double radiusInMeters = searchDto.RadiusInMeters ?? 10_000;

        var boundaries = new CoordinateBoundaries(userLocation.Latitude, userLocation.Longitude, radiusInMeters, DistanceUnit.Meters);
        var productsWithinBoundaries = await _context.Products
            .Include(p => p.Seller)
            .Where(p => p.Seller.LocationLatitude != null && p.Seller.LocationLongitude != null)
            .Where(p => p.Seller.LocationLatitude >= boundaries.MinLatitude && p.Seller.LocationLatitude <= boundaries.MaxLatitude)
            .Where(p => p.Seller.LocationLongitude >= boundaries.MinLongitude && p.Seller.LocationLongitude <= boundaries.MaxLongitude)
            .ToListAsync();

        var productsListViewDtos = productsWithinBoundaries
            .Select(p => p.MapToListViewDto(userLocation))
            .Where(p => p.DistanceInMeters <= radiusInMeters)
            .Where(p => searchDto.UserSearchingId == null || p.SellerId != searchDto.UserSearchingId)
            .OrderBy(p => p.DistanceInMeters)
            .ToList();

        return new ProductsListDto
        {
            Products = productsListViewDtos
        };
    }

    public async Task<ProductsListDto> GetProductsBySeller(int sellerId)
    {
        var products = await _context.Products
            .Where(p => p.SellerId == sellerId)
            .ToListAsync();

        return new ProductsListDto
        {
            Products = products.Select(p => p.MapToListViewDto()).ToList()
        };
    }

    public async Task<ProductDetailViewDto> CreateProduct(ProductCreateDto productDto)
    {
        string? imageUrl = null;

        if (productDto.Image != null && productDto.Image.Length > 0)
        {
            var fileName = $"{Guid.NewGuid()}.jpg";
            var filePath = Path.Combine("uploads", fileName);
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath);

            using (var image = Image.Load(productDto.Image))
            {
                image.Save(fullPath, new JpegEncoder());
            }

            imageUrl = $"/uploads/{fileName}";
        }

        var addResult = await _context.Products.AddAsync(productDto.MapToEntity(imageUrl));
        await _context.SaveChangesAsync();

        await addResult.Reference(p => p.Seller).LoadAsync();

        return addResult.Entity.MapToDetailViewDto();
    }

    public async Task<ProductDetailViewDto?> UpdateProduct(int productId, ProductEditDto productDto)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
        {
            return null;
        }

        var soldAmount = product.TotalAmount - product.RemainingAmount;

        if (productDto.TotalAmount < soldAmount)
        {
            throw new InvalidOperationException("Total amount cannot be less than sold amount.");
        }

        product.Name = productDto.Name;
        product.Description = productDto.Description;
        product.TotalAmount = productDto.TotalAmount;
        product.RemainingAmount = product.TotalAmount - soldAmount;
        product.PricePerUnit = productDto.PricePerUnit;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return product.MapToDetailViewDto();
    }

    public async Task<bool> DeleteProduct(int productId)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
        {
            return false;
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return true;
    }
}
