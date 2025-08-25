using Aptiverse.Application.Students.Dtos;

namespace Aptiverse.Application.Students.Services
{
    public interface IStudentService
    {
        Task<StudentDto> CreateStudentAsync(StudentDto studentDto);
        Task<StudentDto> GetOneStudentAsync(long id);
        Task<IEnumerable<StudentDto>> GetManyStudentAsync(string filter);
        Task<StudentDto> UpdateStudentAsync(long id, StudentDto studentDto);
        Task DeleteStudentAsync(long id);
    }
}