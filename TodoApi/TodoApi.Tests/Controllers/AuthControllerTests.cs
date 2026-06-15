using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoApi.Controllers;
using TodoApi.Models;
using TodoApi.Services.Interfaces;

namespace TodoApi.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _serviceMock = new();

    private AuthController BuildController() => new(_serviceMock.Object);

    [Fact]
    public void Login_WithValidCredentials_ReturnsOkWithToken()
    {
        var login = new Login { Username = "admin", Password = "senhaforte" };
        _serviceMock.Setup(s => s.GenerateToken(login)).Returns("fake.jwt.token");

        var result = BuildController().Login(login);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var tokenProperty = okResult.Value!.GetType().GetProperty("Token");
        Assert.NotNull(tokenProperty);
        var token = tokenProperty.GetValue(okResult.Value) as string;
        Assert.Equal("fake.jwt.token", token);
    }

    [Fact]
    public void Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        var login = new Login { Username = "invalid", Password = "wrong" };
        _serviceMock.Setup(s => s.GenerateToken(login)).Returns((string?)null);

        var result = BuildController().Login(login);

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Credenciais inválidas.", unauthorizedResult.Value);
    }
}
