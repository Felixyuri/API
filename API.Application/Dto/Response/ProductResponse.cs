public class ProductResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }

    public ProductResponse(int id, string name, string description, decimal price)
    {
        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.Price = price;
    }

    public static ProductResponse ToApi(Product product)
    {
        return new ProductResponse(
            id: product.Id,
            name: product.Name,
            description: product.Description,
            price: product.Price
        );
    }
}