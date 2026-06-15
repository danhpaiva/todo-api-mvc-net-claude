using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models;

public class Login
{
    [Required(ErrorMessage = "O username é obrigatório.")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "O username deve ter entre 1 e 50 caracteres.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres.")]
    public string Password { get; set; } = string.Empty;
}
