using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoApi.Controllers;
using TodoApi.Models;
using TodoApi.Services.Interfaces;

namespace TodoApi.Tests.Controllers;

public class TodoItemsControllerTests
{
    private readonly Mock<ITodoService> _serviceMock = new();

    private TodoItemsController BuildController() => new(_serviceMock.Object);

    [Fact]
    public async Task GetTodoItems_ReturnsListOfTodoItemDTOs()
    {
        var items = new List<TodoItemDTO>
        {
            new() { Id = 1, Name = "Test Item 1", IsComplete = false },
            new() { Id = 2, Name = "Test Item 2", IsComplete = true }
        };
        _serviceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(items);

        var result = await BuildController().GetTodoItems(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<TodoItemDTO>>(okResult.Value);
        Assert.Equal(2, returned.Count());
    }

    [Fact]
    public async Task GetTodoItem_WithValidId_ReturnsTodoItemDTO()
    {
        var dto = new TodoItemDTO { Id = 1, Name = "Test Item", IsComplete = false };
        _serviceMock.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var result = await BuildController().GetTodoItem(1, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<TodoItemDTO>(okResult.Value);
        Assert.Equal(1, returned.Id);
        Assert.Equal("Test Item", returned.Name);
    }

    [Fact]
    public async Task GetTodoItem_WithInvalidId_ReturnsNotFound()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((TodoItemDTO?)null);

        var result = await BuildController().GetTodoItem(99, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PutTodoItem_WithMatchingId_ReturnsNoContent()
    {
        var dto = new TodoItemDTO { Id = 1, Name = "Updated", IsComplete = true };
        _serviceMock.Setup(s => s.UpdateAsync(1, dto, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await BuildController().PutTodoItem(1, dto, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task PutTodoItem_WithMismatchedId_ReturnsBadRequest()
    {
        var dto = new TodoItemDTO { Id = 1, Name = "Test", IsComplete = false };

        var result = await BuildController().PutTodoItem(2, dto, CancellationToken.None);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task PutTodoItem_WhenItemNotFound_ReturnsNotFound()
    {
        var dto = new TodoItemDTO { Id = 1, Name = "Test", IsComplete = false };
        _serviceMock.Setup(s => s.UpdateAsync(1, dto, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await BuildController().PutTodoItem(1, dto, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task PostTodoItem_ReturnsCreatedAtAction()
    {
        var dto = new TodoItemDTO { Name = "New Item", IsComplete = false };
        var created = new TodoItemDTO { Id = 1, Name = "New Item", IsComplete = false };
        _serviceMock.Setup(s => s.CreateAsync(dto, It.IsAny<CancellationToken>())).ReturnsAsync(created);

        var result = await BuildController().PostTodoItem(dto, CancellationToken.None);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returned = Assert.IsType<TodoItemDTO>(createdResult.Value);
        Assert.Equal("GetTodoItem", createdResult.ActionName);
        Assert.Equal("New Item", returned.Name);
    }

    [Fact]
    public async Task DeleteTodoItem_WhenItemExists_ReturnsNoContent()
    {
        _serviceMock.Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await BuildController().DeleteTodoItem(1, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteTodoItem_WhenItemNotFound_ReturnsNotFound()
    {
        _serviceMock.Setup(s => s.DeleteAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await BuildController().DeleteTodoItem(99, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }
}
