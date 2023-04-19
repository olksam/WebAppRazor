using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using Moq;

using WebApi.Data;
using WebApi.DTOs.Pagination;
using WebApi.Services;

namespace Tests;

public class Calculator {
    public int Add(int a, int b) {
        return a + b;
    }
}

class FakeEmailSender : IEmailSender {
    public Task SendEmail(string to, string text, string title) {
        return Task.CompletedTask;
    }
}

public class TodoServiceTests {
    // Fact, Theory

    public static IEnumerable<object[]> AddData() {
        yield return new object[] { 1, 2, 3 };
        yield return new object[] { 2, 2, 4 };
        yield return new object[] { 3, 2, 5 };
    }

    [Theory]
    [MemberData(nameof(AddData))]
    public void Add_ReturnsResult(int a, int b, int expectedResult) {
        var calculator = new Calculator();

        var actualResult = calculator.Add(a, b);

        Assert.Equal(expectedResult, actualResult);
    }


    [Fact]
    public async Task GetTodoItem_ReturnsTodoItemWhichBelongsToUser() {
        // Arrange
        var dbContext = new TodoDbContext(new DbContextOptionsBuilder()
            //.UseSqlite("Data Source=:memory:")
            .UseInMemoryDatabase("test")
            .Options);
        var emailSender = new FakeEmailSender();
        //await dbContext.Database.MigrateAsync();


        var user = dbContext.Users.Add(new WebApi.Entities.AppUser {
            UserName = "Test",
            Email = "test@gmail.com"
        }).Entity;

        var createdTodoItem = dbContext.TodoItems.Add(new WebApi.Entities.TodoItem {
            Text = "Test",
            UserId = user.Id,
        }).Entity;

        await dbContext.SaveChangesAsync();

        var service = new TodoService(dbContext, emailSender);


        // Act
        var retrievedTodoItem = await service.GetTodoItem(user.Id, createdTodoItem.Id);

        // Assert
        Assert.NotNull(retrievedTodoItem);
    }

    [Fact]
    public async Task GetTodoItems_ReturnsPaginatedTodoItemsWhichBelongToUser() {
        // Arrange
        var dbContext = new TodoDbContext(new DbContextOptionsBuilder()
            //.UseSqlite("Data Source=:memory:")
            .UseInMemoryDatabase("test")
            .Options);
        var emailSender = new FakeEmailSender();
        //await dbContext.Database.MigrateAsync();


        var user = dbContext.Users.Add(new WebApi.Entities.AppUser {
            UserName = "Test",
            Email = "test@gmail.com"
        }).Entity;

        Enumerable
            .Range(1, 5)
            .Select(i => new WebApi.Entities.TodoItem {
                Text = $"Test {i}",
                UserId = user.Id,
            })
            .ToList()
            .ForEach(todo => dbContext.TodoItems.Add(todo));



        await dbContext.SaveChangesAsync();

        var service = new TodoService(dbContext, emailSender);


        // Act
        var retrievedTodoItems = await service.GetTodoItems(user.Id, 1, 3, null, null);

        // Assert
        retrievedTodoItems.Should().NotBeNull();
        retrievedTodoItems.Meta.Should().BeEquivalentTo(new PaginationMeta(1, 3, 2));

        retrievedTodoItems.Items
            .Should().HaveCount(3)
            .And.ContainSingle(item => item.Text == "Test 1")
            .And.ContainSingle(item => item.Text == "Test 2")
            .And.ContainSingle(item => item.Text == "Test 3");
    }

    [Fact]
    public async Task GetTodoItem_DoesNotReturnTodoItemWhichDoesNotBelongToUser() {
        // Arrange
        var dbContext = new TodoDbContext(new DbContextOptionsBuilder()
            //.UseSqlite("Data Source=:memory:")
            .UseInMemoryDatabase("test")
            .Options);
        var emailSender = new FakeEmailSender();
        //await dbContext.Database.MigrateAsync();


        var user = dbContext.Users.Add(new WebApi.Entities.AppUser {
            UserName = "Test",
            Email = "test@gmail.com"
        }).Entity;

        var anotherUser = dbContext.Users.Add(new WebApi.Entities.AppUser {
            UserName = "Test2",
            Email = "test2@gmail.com"
        }).Entity;

        var createdTodoItem = dbContext.TodoItems.Add(new WebApi.Entities.TodoItem {
            Text = "Test",
            UserId = user.Id,
        }).Entity;

        await dbContext.SaveChangesAsync();

        var service = new TodoService(dbContext, emailSender);


        // Act
        var retrievedTodoItem = await service.GetTodoItem(anotherUser.Id, createdTodoItem.Id);

        // Assert
        Assert.Null(retrievedTodoItem);
    }


    [Fact]
    public async Task CreateTodoItem_ThrowsException_UserNotFound() {
        // Arrange
        var userId = "123";
        var todoItemToCreate = new WebApi.DTOs.CreateTodoItemRequest {
            Text = "Test"
        };

        var dbContext = new TodoDbContext(new DbContextOptionsBuilder()
            .UseInMemoryDatabase("test")
            .Options);

        var emailSender = new FakeEmailSender();

        var service = new TodoService(dbContext, emailSender);


        // Act


        //Record.ExceptionAsync(async () => {
        //    var retrievedTodoItem = await service.CreateTodoItem(userId, todoItemToCreate);
        //}).Result.Should().BeOfType<KeyNotFoundException>();


        await service
            .Awaiting(s => s.CreateTodoItem(userId, todoItemToCreate))
            .Should()
            .ThrowAsync<KeyNotFoundException>();

        //try {
        //    var retrievedTodoItem = await service.CreateTodoItem(userId, todoItemToCreate);
        //} catch (Exception ex) {
        //    Assert.True(ex is KeyNotFoundException);
        //}

        // Assert

    }


    [Fact]
    public async Task CreateTodoItem_CreatesNewItem_And_SendsEmailNotification() {
        // Arrange
        var dbContext = new TodoDbContext(new DbContextOptionsBuilder()
            .UseInMemoryDatabase("test")
            .Options);
        
       

        var user = dbContext.Users.Add(new WebApi.Entities.AppUser {
            UserName = "Test",
            Email = "test@gmail.com"
        }).Entity;

        var todoItemToCreate = new WebApi.DTOs.CreateTodoItemRequest {
            Text = "Test"
        };

        await dbContext.SaveChangesAsync();

        var emailSender = new Mock<IEmailSender>(MockBehavior.Strict);
        emailSender.Setup(e => e.SendEmail(user.Email!, It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var service = new TodoService(dbContext, emailSender.Object);


        // Act
        var retrievedTodoItem = await service.CreateTodoItem(user.Id, todoItemToCreate);

        // Assert
        retrievedTodoItem.Should().NotBeNull();

        emailSender.VerifyAll();
        //emailSender.Verify(e => e.SendEmail(user.Email!, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
}