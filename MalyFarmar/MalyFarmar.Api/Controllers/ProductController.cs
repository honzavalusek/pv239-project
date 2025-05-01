using Geolocation;
using MalyFarmar.Api.DAL.Data;
using MalyFarmar.Api.DTOs.Input;
using MalyFarmar.Api.DTOs.Output;
using MalyFarmar.Api.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<IActionResult> GetProduct([FromRoute] int productId)
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
    public async Task<IActionResult> GetProducts([FromBody] ProductSearchDto searchDto)
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
    public async Task<IActionResult> GetProductsBySeller([FromRoute] int sellerId)
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
    public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDto productDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Products.Add(productDto.MapToEntity());
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost]
    [Route("{productId:int}/update")]
    public async Task<IActionResult> UpdateProduct([FromRoute] int productId, [FromBody] ProductCreateDto productDto)
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
        product.Unit = productDto.Unit;
        product.PricePerUnit = productDto.PricePerUnit;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete]
    [Route("{productId:int}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] int productId)
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