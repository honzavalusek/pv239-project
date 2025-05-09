using System.Drawing;
using Geolocation;
using MalyFarmar.Api.DAL.Data;
using MalyFarmar.Api.DTOs.Input;
using MalyFarmar.Api.DTOs.Output;
using MalyFarmar.Api.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace MalyFarmar.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : Controller
{
    private readonly MalyFarmarDbContext _context;

    public ProductController(MalyFarmarDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("{productId:int}")]
    public async Task<ActionResult<ProductDetailViewDto>> GetProduct([FromRoute] int productId)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product.MapToDetailViewDto());
    }

    [HttpPost]
    [Route("get-products")]
    public async Task<ActionResult<ProductsListDto>> GetProducts([FromBody] ProductSearchDto searchDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Coordinate userLocation = new Coordinate(searchDto.Latitude, searchDto.Longitude);
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
            .OrderBy(p => p.DistanceInMeters)
            .ToList();

        return Ok(new ProductsListDto
        {
            Products = productsListViewDtos
        });
    }

    [HttpGet]
    [Route("get-products-by-seller/{sellerId:int}")]
    public async Task<ActionResult<ProductsListDto>> GetProductsBySeller([FromRoute] int sellerId)
    {
        var products = await _context.Products
            .Where(p => p.SellerId == sellerId)
            .ToListAsync();

        return Ok(new ProductsListDto
        {
            Products = products.Select(p => p.MapToListViewDto()).ToList()
        });
    }

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<ProductDetailViewDto>> CreateProduct([FromBody] ProductCreateDto productDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

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

        return Ok(addResult.Entity.MapToDetailViewDto());
    }

    [HttpPost]
    [Route("{productId:int}/update")]
    public async Task<ActionResult<ProductDetailViewDto>> UpdateProduct([FromRoute] int productId, [FromBody] ProductEditDto productDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
        {
            return NotFound();
        }

        var soldAmount = product.TotalAmount - product.RemainingAmount;

        if (productDto.TotalAmount < soldAmount)
        {
            return BadRequest("Total amount cannot be less than sold amount.");
        }

        product.Name = productDto.Name;
        product.Description = productDto.Description;
        product.TotalAmount = productDto.TotalAmount;
        product.RemainingAmount = product.TotalAmount - soldAmount;
        product.PricePerUnit = productDto.PricePerUnit;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(product.MapToDetailViewDto());
    }

    [HttpDelete]
    [Route("{productId:int}")]
    public async Task<ActionResult> DeleteProduct([FromRoute] int productId)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
        {
            return NotFound();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return Ok();
    }
}
