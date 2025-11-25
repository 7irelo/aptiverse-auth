using Aptiverse.Application.Students.Dtos;
using Aptiverse.Domain.Interfaces;
using Aptiverse.Domain.Models;
using AutoMapper;

namespace Aptiverse.Application.Students.Services
{
    public class StudentService(IRepository<User> repository, IMapper mapper) : IStudentService
    {
        private readonly IRepository<User> _repository = repository;
        private readonly IMapper _mapper = mapper;

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
