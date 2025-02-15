using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductsService _productsService;
    private readonly IMemoryCache _cache;
    private const string ProductKey  = "product";


    public ProductsController(IProductsService productsService, IMemoryCache cache) {
        _productsService = productsService;
        _cache = cache;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> Get() {
        if(_cache.TryGetValue("Products", out IEnumerable<ProductResponse>? cachedProducts)) {
            return Ok(cachedProducts);
        }
        
        var products = await _productsService.GetProductsAsync();

        var productsResults = products.Select(ProductResponse.ToApi);

        _cache.Set(ProductKey, productsResults, TimeSpan.FromMinutes(5));

        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponse>> GetProductById(int id) {
        if(_cache.TryGetValue($"{ProductKey}_{id}", out List<Product>? cachedProduct)) {
            return Ok(cachedProduct);
        }

        var product = await _productsService.GetProductByIdAsync(id);

        if (product == null) {
            return NotFound();
        }

        var productResult = ProductResponse.ToApi(product);

        _cache.Set(ProductKey, product, TimeSpan.FromMinutes(5));

        return Ok(productResult);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> Create([FromBody] ProductRequest productRequest) {
        var createdProduct = await _productsService.CreateProductAsync(productRequest);

        _cache.Remove("Products");

        return Ok(createdProduct);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult> Update(int id, [FromBody] ProductRequest updatedProduct) {
        try {
            var productObj = new Product {
                Id = id,
                Name = updatedProduct.Name,
                Price = updatedProduct.Price,
                Description = updatedProduct.Description,
                IsDeleted = false
            };

            await _productsService.UpdateProductAsync(productObj);

            _cache.Remove("Products");
        }
        catch (Exception) {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id) {
        try {
            await _productsService.SoftDeleteProductAsync(id);

            _cache.Remove("Products");
        }
        catch (Exception) {
            return NotFound();
        }

        return NoContent();
    }
}