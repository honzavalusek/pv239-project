using MalyFarmar.Api.BusinessLayer.DTOs.Input;
using MalyFarmar.Api.BusinessLayer.DTOs.Output;
using Microsoft.AspNetCore.Mvc;

namespace MalyFarmar.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [Route("{productId:int}")]
    public async Task<ActionResult<ProductDetailViewDto>> GetProduct([FromRoute] int productId)
    {
        var productDto = await _productService.GetProduct(productId);

        if (productDto == null)
        {
            return NotFound();
        }

        return Ok(productDto);
    }

    [HttpPost]
    [Route("get-products")]
    public async Task<ActionResult<ProductsListDto>> GetProducts([FromBody] ProductSearchDto searchDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var productsDto = await _productService.GetProducts(searchDto);
        return Ok(productsDto);
    }

    [HttpGet]
    [Route("get-products-by-seller/{sellerId:int}")]
    public async Task<ActionResult<ProductsListDto>> GetProductsBySeller([FromRoute] int sellerId)
    {
        var productsDto = await _productService.GetProductsBySeller(sellerId);
        return Ok(productsDto);
    }

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<ProductDetailViewDto>> CreateProduct([FromBody] ProductCreateDto productDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdProduct = await _productService.CreateProduct(productDto);
        return Ok(createdProduct);
    }

    [HttpPost]
    [Route("{productId:int}/update")]
    public async Task<ActionResult<ProductDetailViewDto>> UpdateProduct([FromRoute] int productId, [FromBody] ProductEditDto productDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updatedProduct = await _productService.UpdateProduct(productId, productDto);

            if (updatedProduct == null)
            {
                return NotFound();
            }

            return Ok(updatedProduct);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete]
    [Route("{productId:int}")]
    public async Task<ActionResult> DeleteProduct([FromRoute] int productId)
    {
        var result = await _productService.DeleteProduct(productId);

        if (!result)
        {
            return NotFound();
        }

        return Ok();
    }
}
