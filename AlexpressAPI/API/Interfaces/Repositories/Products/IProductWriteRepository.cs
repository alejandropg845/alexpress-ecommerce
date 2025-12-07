using API.DTOs.CartProductsDTO;
using API.Entities;

namespace API.Interfaces.Repositories.Products
{
    public interface IProductWriteRepository
    {
        Task<Product?> GetProductAsync(int id);
        Task<Product> AddProductAsync(Product product);
        Task SaveProductChangesAsync();
        Task SetProductAsDeletedAsync(int id);
        Task UpdateOrderedProductsAsync(List<CartProductDto> orderedProducts);
        Task SetProductsReviewAsync(List<int> productsIds, int rating);

    }
}
