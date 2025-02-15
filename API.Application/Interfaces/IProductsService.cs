public interface IProductsService
{
    Task<List<Product>> GetProductsAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task<Product> CreateProductAsync(ProductRequest product);
    Task UpdateProductAsync(Product product);
    Task SoftDeleteProductAsync(int id);
}