using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models;

public class Login
{
    [Required(ErrorMessage = "O username é obrigatório.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    public string Password { get; set; } = string.Empty;
}
