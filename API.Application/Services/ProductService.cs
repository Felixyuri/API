using Microsoft.EntityFrameworkCore;

public class ProductsService : IProductsService
{
    private readonly AppDbContext _context;

    public ProductsService(AppDbContext context) {
        _context = context;
    }

    public async Task<List<Product>> GetProductsAsync() {
        var products = await _context.Products.Where(p => !p.IsDeleted).ToListAsync();

        return products ?? [];
    }

    public async Task<Product?> GetProductByIdAsync(int id) {
        return await _context.Products.FindAsync(id);
    }

    public async Task<Product> CreateProductAsync(ProductRequest product) {
        var productObj = new Product {
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            IsDeleted = false
        };

        _context.Products.Add(productObj);
        await _context.SaveChangesAsync();
        
        return productObj;
    }

    public async Task UpdateProductAsync(Product product) {
        _context.Entry(product).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteProductAsync(int id) {
        var product = await _context.Products.FindAsync(id);
        if (product == null || product.IsDeleted) {
            throw new InvalidOperationException("Product not found or already deleted.");
        }

        product.IsDeleted = true;
        _context.Entry(product).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}