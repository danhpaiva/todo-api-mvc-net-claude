using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models;

public class TodoItemDTO
{
    public long Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "O nome deve ter entre 1 e 200 caracteres.")]
    [RegularExpression(@".*\S.*", ErrorMessage = "O nome não pode conter apenas espaços.")]
    public string Name { get; set; } = string.Empty;

    public bool IsComplete { get; set; }
}
