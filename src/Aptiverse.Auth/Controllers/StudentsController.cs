using Aptiverse.Application.Students.Dtos;
using Aptiverse.Application.Students.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aptiverse.Auth.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController(IStudentService studentService, ILogger<StudentsController> logger) : ControllerBase
    {
        private readonly IStudentService _studentService = studentService;
        private readonly ILogger<StudentsController> _logger = logger;

        [HttpPost]
        public async Task<StudentDto> CreateStudent([FromBody] StudentDto student)
        {
            return await _studentService.CreateStudentAsync(student);
        }

        [HttpGet("{id}")]
        public async Task<StudentDto> GetOneStudent(long id)
        {
            return await _studentService.GetOneStudentAsync(id);
        }

        [HttpGet]
        [Authorize(Policy = "ParentAccess")]
        public async Task<IEnumerable<StudentDto>> GetManyStudents([FromQuery] string filter = "{}")
        {
            return await _studentService.GetManyStudentAsync(filter);
        }

        [HttpPut("{id}")]
        public async Task<StudentDto> UpdateStudent(long id, [FromBody] StudentDto student)
        {
            return await _studentService.UpdateStudentAsync(id, student);
        }

        [Authorize(Roles = "Admin,SuperUser")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            try
            {
                await _studentService.DeleteStudentAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
