using Aptiverse.Api.Application.TutorAvailabilities.Dtos;
using Aptiverse.Api.Domain.Repositories;

namespace Aptiverse.Api.Application.TutorAvailabilities.Services
{
    public interface ITutorAvailabilityService
    {
        Task<TutorAvailabilityDto> CreateTutorAvailabilityAsync(CreateTutorAvailabilityDto createTutorAvailabilityDto);
        Task<TutorAvailabilityDto?> GetTutorAvailabilityByIdAsync(long id);
        Task<PaginatedResult<TutorAvailabilityDto>> GetTutorAvailabilitiesAsync(
            long? tutorId = null,
            DayOfWeek? dayOfWeek = null,
            bool? isAvailable = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);
        Task<TutorAvailabilityDto> UpdateTutorAvailabilityAsync(long id, UpdateTutorAvailabilityDto updateTutorAvailabilityDto);
        Task<bool> DeleteTutorAvailabilityAsync(long id);
        Task<int> CountTutorAvailabilitiesAsync(long? tutorId = null, bool? isAvailable = null);
        Task<bool> TutorAvailabilityExistsAsync(long id);
    }
}