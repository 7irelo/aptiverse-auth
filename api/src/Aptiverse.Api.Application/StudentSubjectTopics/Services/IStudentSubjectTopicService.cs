using Aptiverse.Api.Application.StudentSubjectTopics.Dtos;
using Aptiverse.Api.Domain.Repositories;

namespace Aptiverse.Api.Application.StudentSubjectTopics.Services
{
    public interface IStudentSubjectTopicService
    {
        Task<StudentSubjectTopicDto> CreateStudentSubjectTopicAsync(CreateStudentSubjectTopicDto createStudentSubjectTopicDto);
        Task<StudentSubjectTopicDto?> GetStudentSubjectTopicByIdAsync(long id);
        Task<PaginatedResult<StudentSubjectTopicDto>> GetStudentSubjectTopicsAsync(
            long? studentSubjectId = null,
            long? topicId = null,
            double? minScore = null,
            double? maxScore = null,
            string? trend = null,
            DateTime? lastTestedAfter = null,
            DateTime? lastTestedBefore = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);
        Task<StudentSubjectTopicDto> UpdateStudentSubjectTopicAsync(long id, UpdateStudentSubjectTopicDto updateStudentSubjectTopicDto);
        Task<bool> DeleteStudentSubjectTopicAsync(long id);
        Task<int> CountStudentSubjectTopicsAsync(long? studentSubjectId = null, long? topicId = null, string? trend = null);
        Task<bool> StudentSubjectTopicExistsAsync(long id);
        Task<bool> StudentSubjectTopicExistsForStudentSubjectAndTopicAsync(long studentSubjectId, long topicId);
    }
}