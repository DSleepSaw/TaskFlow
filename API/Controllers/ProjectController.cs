using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.DTOs;
using TaskFlow.Models;

namespace TaskFlow.API.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectController : ControllerBase
    {
        private readonly TaskFlowDbContext _db;
        public ProjectController(TaskFlowDbContext db)
        {
            _db = db;
        }
        // GET api/projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetAll()
        {
            var projects = await _db.Projects
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            return Ok(projects);
        }

        // GET api/projects/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProjectDto>> GetById(Guid id)
        {
            var project = await _db.Projects.FindAsync(id);
            if (project == null)
                return NotFound();

            return Ok(new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt
            });
        }

        // POST api/projects
        [HttpPost]
        public async Task<ActionResult<ProjectDto>> Create(ProjectCreateDto dto)
        {
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow
            };

            _db.Projects.Add(project);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
        }
        // PUT api/projects/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, ProjectUpdateDto dto)
        {
            var project = await _db.Projects.FindAsync(id);
            if (project == null)
                return NotFound();

            project.Name = dto.Name;
            project.Description = dto.Description;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE api/projects/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var project = await _db.Projects.FindAsync(id);
            if (project == null)
                return NotFound();

            _db.Projects.Remove(project);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }

}
