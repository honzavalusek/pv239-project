using MalyFarmar.Api.BusinessLayer.DTOs.Input;
using MalyFarmar.Api.BusinessLayer.DTOs.Output;

public interface IProductService
{
    Task<ProductDetailViewDto?> GetProduct(int productId);
    Task<ProductsListDto> GetProducts(ProductSearchDto searchDto);
    Task<ProductsListDto> GetProductsBySeller(int sellerId);
    Task<ProductDetailViewDto> CreateProduct(ProductCreateDto productDto);
    Task<ProductDetailViewDto?> UpdateProduct(int productId, ProductEditDto productDto);
    Task<bool> DeleteProduct(int productId);
}