using Aptiverse.Api.Application.AssessmentBreakdowns.Dtos;
using Aptiverse.Api.Domain.Repositories;
using System.Security.Claims;

namespace Aptiverse.Api.Application.AssessmentBreakdowns.Services
{
    public interface IAssessmentBreakdownService
    {
        Task<AssessmentBreakdownDto> CreateAssessmentBreakdownAsync(CreateAssessmentBreakdownDto createDto);
        Task<AssessmentBreakdownDto?> GetAssessmentBreakdownByIdAsync(long id);
        Task<PaginatedResult<AssessmentBreakdownDto>> GetAssessmentBreakdownsAsync(
            ClaimsPrincipal currentUser,
            long? studentSubjectId = null,
            string? assessmentType = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);
        Task<AssessmentBreakdownDto> UpdateAssessmentBreakdownAsync(long id, UpdateAssessmentBreakdownDto updateDto);
        Task<bool> DeleteAssessmentBreakdownAsync(long id);
        Task<IEnumerable<AssessmentBreakdownDto>> GetBreakdownsByStudentSubjectAsync(long studentSubjectId);
        Task RecalculateBreakdownAsync(long studentSubjectId);
        Task RecalculateAllBreakdownsAsync(); // Add this new method
    }
}