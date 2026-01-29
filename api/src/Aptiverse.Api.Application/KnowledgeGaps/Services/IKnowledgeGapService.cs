using Aptiverse.Api.Application.KnowledgeGaps.Dtos;
using Aptiverse.Api.Domain.Repositories;

namespace Aptiverse.Api.Application.KnowledgeGaps.Services
{
    public interface IKnowledgeGapService
    {
        Task<KnowledgeGapDto> CreateKnowledgeGapAsync(CreateKnowledgeGapDto createKnowledgeGapDto);
        Task<KnowledgeGapDto?> GetKnowledgeGapByIdAsync(long id);
        Task<PaginatedResult<KnowledgeGapDto>> GetKnowledgeGapsAsync(
            long? studentSubjectId = null,
            string? severity = null,
            string? search = null,
            DateTime? lastTestedAfter = null,
            DateTime? lastTestedBefore = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);
        Task<KnowledgeGapDto> UpdateKnowledgeGapAsync(long id, UpdateKnowledgeGapDto updateKnowledgeGapDto);
        Task<bool> DeleteKnowledgeGapAsync(long id);
        Task<int> CountKnowledgeGapsAsync(long? studentSubjectId = null, string? severity = null);
        Task<bool> KnowledgeGapExistsAsync(long id);
    }
}