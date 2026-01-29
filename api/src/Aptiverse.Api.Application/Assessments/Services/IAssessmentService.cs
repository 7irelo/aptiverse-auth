using Aptiverse.Api.Application.Assessments.Dtos;
using Aptiverse.Api.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Aptiverse.Api.Application.Assessments.Services
{
    public interface IAssessmentService
    {
        Task<AssessmentDto> CreateAssessmentAsync(CreateAssessmentDto createDto);
        Task<AssessmentDto?> GetAssessmentByIdAsync(long id);
        Task<PaginatedResult<AssessmentDto>> GetAssessmentsAsync(
            ClaimsPrincipal currentUser,
            long? studentId = null,
            string? subjectId = null,
            string? type = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? sortBy = "DateTaken",
            bool sortDescending = true,
            int page = 1,
            int pageSize = 20);
        Task<AssessmentDto> UpdateAssessmentAsync(long id, UpdateAssessmentDto updateDto);
        Task<bool> DeleteAssessmentAsync(long id);
        Task<double> GetStudentAverageScoreAsync(long studentId, string? subjectId = null);
        Task<IEnumerable<AssessmentSummaryDto>> GetAssessmentSummaryAsync(long studentId);
        Task<IEnumerable<AssessmentTrendDto>> GetAssessmentTrendAsync(long studentId, string subjectId, string type);
    }
}
