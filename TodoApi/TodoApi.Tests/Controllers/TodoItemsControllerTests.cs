using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using TodoApi.Context;
using TodoApi.Controllers;
using TodoApi.Models;

namespace TodoApi.Tests.Controllers;

public class TodoItemsControllerTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private TodoItemsController BuildController(AppDbContext context) =>
        new(context, new MemoryCache(new MemoryCacheOptions()), NullLogger<TodoItemsController>.Instance);

    [Fact]
    public async Task GetTodoItems_ReturnsListOfTodoItemDTOs()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.TodoItems.Add(new TodoItem { Id = 1, Name = "Test Item 1", IsComplete = false });
        context.TodoItems.Add(new TodoItem { Id = 2, Name = "Test Item 2", IsComplete = true });
        await context.SaveChangesAsync();

        var controller = BuildController(context);

        // Act
        var result = await controller.GetTodoItems(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var todoItems = Assert.IsAssignableFrom<IEnumerable<TodoItemDTO>>(okResult.Value);
        Assert.Equal(2, todoItems.Count());
        Assert.Contains(todoItems, i => i.Name == "Test Item 1");
        Assert.Contains(todoItems, i => i.Name == "Test Item 2");
    }

    [Fact]
    public async Task GetTodoItem_WithValidId_ReturnsTodoItemDTO()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.TodoItems.Add(new TodoItem { Id = 1, Name = "Test Item", IsComplete = false });
        await context.SaveChangesAsync();

        var controller = BuildController(context);

        // Act
        var result = await controller.GetTodoItem(1, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var todoItem = Assert.IsType<TodoItemDTO>(okResult.Value);
        Assert.Equal(1, todoItem.Id);
        Assert.Equal("Test Item", todoItem.Name);
        Assert.False(todoItem.IsComplete);
    }

    [Fact]
    public async Task GetTodoItem_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = BuildController(context);

        // Act
        var result = await controller.GetTodoItem(99, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PutTodoItem_WithMatchingId_ReturnsNoContent()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.TodoItems.Add(new TodoItem { Id = 1, Name = "Original Name", IsComplete = false });
        await context.SaveChangesAsync();

        var controller = BuildController(context);
        var updatedTodoDTO = new TodoItemDTO { Id = 1, Name = "Updated Name", IsComplete = true };

        // Act
        var result = await controller.PutTodoItem(1, updatedTodoDTO, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);

        var updatedItem = await context.TodoItems.FindAsync(1L);
        Assert.NotNull(updatedItem);
        Assert.Equal("Updated Name", updatedItem.Name);
        Assert.True(updatedItem.IsComplete);
    }

    [Fact]
    public async Task PutTodoItem_WithMismatchedId_ReturnsBadRequest()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = BuildController(context);
        var todoDTO = new TodoItemDTO { Id = 1, Name = "Test", IsComplete = false };

        // Act
        var result = await controller.PutTodoItem(2, todoDTO, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task PutTodoItem_WhenItemNotFound_ReturnsNotFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = BuildController(context);
        var todoDTO = new TodoItemDTO { Id = 1, Name = "Test", IsComplete = false };

        // Act
        var result = await controller.PutTodoItem(1, todoDTO, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task PostTodoItem_ReturnsCreatedAtAction()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = BuildController(context);
        var newTodoDTO = new TodoItemDTO { Name = "New Item", IsComplete = false };

        // Act
        var result = await controller.PostTodoItem(newTodoDTO, CancellationToken.None);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var todoItemDTO = Assert.IsType<TodoItemDTO>(createdAtActionResult.Value);

        Assert.Equal("GetTodoItem", createdAtActionResult.ActionName);
        Assert.Equal("New Item", todoItemDTO.Name);
        Assert.False(todoItemDTO.IsComplete);

        var addedItem = await context.TodoItems.FindAsync(todoItemDTO.Id);
        Assert.NotNull(addedItem);
    }

    [Fact]
    public async Task DeleteTodoItem_WhenItemExists_ReturnsNoContent()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.TodoItems.Add(new TodoItem { Id = 1, Name = "Item to Delete", IsComplete = false });
        await context.SaveChangesAsync();

        var controller = BuildController(context);

        // Act
        var result = await controller.DeleteTodoItem(1, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Empty(await context.TodoItems.ToListAsync());
    }

    [Fact]
    public async Task DeleteTodoItem_WhenItemNotFound_ReturnsNotFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = BuildController(context);

        // Act
        var result = await controller.DeleteTodoItem(99, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
