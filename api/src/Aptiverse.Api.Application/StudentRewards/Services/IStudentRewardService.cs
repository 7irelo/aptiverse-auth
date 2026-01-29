using Aptiverse.Api.Application.StudentRewards.Dtos;
using Aptiverse.Api.Domain.Repositories;

namespace Aptiverse.Api.Application.StudentRewards.Services
{
    public interface IStudentRewardService
    {
        Task<StudentRewardDto> CreateStudentRewardAsync(CreateStudentRewardDto createStudentRewardDto);
        Task<StudentRewardDto?> GetStudentRewardByIdAsync(long id);
        Task<PaginatedResult<StudentRewardDto>> GetStudentRewardsAsync(
            long? studentId = null,
            long? rewardId = null,
            long? goalId = null,
            string? status = null,
            DateTime? earnedAfter = null,
            DateTime? earnedBefore = null,
            DateTime? expiresBefore = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20);
        Task<StudentRewardDto> UpdateStudentRewardAsync(long id, UpdateStudentRewardDto updateStudentRewardDto);
        Task<bool> DeleteStudentRewardAsync(long id);
        Task<int> CountStudentRewardsAsync(long? studentId = null, long? rewardId = null, string? status = null);
        Task<bool> StudentRewardExistsAsync(long id);
    }
}