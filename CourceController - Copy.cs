using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using task.Data;
using task.DTOs;
using task.Model;

namespace task.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourceController : ControllerBase
    {
        private readonly AppDbContext _db;
        public CourceController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _db.Cources
                .AsNoTracking()
                .Select(c => new CourceResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Teacher = c.Teacher
                })
                .ToListAsync();

            return Ok(courses);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CourceCreateDto dto)
        {
            var course = new Cource
            {
                Name = dto.Name.Trim(),
                Teacher = dto.Teacher.Trim()
            };

            _db.Cources.Add(course);
            await _db.SaveChangesAsync();

            return Ok(new { course.Id });
        }
    }
}

