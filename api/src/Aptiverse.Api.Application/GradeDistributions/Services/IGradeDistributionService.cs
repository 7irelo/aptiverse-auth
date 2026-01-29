using Aptiverse.Api.Application.GradeDistributions.Dtos;
using Aptiverse.Api.Domain.Repositories;

namespace Aptiverse.Api.Application.GradeDistributions.Services
{
    public interface IGradeDistributionService
    {
        Task<GradeDistributionDto> CreateGradeDistributionAsync(CreateGradeDistributionDto createGradeDistributionDto);
        Task<GradeDistributionDto?> GetGradeDistributionByIdAsync(long id);
        Task<PaginatedResult<GradeDistributionDto>> GetGradeDistributionsAsync(
            long? studentSubjectId = null,
            string? grade = null,
            int? minCount = null,
            int? maxCount = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);
        Task<GradeDistributionDto> UpdateGradeDistributionAsync(long id, UpdateGradeDistributionDto updateGradeDistributionDto);
        Task<bool> DeleteGradeDistributionAsync(long id);
        Task<int> CountGradeDistributionsAsync(long? studentSubjectId = null, string? grade = null);
        Task<bool> GradeDistributionExistsAsync(long id);
    }
}