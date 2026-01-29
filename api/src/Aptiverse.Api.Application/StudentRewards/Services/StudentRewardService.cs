using Aptiverse.Api.Application.StudentRewards.Dtos;
using Aptiverse.Api.Domain.Models.Rewards;
using Aptiverse.Api.Domain.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Aptiverse.Api.Application.StudentRewards.Services
{
    public class StudentRewardService(
        IRepository<StudentReward> studentRewardRepository,
        IMapper mapper) : IStudentRewardService
    {
        private readonly IRepository<StudentReward> _studentRewardRepository = studentRewardRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<StudentRewardDto> CreateStudentRewardAsync(CreateStudentRewardDto createStudentRewardDto)
        {
            ArgumentNullException.ThrowIfNull(createStudentRewardDto);

            StudentReward studentReward = _mapper.Map<StudentReward>(createStudentRewardDto);
            await _studentRewardRepository.AddAsync(studentReward);
            return _mapper.Map<StudentRewardDto>(studentReward);
        }

        public async Task<StudentRewardDto?> GetStudentRewardByIdAsync(long id)
        {
            var studentReward = await _studentRewardRepository.GetAsync(
                predicate: sr => sr.Id == id,
                include: query => query
                    .Include(sr => sr.Student)
                    .Include(sr => sr.Reward)
                    .Include(sr => sr.Goal),
                disableTracking: false);

            if (studentReward == null)
                return null;

            return _mapper.Map<StudentRewardDto>(studentReward);
        }

        public async Task<PaginatedResult<StudentRewardDto>> GetStudentRewardsAsync(
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
            int pageSize = 20)
        {
            Expression<Func<StudentReward, bool>>? predicate = BuildFilterPredicate(
                studentId, rewardId, goalId, status, earnedAfter, earnedBefore, expiresBefore);

            Func<IQueryable<StudentReward>, IOrderedQueryable<StudentReward>>? orderBy = GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _studentRewardRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                include: query => query
                    .Include(sr => sr.Student)
                    .Include(sr => sr.Reward)
                    .Include(sr => sr.Goal));

            var studentRewardDtos = _mapper.Map<List<StudentRewardDto>>(paginatedResult.Data);

            return new PaginatedResult<StudentRewardDto>(
                studentRewardDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<StudentReward, bool>>? BuildFilterPredicate(
            long? studentId,
            long? rewardId,
            long? goalId,
            string? status,
            DateTime? earnedAfter,
            DateTime? earnedBefore,
            DateTime? expiresBefore)
        {
            if (!studentId.HasValue && !rewardId.HasValue && !goalId.HasValue &&
                string.IsNullOrEmpty(status) && !earnedAfter.HasValue &&
                !earnedBefore.HasValue && !expiresBefore.HasValue)
                return null;

            return sr =>
                (!studentId.HasValue || sr.StudentId == studentId.Value) &&
                (!rewardId.HasValue || sr.RewardId == rewardId.Value) &&
                (!goalId.HasValue || sr.GoalId == goalId.Value) &&
                (string.IsNullOrEmpty(status) || sr.Status == status) &&
                (!earnedAfter.HasValue || sr.EarnedAt >= earnedAfter.Value) &&
                (!earnedBefore.HasValue || sr.EarnedAt <= earnedBefore.Value) &&
                (!expiresBefore.HasValue || sr.ExpiresAt == null || sr.ExpiresAt <= expiresBefore.Value);
        }

        private Func<IQueryable<StudentReward>, IOrderedQueryable<StudentReward>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "earnedat" => sortDescending
                    ? query => query.OrderByDescending(sr => sr.EarnedAt).ThenByDescending(sr => sr.Id)
                    : query => query.OrderBy(sr => sr.EarnedAt).ThenBy(sr => sr.Id),
                "expiresat" => sortDescending
                    ? query => query.OrderByDescending(sr => sr.ExpiresAt.HasValue).ThenByDescending(sr => sr.ExpiresAt).ThenByDescending(sr => sr.Id)
                    : query => query.OrderBy(sr => sr.ExpiresAt.HasValue).ThenBy(sr => sr.ExpiresAt).ThenBy(sr => sr.Id),
                "pointsearned" => sortDescending
                    ? query => query.OrderByDescending(sr => sr.PointsEarned).ThenByDescending(sr => sr.Id)
                    : query => query.OrderBy(sr => sr.PointsEarned).ThenBy(sr => sr.Id),
                "status" => sortDescending
                    ? query => query.OrderByDescending(sr => sr.Status).ThenByDescending(sr => sr.Id)
                    : query => query.OrderBy(sr => sr.Status).ThenBy(sr => sr.Id),
                _ => sortDescending
                    ? query => query.OrderByDescending(sr => sr.Id)
                    : query => query.OrderBy(sr => sr.Id)
            };
        }

        public async Task<StudentRewardDto> UpdateStudentRewardAsync(long id, UpdateStudentRewardDto updateStudentRewardDto)
        {
            var existingStudentReward = await _studentRewardRepository.GetAsync(
                predicate: sr => sr.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"StudentReward with ID {id} not found");

            _mapper.Map(updateStudentRewardDto, existingStudentReward);
            await _studentRewardRepository.UpdateAsync(existingStudentReward);
            return _mapper.Map<StudentRewardDto>(existingStudentReward);
        }

        public async Task<bool> DeleteStudentRewardAsync(long id)
        {
            var studentReward = await _studentRewardRepository.GetAsync(
                predicate: sr => sr.Id == id,
                disableTracking: false);

            if (studentReward == null)
                return false;

            await _studentRewardRepository.DeleteAsync(studentReward);
            return true;
        }

        public async Task<int> CountStudentRewardsAsync(long? studentId = null, long? rewardId = null, string? status = null)
        {
            if (!studentId.HasValue && !rewardId.HasValue && string.IsNullOrEmpty(status))
                return await _studentRewardRepository.CountAsync();

            Expression<Func<StudentReward, bool>> predicate = sr =>
                (!studentId.HasValue || sr.StudentId == studentId.Value) &&
                (!rewardId.HasValue || sr.RewardId == rewardId.Value) &&
                (string.IsNullOrEmpty(status) || sr.Status == status);

            return await _studentRewardRepository.CountAsync(predicate);
        }

        public async Task<bool> StudentRewardExistsAsync(long id)
        {
            return await _studentRewardRepository.ExistsAsync(sr => sr.Id == id);
        }
    }
}