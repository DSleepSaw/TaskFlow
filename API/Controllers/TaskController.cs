using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.DTOs;
using TaskFlow.Models;

namespace TaskFlow.API.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TasksController : ControllerBase
    {
        private readonly TaskFlowDbContext _db;

        public TasksController(TaskFlowDbContext db)
        {
            _db = db;
        }

        // GET api/tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetAll()
        {
            var tasks = await _db.Tasks
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    IsCompleted = t.IsCompleted,
                    ProjectId = t.ProjectId,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            return Ok(tasks);
        }

        // GET api/tasks/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TaskDto>> GetById(Guid id)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            return Ok(new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                ProjectId = task.ProjectId,
                CreatedAt = task.CreatedAt
            });
        }

        // POST api/tasks
        [HttpPost]
        public async Task<ActionResult<TaskDto>> Create(TaskCreateDto dto)
        {
            // Проверяем, что проект существует
            var project = await _db.Projects.FindAsync(dto.ProjectId);
            if (project == null) return BadRequest("Project not found");

            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                ProjectId = dto.ProjectId,
                CreatedAt = DateTime.UtcNow
            };

            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }

        // PUT api/tasks/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, TaskUpdateDto dto)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.IsCompleted = dto.IsCompleted;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE api/tasks/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        // GET api/projects/{projectId}/tasks
        [HttpGet("/api/projects/{projectId:guid}/tasks")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetByProject(Guid projectId)
        {
            var projectExists = await _db.Projects.AnyAsync(p => p.Id == projectId);
            if (!projectExists) return NotFound();

            var tasks = await _db.Tasks
                .Where(t => t.ProjectId == projectId)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    IsCompleted = t.IsCompleted,
                    ProjectId = t.ProjectId,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            return Ok(tasks);
        }
    }
}
