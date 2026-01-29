using Aptiverse.Api.Application.Tutors.Dtos;
using Aptiverse.Api.Domain.Repositories;

namespace Aptiverse.Api.Application.Tutors.Services
{
    public interface ITutorService
    {
        Task<TutorDto> CreateTutorAsync(CreateTutorDto createTutorDto);
        Task<TutorDto?> GetTutorByIdAsync(long id);
        Task<PaginatedResult<TutorDto>> GetTutorsAsync(
            string? search = null,
            string? specialization = null,
            string? teachingStyle = null,
            bool? isVerified = null,
            decimal? minHourlyRate = null,
            decimal? maxHourlyRate = null,
            double? minRating = null,
            double? maxRating = null,
            int? minYearsOfExperience = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);
        Task<TutorDto> UpdateTutorAsync(long id, UpdateTutorDto updateTutorDto);
        Task<bool> DeleteTutorAsync(long id);
        Task<int> CountTutorsAsync(string? specialization = null, bool? isVerified = null);
        Task<bool> TutorExistsAsync(long id);
    }
}