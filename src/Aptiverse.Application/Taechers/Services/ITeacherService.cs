using Aptiverse.Application.Taechers.Dtos;

namespace Aptiverse.Application.Taechers.Services
{
    public interface ITeacherService
    {
        Task<TeacherDto> CreateTeacherAsync(TeacherDto teacherDto);
        Task<TeacherDto> GetOneTeacherAsync(long id);
        Task<IEnumerable<TeacherDto>> GetManyTeachersAsync(string filter);
        Task<TeacherDto> UpdateTeachersAsync(long id, TeacherDto teacherDto);
        Task DeleteTeacherAsync(long id);
    }
}
