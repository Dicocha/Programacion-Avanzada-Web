using MVC.Models;
using MVC.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MVC.Controllers
{
    public class TodoController : Controller
    {
        private const string SessionKeyTodos = "Todos";

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult List()
        {
            var todos = GetTodosFromSession();
            return Json(todos);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateTodoDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                return BadRequest();
            }

            var todos = GetTodosFromSession();
            todos.Add(new TodoItem
            {
                Id = Guid.NewGuid(),
                Title = dto.Title.Trim(),
                Description = string.IsNullOrWhiteSpace(dto.Description) ? string.Empty : dto.Description.Trim(),
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            });
            SaveTodosToSession(todos);
            return Ok();
        }

        [HttpPost]
        public IActionResult Toggle([FromBody] ToggleTodoDto dto)
        {
            var todos = GetTodosFromSession();
            var todo = todos.FirstOrDefault(t => t.Id == dto.Id);
            if (todo == null)
            {
                return NotFound();
            }
            todo.IsCompleted = !todo.IsCompleted;
            SaveTodosToSession(todos);
            return Ok();
        }

        private List<TodoItem> GetTodosFromSession()
        {
            var json = HttpContext.Session.GetString(SessionKeyTodos);
            if (string.IsNullOrEmpty(json))
            {
                return new List<TodoItem>();
            }
            try
            {
                var todos = JsonSerializer.Deserialize<List<TodoItem>>(json);
                return todos ?? new List<TodoItem>();
            }
            catch
            {
                return new List<TodoItem>();
            }
        }

        private void SaveTodosToSession(List<TodoItem> todos)
        {
            var json = JsonSerializer.Serialize(todos);
            HttpContext.Session.SetString(SessionKeyTodos, json);
        }
    }
}
