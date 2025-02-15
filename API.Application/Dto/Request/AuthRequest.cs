using System.ComponentModel.DataAnnotations;

public class AuthRequest
{
    [Required(ErrorMessage = "O nome do usuário é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome do usuário deve ter no máximo 100 caracteres.")]
    public required string Username { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória.")]
    public required string Password { get; set; }
}