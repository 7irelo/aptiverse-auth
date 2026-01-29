using Aptiverse.Api.Application.StudentPointss.Dtos;
using Aptiverse.Api.Domain.Models.Rewards;
using Aptiverse.Api.Domain.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Aptiverse.Api.Application.StudentPointss.Services
{
    public class StudentPointsService(
        IRepository<StudentPoints> studentPointsRepository,
        IMapper mapper) : IStudentPointsService
    {
        private readonly IRepository<StudentPoints> _studentPointsRepository = studentPointsRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<StudentPointsDto> CreateStudentPointsAsync(CreateStudentPointsDto createStudentPointsDto)
        {
            ArgumentNullException.ThrowIfNull(createStudentPointsDto);

            StudentPoints studentPoints = _mapper.Map<StudentPoints>(createStudentPointsDto);
            studentPoints.LastUpdated = DateTime.UtcNow;

            await _studentPointsRepository.AddAsync(studentPoints);
            return _mapper.Map<StudentPointsDto>(studentPoints);
        }

        public async Task<StudentPointsDto?> GetStudentPointsByIdAsync(long id)
        {
            var studentPoints = await _studentPointsRepository.GetAsync(
                predicate: sp => sp.Id == id,
                include: query => query
                    .Include(sp => sp.Student)
                    .Include(sp => sp.Transactions),
                disableTracking: false);

            if (studentPoints == null)
                return null;

            return _mapper.Map<StudentPointsDto>(studentPoints);
        }

        public async Task<StudentPointsDto?> GetStudentPointsByStudentIdAsync(long studentId)
        {
            var studentPoints = await _studentPointsRepository.GetAsync(
                predicate: sp => sp.StudentId == studentId,
                include: query => query
                    .Include(sp => sp.Student)
                    .Include(sp => sp.Transactions),
                disableTracking: false);

            if (studentPoints == null)
                return null;

            return _mapper.Map<StudentPointsDto>(studentPoints);
        }

        public async Task<PaginatedResult<StudentPointsDto>> GetStudentPointsAsync(
            long? studentId = null,
            int? minLevel = null,
            int? maxLevel = null,
            string? rank = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<StudentPoints, bool>>? predicate = BuildFilterPredicate(studentId, minLevel, maxLevel, rank);
            Func<IQueryable<StudentPoints>, IOrderedQueryable<StudentPoints>>? orderBy = GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _studentPointsRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                include: query => query
                    .Include(sp => sp.Student)
                    .Include(sp => sp.Transactions));

            var studentPointsDtos = _mapper.Map<List<StudentPointsDto>>(paginatedResult.Data);

            return new PaginatedResult<StudentPointsDto>(
                studentPointsDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<StudentPoints, bool>>? BuildFilterPredicate(
            long? studentId,
            int? minLevel,
            int? maxLevel,
            string? rank)
        {
            if (!studentId.HasValue && !minLevel.HasValue && !maxLevel.HasValue && string.IsNullOrEmpty(rank))
                return null;

            return sp =>
                (!studentId.HasValue || sp.StudentId == studentId.Value) &&
                (!minLevel.HasValue || sp.Level >= minLevel.Value) &&
                (!maxLevel.HasValue || sp.Level <= maxLevel.Value) &&
                (string.IsNullOrEmpty(rank) || sp.CurrentRank == rank);
        }

        private Func<IQueryable<StudentPoints>, IOrderedQueryable<StudentPoints>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "totalpoints" => sortDescending
                    ? query => query.OrderByDescending(sp => sp.TotalPoints).ThenByDescending(sp => sp.Id)
                    : query => query.OrderBy(sp => sp.TotalPoints).ThenBy(sp => sp.Id),
                "availablepoints" => sortDescending
                    ? query => query.OrderByDescending(sp => sp.AvailablePoints).ThenByDescending(sp => sp.Id)
                    : query => query.OrderBy(sp => sp.AvailablePoints).ThenBy(sp => sp.Id),
                "level" => sortDescending
                    ? query => query.OrderByDescending(sp => sp.Level).ThenByDescending(sp => sp.Id)
                    : query => query.OrderBy(sp => sp.Level).ThenBy(sp => sp.Id),
                "lastupdated" => sortDescending
                    ? query => query.OrderByDescending(sp => sp.LastUpdated).ThenByDescending(sp => sp.Id)
                    : query => query.OrderBy(sp => sp.LastUpdated).ThenBy(sp => sp.Id),
                _ => sortDescending
                    ? query => query.OrderByDescending(sp => sp.Id)
                    : query => query.OrderBy(sp => sp.Id)
            };
        }

        public async Task<StudentPointsDto> UpdateStudentPointsAsync(long id, UpdateStudentPointsDto updateStudentPointsDto)
        {
            var existingStudentPoints = await _studentPointsRepository.GetAsync(
                predicate: sp => sp.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"StudentPoints with ID {id} not found");

            _mapper.Map(updateStudentPointsDto, existingStudentPoints);
            existingStudentPoints.LastUpdated = DateTime.UtcNow;

            await _studentPointsRepository.UpdateAsync(existingStudentPoints);
            return _mapper.Map<StudentPointsDto>(existingStudentPoints);
        }

        public async Task<bool> DeleteStudentPointsAsync(long id)
        {
            var studentPoints = await _studentPointsRepository.GetAsync(
                predicate: sp => sp.Id == id,
                disableTracking: false);

            if (studentPoints == null)
                return false;

            await _studentPointsRepository.DeleteAsync(studentPoints);
            return true;
        }

        public async Task<int> CountStudentPointsAsync(long? studentId = null, int? minLevel = null, string? rank = null)
        {
            if (!studentId.HasValue && !minLevel.HasValue && string.IsNullOrEmpty(rank))
                return await _studentPointsRepository.CountAsync();

            Expression<Func<StudentPoints, bool>> predicate = sp =>
                (!studentId.HasValue || sp.StudentId == studentId.Value) &&
                (!minLevel.HasValue || sp.Level >= minLevel.Value) &&
                (string.IsNullOrEmpty(rank) || sp.CurrentRank == rank);

            return await _studentPointsRepository.CountAsync(predicate);
        }

        public async Task<bool> StudentPointsExistsAsync(long id)
        {
            return await _studentPointsRepository.ExistsAsync(sp => sp.Id == id);
        }

        public async Task<bool> StudentPointsExistsForStudentAsync(long studentId)
        {
            return await _studentPointsRepository.ExistsAsync(sp => sp.StudentId == studentId);
        }
    }
}