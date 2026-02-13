using Microsoft.AspNetCore.Mvc;
using task.DTOs;
using task.Repository;

namespace task.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly StudentRepository _repo;
        public StudentController(StudentRepository repo) => _repo = repo;

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int? courceId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var (data, totalCount) = await _repo.GetStudentsAsync(search, courceId, pageNumber, pageSize);

            return Ok(new
            {
                pageNumber,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                data
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var student = await _repo.GetStudentByIdAsync(id);
            if (student == null) return NotFound("Student not found");
            return Ok(student);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StudentCreateDto dto)
        {
            var result = await _repo.CreateStudentAsync(dto);
            if (!result.Ok) return BadRequest(result.Message);

            return CreatedAtAction(nameof(GetById), new { id = result.NewId }, new { id = result.NewId });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] StudentUpdateDto dto)
        {
            var result = await _repo.UpdateStudentAsync(id, dto);
            if (!result.Ok) return BadRequest(result.Message);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _repo.DeleteStudentAsync(id);
            if (!result.Ok) return NotFound(result.Message);

            return NoContent();
        }
    }
}

