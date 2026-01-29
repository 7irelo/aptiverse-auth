using Aptiverse.Api.Application.TutorAvailabilities.Dtos;
using Aptiverse.Api.Domain.Models.Tutors;
using Aptiverse.Api.Domain.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Aptiverse.Api.Application.TutorAvailabilities.Services
{
    public class TutorAvailabilityService(
        IRepository<TutorAvailability> tutorAvailabilityRepository,
        IMapper mapper) : ITutorAvailabilityService
    {
        private readonly IRepository<TutorAvailability> _tutorAvailabilityRepository = tutorAvailabilityRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<TutorAvailabilityDto> CreateTutorAvailabilityAsync(CreateTutorAvailabilityDto createTutorAvailabilityDto)
        {
            ArgumentNullException.ThrowIfNull(createTutorAvailabilityDto);

            TutorAvailability tutorAvailability = _mapper.Map<TutorAvailability>(createTutorAvailabilityDto);
            await _tutorAvailabilityRepository.AddAsync(tutorAvailability);
            return _mapper.Map<TutorAvailabilityDto>(tutorAvailability);
        }

        public async Task<TutorAvailabilityDto?> GetTutorAvailabilityByIdAsync(long id)
        {
            var tutorAvailability = await _tutorAvailabilityRepository.GetAsync(
                predicate: ta => ta.Id == id,
                include: query => query.Include(ta => ta.Tutor),
                disableTracking: false);

            if (tutorAvailability == null)
                return null;

            return _mapper.Map<TutorAvailabilityDto>(tutorAvailability);
        }

        public async Task<PaginatedResult<TutorAvailabilityDto>> GetTutorAvailabilitiesAsync(
            long? tutorId = null,
            DayOfWeek? dayOfWeek = null,
            bool? isAvailable = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<TutorAvailability, bool>>? predicate = BuildFilterPredicate(tutorId, dayOfWeek, isAvailable);
            Func<IQueryable<TutorAvailability>, IOrderedQueryable<TutorAvailability>>? orderBy = GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _tutorAvailabilityRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                include: query => query.Include(ta => ta.Tutor));

            var tutorAvailabilityDtos = _mapper.Map<List<TutorAvailabilityDto>>(paginatedResult.Data);

            return new PaginatedResult<TutorAvailabilityDto>(
                tutorAvailabilityDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<TutorAvailability, bool>>? BuildFilterPredicate(
            long? tutorId,
            DayOfWeek? dayOfWeek,
            bool? isAvailable)
        {
            if (!tutorId.HasValue && !dayOfWeek.HasValue && !isAvailable.HasValue)
                return null;

            return ta =>
                (!tutorId.HasValue || ta.TutorId == tutorId.Value) &&
                (!dayOfWeek.HasValue || ta.DayOfWeek == dayOfWeek.Value) &&
                (!isAvailable.HasValue || ta.IsAvailable == isAvailable.Value);
        }

        private Func<IQueryable<TutorAvailability>, IOrderedQueryable<TutorAvailability>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "dayofweek" => sortDescending
                    ? query => query.OrderByDescending(ta => ta.DayOfWeek).ThenByDescending(ta => ta.Id)
                    : query => query.OrderBy(ta => ta.DayOfWeek).ThenBy(ta => ta.Id),
                "starttime" => sortDescending
                    ? query => query.OrderByDescending(ta => ta.StartTime).ThenByDescending(ta => ta.Id)
                    : query => query.OrderBy(ta => ta.StartTime).ThenBy(ta => ta.Id),
                "endtime" => sortDescending
                    ? query => query.OrderByDescending(ta => ta.EndTime).ThenByDescending(ta => ta.Id)
                    : query => query.OrderBy(ta => ta.EndTime).ThenBy(ta => ta.Id),
                "tutorid" => sortDescending
                    ? query => query.OrderByDescending(ta => ta.TutorId).ThenByDescending(ta => ta.Id)
                    : query => query.OrderBy(ta => ta.TutorId).ThenBy(ta => ta.Id),
                _ => sortDescending
                    ? query => query.OrderByDescending(ta => ta.Id)
                    : query => query.OrderBy(ta => ta.Id)
            };
        }

        public async Task<TutorAvailabilityDto> UpdateTutorAvailabilityAsync(long id, UpdateTutorAvailabilityDto updateTutorAvailabilityDto)
        {
            var existingTutorAvailability = await _tutorAvailabilityRepository.GetAsync(
                predicate: ta => ta.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"TutorAvailability with ID {id} not found");

            _mapper.Map(updateTutorAvailabilityDto, existingTutorAvailability);
            await _tutorAvailabilityRepository.UpdateAsync(existingTutorAvailability);
            return _mapper.Map<TutorAvailabilityDto>(existingTutorAvailability);
        }

        public async Task<bool> DeleteTutorAvailabilityAsync(long id)
        {
            var tutorAvailability = await _tutorAvailabilityRepository.GetAsync(
                predicate: ta => ta.Id == id,
                disableTracking: false);

            if (tutorAvailability == null)
                return false;

            await _tutorAvailabilityRepository.DeleteAsync(tutorAvailability);
            return true;
        }

        public async Task<int> CountTutorAvailabilitiesAsync(long? tutorId = null, bool? isAvailable = null)
        {
            if (!tutorId.HasValue && !isAvailable.HasValue)
                return await _tutorAvailabilityRepository.CountAsync();

            Expression<Func<TutorAvailability, bool>> predicate = ta =>
                (!tutorId.HasValue || ta.TutorId == tutorId.Value) &&
                (!isAvailable.HasValue || ta.IsAvailable == isAvailable.Value);

            return await _tutorAvailabilityRepository.CountAsync(predicate);
        }

        public async Task<bool> TutorAvailabilityExistsAsync(long id)
        {
            return await _tutorAvailabilityRepository.ExistsAsync(ta => ta.Id == id);
        }
    }
}