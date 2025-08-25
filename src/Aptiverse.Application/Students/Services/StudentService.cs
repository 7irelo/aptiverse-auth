using Aptiverse.Application.Students.Dtos;

namespace Aptiverse.Application.Students.Services
{
    public class StudentService : IStudentService
    {
        public Task<StudentDto> CreateStudentAsync(StudentDto studentDto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteStudentAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<StudentDto>> GetManyStudentAsync(string filter)
        {
            throw new NotImplementedException();
        }

        public Task<StudentDto> GetOneStudentAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<StudentDto> UpdateStudentAsync(long id, StudentDto studentDto)
        {
            throw new NotImplementedException();
        }
    }
}
