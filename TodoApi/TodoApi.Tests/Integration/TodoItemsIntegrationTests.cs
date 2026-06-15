using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace TodoApi.Tests.Integration;

public class TodoItemsIntegrationTests : IClassFixture<IntegrationTestFactory>
{
    private readonly HttpClient _client;

    public TodoItemsIntegrationTests(IntegrationTestFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> GetTokenAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            username = "admin",
            password = "senhaforte"
        });

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("token").GetString()!;
    }

    // POST /api/todoitems

    [Fact]
    public async Task Post_ComPayloadValido_Retorna201ComDTO()
    {
        var response = await _client.PostAsJsonAsync("/api/todoitems",
            new { name = "Comprar leite", isComplete = false });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(body.GetProperty("id").GetInt64() > 0);
        Assert.Equal("Comprar leite", body.GetProperty("name").GetString());
        Assert.False(body.GetProperty("isComplete").GetBoolean());
    }

    [Fact]
    public async Task Post_ComNomeVazio_Retorna400()
    {
        var response = await _client.PostAsJsonAsync("/api/todoitems",
            new { name = "", isComplete = false });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_ComNomeApenasEspacos_Retorna400()
    {
        var response = await _client.PostAsJsonAsync("/api/todoitems",
            new { name = "   ", isComplete = false });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // GET /api/todoitems

    [Fact]
    public async Task Get_RetornaListaComItensCriados()
    {
        await _client.PostAsJsonAsync("/api/todoitems", new { name = "Item GET A", isComplete = false });
        await _client.PostAsJsonAsync("/api/todoitems", new { name = "Item GET B", isComplete = true });

        var response = await _client.GetAsync("/api/todoitems");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(body.GetArrayLength() >= 2);
    }

    // GET /api/todoitems/{id}

    [Fact]
    public async Task GetById_ComIdValido_Retorna200()
    {
        var post = await _client.PostAsJsonAsync("/api/todoitems",
            new { name = "Item Específico", isComplete = false });
        var created = await post.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetInt64();

        var response = await _client.GetAsync($"/api/todoitems/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(id, body.GetProperty("id").GetInt64());
    }

    [Fact]
    public async Task GetById_ComIdInexistente_Retorna404()
    {
        var response = await _client.GetAsync("/api/todoitems/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // PUT /api/todoitems/{id}

    [Fact]
    public async Task Put_ComDadosValidos_Retorna204()
    {
        var post = await _client.PostAsJsonAsync("/api/todoitems",
            new { name = "Original", isComplete = false });
        var created = await post.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetInt64();

        var response = await _client.PutAsJsonAsync($"/api/todoitems/{id}",
            new { id, name = "Atualizado", isComplete = true });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Put_ComIdsDivergentes_Retorna400()
    {
        var response = await _client.PutAsJsonAsync("/api/todoitems/1",
            new { id = 2, name = "Teste", isComplete = false });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // DELETE /api/todoitems/{id}

    [Fact]
    public async Task Delete_SemToken_Retorna401()
    {
        var response = await _client.DeleteAsync("/api/todoitems/1");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ComTokenAdmin_Retorna204()
    {
        var token = await GetTokenAsync();

        var post = await _client.PostAsJsonAsync("/api/todoitems",
            new { name = "Para deletar", isComplete = false });
        var created = await post.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetInt64();

        using var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/todoitems/{id}");
        deleteRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(deleteRequest);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    // POST /api/auth/login

    [Fact]
    public async Task Login_ComCredenciaisValidas_Retorna200ComToken()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login",
            new { username = "admin", password = "senhaforte" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        var token = body.GetProperty("token").GetString();
        Assert.False(string.IsNullOrEmpty(token));
    }

    [Fact]
    public async Task Login_ComCredenciaisInvalidas_Retorna401()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login",
            new { username = "hacker", password = "errada123" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_ComSenhaCurta_Retorna400()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login",
            new { username = "admin", password = "123" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // GET /health

    [Fact]
    public async Task Health_Retorna200Healthy()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Healthy", body.GetProperty("status").GetString());
    }
}
