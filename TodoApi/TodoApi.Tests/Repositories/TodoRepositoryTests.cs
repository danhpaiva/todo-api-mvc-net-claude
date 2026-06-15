using Microsoft.EntityFrameworkCore;
using TodoApi.Context;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Tests.Repositories;

public class TodoRepositoryTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private TodoRepository BuildRepository(AppDbContext context) => new(context);

    // GET ALL

    [Fact]
    public async Task GetAllAsync_QuandoExistemItens_DeveRetornarTodos()
    {
        var context = GetInMemoryDbContext();
        context.TodoItems.AddRange(
            new TodoItem { Id = 1, Name = "Item 1", IsComplete = false },
            new TodoItem { Id = 2, Name = "Item 2", IsComplete = true }
        );
        await context.SaveChangesAsync();

        var result = await BuildRepository(context).GetAllAsync(CancellationToken.None);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_QuandoNaoExistemItens_DeveRetornarListaVazia()
    {
        var context = GetInMemoryDbContext();

        var result = await BuildRepository(context).GetAllAsync(CancellationToken.None);

        Assert.Empty(result);
    }

    // GET BY ID

    [Fact]
    public async Task GetByIdAsync_ComIdValido_DeveRetornarItem()
    {
        var context = GetInMemoryDbContext();
        context.TodoItems.Add(new TodoItem { Id = 1, Name = "Item", IsComplete = false });
        await context.SaveChangesAsync();

        var result = await BuildRepository(context).GetByIdAsync(1, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Item", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ComIdInvalido_DeveRetornarNull()
    {
        var context = GetInMemoryDbContext();

        var result = await BuildRepository(context).GetByIdAsync(99, CancellationToken.None);

        Assert.Null(result);
    }

    // CREATE

    [Fact]
    public async Task CreateAsync_DevePersistirERetornarItemCriado()
    {
        var context = GetInMemoryDbContext();
        var item = new TodoItem { Name = "Novo Item", IsComplete = false };

        var result = await BuildRepository(context).CreateAsync(item, CancellationToken.None);

        Assert.True(result.Id > 0);
        Assert.Equal("Novo Item", result.Name);
        Assert.Equal(1, await context.TodoItems.CountAsync());
    }

    // UPDATE

    [Fact]
    public async Task UpdateAsync_ComItemExistente_DeveAtualizarERetornarTrue()
    {
        var context = GetInMemoryDbContext();
        context.TodoItems.Add(new TodoItem { Id = 1, Name = "Original", IsComplete = false });
        await context.SaveChangesAsync();

        var item = await context.TodoItems.FindAsync(1L);
        item!.Name = "Atualizado";
        item.IsComplete = true;

        var result = await BuildRepository(context).UpdateAsync(item, CancellationToken.None);

        Assert.True(result);
        var updated = await context.TodoItems.FindAsync(1L);
        Assert.Equal("Atualizado", updated!.Name);
        Assert.True(updated.IsComplete);
    }

    // DELETE

    [Fact]
    public async Task DeleteAsync_ComItemExistente_DeveRemoverERetornarTrue()
    {
        var context = GetInMemoryDbContext();
        context.TodoItems.Add(new TodoItem { Id = 1, Name = "Para deletar", IsComplete = false });
        await context.SaveChangesAsync();

        var result = await BuildRepository(context).DeleteAsync(1, CancellationToken.None);

        Assert.True(result);
        Assert.Equal(0, await context.TodoItems.CountAsync());
    }

    [Fact]
    public async Task DeleteAsync_ComItemInexistente_DeveRetornarFalse()
    {
        var context = GetInMemoryDbContext();

        var result = await BuildRepository(context).DeleteAsync(99, CancellationToken.None);

        Assert.False(result);
    }

    // EXISTS

    [Fact]
    public async Task ExistsAsync_ComItemExistente_DeveRetornarTrue()
    {
        var context = GetInMemoryDbContext();
        context.TodoItems.Add(new TodoItem { Id = 1, Name = "Item", IsComplete = false });
        await context.SaveChangesAsync();

        var result = await BuildRepository(context).ExistsAsync(1, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_ComItemInexistente_DeveRetornarFalse()
    {
        var context = GetInMemoryDbContext();

        var result = await BuildRepository(context).ExistsAsync(99, CancellationToken.None);

        Assert.False(result);
    }
}
