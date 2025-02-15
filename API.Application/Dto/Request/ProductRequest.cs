using System.ComponentModel.DataAnnotations;

public class ProductRequest
{
    [Required(ErrorMessage = "O nome do produto é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome do produto deve ter no máximo 100 caracteres.")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "A descrição do produto é obrigatória.")]
    [StringLength(500, ErrorMessage = "A descrição do produto deve ter no máximo 500 caracteres.")]
    public required string Description { get; set; }

    [Required(ErrorMessage = "O preço do produto é obrigatório.")]
    [Range(0.01, 1000000, ErrorMessage = "O preço do produto deve estar entre 0.01 e 1.000.000.")]
    public decimal Price { get; set; }
}