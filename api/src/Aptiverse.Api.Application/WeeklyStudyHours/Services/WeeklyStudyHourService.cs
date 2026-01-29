using Aptiverse.Api.Application.WeeklyStudyHours.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using Aptiverse.Api.Domain.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Aptiverse.Api.Application.WeeklyStudyHours.Services
{
    public class WeeklyStudyHourService(
        IRepository<WeeklyStudyHour> weeklyStudyHourRepository,
        IMapper mapper) : IWeeklyStudyHourService
    {
        private readonly IRepository<WeeklyStudyHour> _weeklyStudyHourRepository = weeklyStudyHourRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<WeeklyStudyHourDto> CreateWeeklyStudyHourAsync(CreateWeeklyStudyHourDto createWeeklyStudyHourDto)
        {
            ArgumentNullException.ThrowIfNull(createWeeklyStudyHourDto);

            WeeklyStudyHour weeklyStudyHour = _mapper.Map<WeeklyStudyHour>(createWeeklyStudyHourDto);
            await _weeklyStudyHourRepository.AddAsync(weeklyStudyHour);
            return _mapper.Map<WeeklyStudyHourDto>(weeklyStudyHour);
        }

        public async Task<WeeklyStudyHourDto?> GetWeeklyStudyHourByIdAsync(long id)
        {
            var weeklyStudyHour = await _weeklyStudyHourRepository.GetAsync(
                predicate: wsh => wsh.Id == id,
                include: query => query.Include(wsh => wsh.StudentSubject),
                disableTracking: false);

            if (weeklyStudyHour == null)
                return null;

            return _mapper.Map<WeeklyStudyHourDto>(weeklyStudyHour);
        }

        public async Task<PaginatedResult<WeeklyStudyHourDto>> GetWeeklyStudyHoursAsync(
            long? studentSubjectId = null,
            int? weekNumber = null,
            int? minHours = null,
            int? maxHours = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<WeeklyStudyHour, bool>>? predicate = BuildFilterPredicate(
                studentSubjectId, weekNumber, minHours, maxHours);

            Func<IQueryable<WeeklyStudyHour>, IOrderedQueryable<WeeklyStudyHour>>? orderBy = GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _weeklyStudyHourRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                include: query => query.Include(wsh => wsh.StudentSubject));

            var weeklyStudyHourDtos = _mapper.Map<List<WeeklyStudyHourDto>>(paginatedResult.Data);

            return new PaginatedResult<WeeklyStudyHourDto>(
                weeklyStudyHourDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<WeeklyStudyHour, bool>>? BuildFilterPredicate(
            long? studentSubjectId,
            int? weekNumber,
            int? minHours,
            int? maxHours)
        {
            if (!studentSubjectId.HasValue && !weekNumber.HasValue &&
                !minHours.HasValue && !maxHours.HasValue)
                return null;

            return wsh =>
                (!studentSubjectId.HasValue || wsh.StudentSubjectId == studentSubjectId.Value) &&
                (!weekNumber.HasValue || wsh.WeekNumber == weekNumber.Value) &&
                (!minHours.HasValue || wsh.Hours >= minHours.Value) &&
                (!maxHours.HasValue || wsh.Hours <= maxHours.Value);
        }

        private Func<IQueryable<WeeklyStudyHour>, IOrderedQueryable<WeeklyStudyHour>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "weeknumber" => sortDescending
                    ? query => query.OrderByDescending(wsh => wsh.WeekNumber).ThenByDescending(wsh => wsh.Id)
                    : query => query.OrderBy(wsh => wsh.WeekNumber).ThenBy(wsh => wsh.Id),
                "hours" => sortDescending
                    ? query => query.OrderByDescending(wsh => wsh.Hours).ThenByDescending(wsh => wsh.Id)
                    : query => query.OrderBy(wsh => wsh.Hours).ThenBy(wsh => wsh.Id),
                _ => sortDescending
                    ? query => query.OrderByDescending(wsh => wsh.Id)
                    : query => query.OrderBy(wsh => wsh.Id)
            };
        }

        public async Task<WeeklyStudyHourDto> UpdateWeeklyStudyHourAsync(long id, UpdateWeeklyStudyHourDto updateWeeklyStudyHourDto)
        {
            var existingWeeklyStudyHour = await _weeklyStudyHourRepository.GetAsync(
                predicate: wsh => wsh.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"WeeklyStudyHour with ID {id} not found");

            _mapper.Map(updateWeeklyStudyHourDto, existingWeeklyStudyHour);
            await _weeklyStudyHourRepository.UpdateAsync(existingWeeklyStudyHour);
            return _mapper.Map<WeeklyStudyHourDto>(existingWeeklyStudyHour);
        }

        public async Task<bool> DeleteWeeklyStudyHourAsync(long id)
        {
            var weeklyStudyHour = await _weeklyStudyHourRepository.GetAsync(
                predicate: wsh => wsh.Id == id,
                disableTracking: false);

            if (weeklyStudyHour == null)
                return false;

            await _weeklyStudyHourRepository.DeleteAsync(weeklyStudyHour);
            return true;
        }

        public async Task<int> CountWeeklyStudyHoursAsync(long? studentSubjectId = null, int? weekNumber = null)
        {
            if (!studentSubjectId.HasValue && !weekNumber.HasValue)
                return await _weeklyStudyHourRepository.CountAsync();

            Expression<Func<WeeklyStudyHour, bool>> predicate = wsh =>
                (!studentSubjectId.HasValue || wsh.StudentSubjectId == studentSubjectId.Value) &&
                (!weekNumber.HasValue || wsh.WeekNumber == weekNumber.Value);

            return await _weeklyStudyHourRepository.CountAsync(predicate);
        }

        public async Task<bool> WeeklyStudyHourExistsAsync(long id)
        {
            return await _weeklyStudyHourRepository.ExistsAsync(wsh => wsh.Id == id);
        }
    }
}