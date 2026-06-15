using TodoApi.Models;

namespace TodoApi.Services.Interfaces;

public interface IAuthService
{
    string? GenerateToken(Login login);
}
