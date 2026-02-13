using Microsoft.EntityFrameworkCore;
using task.Data;
using task.DTOs;
using task.Model;

namespace task.Repository
{
    public class StudentRepository
    {
        private readonly AppDbContext _db;
        public StudentRepository(AppDbContext db) => _db = db;

        public async Task<(List<StudentResponseDto> Data, int TotalCount)> GetStudentsAsync(
            string? search,
            int? courceId,
            int pageNumber,
            int pageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 100);

            var query = _db.Students
                .AsNoTracking()
                .Include(s => s.Cource)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(s =>
                    s.Name.Contains(search) ||
                    s.Email.Contains(search) ||
                    s.Phone.Contains(search) ||
                    s.Cource!.Name.Contains(search));
            }

            if (courceId.HasValue)
                query = query.Where(s => s.CourceId == courceId.Value);

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderByDescending(s => s.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new StudentResponseDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Age = s.Age,
                    Dob = s.Dob,
                    Email = s.Email,
                    Phone = s.Phone,
                    CourceId = s.CourceId,
                    CourceName = s.Cource!.Name,
                    Teacher = s.Cource!.Teacher
                })
                .ToListAsync();

            return (data, totalCount);
        }

        public async Task<StudentResponseDto?> GetStudentByIdAsync(int id)
        {
            return await _db.Students
                .AsNoTracking()
                .Include(s => s.Cource)
                .Where(s => s.Id == id)
                .Select(s => new StudentResponseDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Age = s.Age,
                    Dob = s.Dob,
                    Email = s.Email,
                    Phone = s.Phone,
                    CourceId = s.CourceId,
                    CourceName = s.Cource!.Name,
                    Teacher = s.Cource!.Teacher
                })
                .FirstOrDefaultAsync();
        }

        public async Task<(bool Ok, string Message, int? NewId)> CreateStudentAsync(StudentCreateDto dto)
        {
            if (dto.Dob.Date > DateTime.Today) return (false, "DOB cannot be in the future.", null);

            var courceExists = await _db.Cources.AnyAsync(c => c.Id == dto.CourceId);
            if (!courceExists) return (false, "Invalid CourceId.", null);

            var emailExists = await _db.Students.AnyAsync(s => s.Email == dto.Email);
            if (emailExists) return (false, "Email already exists.", null);

            var student = new Student
            {
                Name = dto.Name.Trim(),
                Age = dto.Age,
                Dob = dto.Dob,
                Email = dto.Email.Trim(),
                Phone = dto.Phone.Trim(),
                Password = dto.Password,
                CourceId = dto.CourceId
            };

            _db.Students.Add(student);
            await _db.SaveChangesAsync();

            return (true, "Created", student.Id);
        }

        public async Task<(bool Ok, string Message)> UpdateStudentAsync(int id, StudentUpdateDto dto)
        {
            if (dto.Dob.Date > DateTime.Today) return (false, "DOB cannot be in the future.");

            var student = await _db.Students.FindAsync(id);
            if (student == null) return (false, "Student not found.");

            var courceExists = await _db.Cources.AnyAsync(c => c.Id == dto.CourceId);
            if (!courceExists) return (false, "Invalid CourceId.");

            student.Name = dto.Name.Trim();
            student.Age = dto.Age;
            student.Dob = dto.Dob;
            student.Phone = dto.Phone.Trim();
            student.CourceId = dto.CourceId;

            if (!string.IsNullOrWhiteSpace(dto.Password))
                student.Password = dto.Password;

            await _db.SaveChangesAsync();
            return (true, "Updated");
        }

        public async Task<(bool Ok, string Message)> DeleteStudentAsync(int id)
        {
            var student = await _db.Students.FindAsync(id);
            if (student == null) return (false, "Student not found.");

            _db.Students.Remove(student);
            await _db.SaveChangesAsync();
            return (true, "Deleted");
        }
    }
}
