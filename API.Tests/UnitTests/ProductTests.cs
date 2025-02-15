using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System.Threading.Tasks;

public class ProductServiceTests
{
    private readonly AppDbContext _dbContext;
    private readonly ProductsService _productsService;

    public ProductServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _dbContext = new AppDbContext(options);
        _productsService = new ProductsService(_dbContext);
    }

    [Fact]
    public async Task CreateProduct()
    {
        var productRequest = new ProductRequest
        {
            Name = "Product",
            Price = 50,
            Description = "Description"
        };

        await _productsService.CreateProductAsync(productRequest);

        var savedProduct = await _dbContext.Products.FirstOrDefaultAsync(p => p.Name == "Product");
        Assert.NotNull(savedProduct);
        Assert.Equal(50, savedProduct.Price);
        Assert.Equal("Description", savedProduct.Description);
    }

    [Fact]
    public async Task GetProducts_NonDeleted()
    {
        _dbContext.Products.RemoveRange(_dbContext.Products);
        await _dbContext.SaveChangesAsync();

        var product = new Product
        {
            Name = "Product",
            Price = 50,
            Description = "Description",
            IsDeleted = false
        };

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        var result = await _productsService.GetProductsAsync();

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.DoesNotContain(result, p => p.IsDeleted);
    }

    [Fact]
    public async Task GetProductById()
    {
        var product = new Product { Name = "Product", Price = 50, Description = "string 1" };
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        var foundProduct = await _productsService.GetProductByIdAsync(product.Id);

        Assert.NotNull(foundProduct);
        Assert.Equal(product.Id, foundProduct.Id);
    }

    [Fact]
    public async Task GetProductById_NotFound()
    {
        var product = await _productsService.GetProductByIdAsync(999);
        Assert.Null(product);
    }

    [Fact]
    public async Task UpdateProduct()
    {
        var product = new Product { Name = "Product 1", Price = 30, Description = "string" };
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        product.Name = "Updated Product";
        product.Price = 100;
        product.Description = "Updated Description";

        await _productsService.UpdateProductAsync(product);

        var updatedProduct = await _dbContext.Products.FindAsync(product.Id);
        Assert.NotNull(updatedProduct);
        Assert.Equal("Updated Product", updatedProduct.Name);
        Assert.Equal(100, updatedProduct.Price);
        Assert.Equal("Updated Description", updatedProduct.Description);
    }

    [Fact]
    public async Task SoftDelete()
    {
        var product = new Product { Name = "Product", Price = 50, Description = "Description", IsDeleted = false };
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        await _productsService.SoftDeleteProductAsync(product.Id);
        var deletedProduct = await _dbContext.Products.FindAsync(product.Id);

        Assert.NotNull(deletedProduct);
        Assert.True(deletedProduct.IsDeleted);
    }

    [Fact]
    public async Task SoftDelete_NotFound()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => _productsService.SoftDeleteProductAsync(999));
    }
}
