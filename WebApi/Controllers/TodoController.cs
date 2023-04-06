using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

using WebApi.DTOs;
using WebApi.DTOs.Pagination;
using WebApi.Providers;
using WebApi.Services;

namespace WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TodoController : ControllerBase {
    private readonly ITodoService _todoService;
    private readonly IRequestUserProvider _userProvider;

    public TodoController(
        ITodoService todoService, 
        IRequestUserProvider userProvider) {
        _todoService = todoService;
        _userProvider = userProvider;
    }

    [Authorize(Roles = "admin")]
    [HttpGet("/api/users/{userId}/[controller]")]
    public async Task<ActionResult<PaginatedListDto<TodoItemDto>>> GetUserList(
        [FromQuery] TodoQueryFilters filters, // search, completed
        [FromQuery] PaginationRequest pagination, // page, pageSize
        string userId) { 

        return await _todoService.GetTodoItems(
            userId,
            pagination.Page,
            pagination.PageSize,
            filters.Search,
            filters.Completed);
    }

    // GET: api/<TodoController>
    [HttpGet]
    public async Task<ActionResult<PaginatedListDto<TodoItemDto>>> GetList(
        [FromQuery] TodoQueryFilters filters, // search, completed
        [FromQuery] PaginationRequest pagination) { // page, pageSize

        var user = _userProvider.GetUserInfo();


        return await _todoService.GetTodoItems(
            user.Id,
            pagination.Page, 
            pagination.PageSize, 
            filters.Search, 
            filters.Completed);
    }

    // GET api/<TodoController>/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItemDto>> Get(int id) {
        var user = _userProvider.GetUserInfo();

        var item = await _todoService.GetTodoItem(user.Id, id);

        return item != null
            ? item // -> Ok(item)
            : NotFound();
    }

    /// <summary>
    /// Creates new todo item.
    /// </summary>
    /// <param name="request">Request payload</param>
    /// <response code="201">Success</response>
    /// <response code="409">Task already created</response>
    /// <response code="403">Forbidden</response>
    [HttpPost]
    public async Task<ActionResult<TodoItemDto>> Create([FromBody] CreateTodoItemRequest request) {
        var user = _userProvider.GetUserInfo();

        var createdItem = await _todoService.CreateTodoItem(user.Id, request);

        return CreatedAtAction(nameof(Get), new {id = createdItem.Id}, createdItem);
    }

    // PATCH api/todo/5/status
    [HttpPatch("{id}/status")]
    public async Task<ActionResult<TodoItemDto>> ChangeStatus(int id, [FromBody] bool isCompleted) {
        var user = _userProvider.GetUserInfo();

        var todoItem = await _todoService.ChangeTodoItemStatus(user.Id, id, isCompleted);

        return todoItem != null
            ? todoItem
            : NotFound();
    }
}

// MVC
// CREATE:
//   GET  /products/create -> html
//   POST /products/create -> html
// UPDATE:
//   GET  /products/update/{id} -> html
//   POST /products/update/{id} -> html
// DELETE:
//   GET  /products/delete/{id} -> html
//   POST /products/delete/{id} -> html
// GET ALL:
//   GET  /products/index -> html
// GET ONE:
//   GET  /products/{id}/details -> html




// WEB API
// CREATE
//   POST /products -> json
// UPDATE (replace)
//   PUT /products/{id} -> json
// UPDATE (modify)
//   PATCH /products/{id} -> json
// DELETE
//   DELETE /products/{id} -> json
// GET ALL
//   GET /products -> json
// GET ONE
//   GET /products/{id} -> json

// GET suppliers/{supplierId}/products/{productId}/reviews + dadada

// GET /getReviewsForProductForSupplier?productId=1&supplierId=2 - nonono

// todoist