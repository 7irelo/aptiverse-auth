using Aptiverse.Api.Application.Tutors.Dtos;
using Aptiverse.Api.Domain.Models.Tutors;
using Aptiverse.Api.Domain.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Aptiverse.Api.Application.Tutors.Services
{
    public class TutorService(
        IRepository<Tutor> tutorRepository,
        IMapper mapper) : ITutorService
    {
        private readonly IRepository<Tutor> _tutorRepository = tutorRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<TutorDto> CreateTutorAsync(CreateTutorDto createTutorDto)
        {
            ArgumentNullException.ThrowIfNull(createTutorDto);

            Tutor tutor = _mapper.Map<Tutor>(createTutorDto);
            tutor.CreatedAt = DateTime.UtcNow;
            await _tutorRepository.AddAsync(tutor);
            return _mapper.Map<TutorDto>(tutor);
        }

        public async Task<TutorDto?> GetTutorByIdAsync(long id)
        {
            var tutor = await _tutorRepository.GetAsync(
                predicate: t => t.Id == id,
                include: query => query
                    .Include(t => t.TutorSubjects)
                    .Include(t => t.TutorStudents)
                    .Include(t => t.Courses)
                    .Include(t => t.Resources)
                    .Include(t => t.Availabilities),
                disableTracking: false);

            if (tutor == null)
                return null;

            return _mapper.Map<TutorDto>(tutor);
        }

        public async Task<PaginatedResult<TutorDto>> GetTutorsAsync(
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
            int pageSize = 20)
        {
            Expression<Func<Tutor, bool>>? predicate = BuildFilterPredicate(
                search, specialization, teachingStyle, isVerified, minHourlyRate, maxHourlyRate, minRating, maxRating, minYearsOfExperience);

            Func<IQueryable<Tutor>, IOrderedQueryable<Tutor>>? orderBy = GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _tutorRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                include: query => query
                    .Include(t => t.TutorSubjects)
                    .Include(t => t.TutorStudents)
                    .Include(t => t.Courses)
                    .Include(t => t.Resources)
                    .Include(t => t.Availabilities));

            var tutorDtos = _mapper.Map<List<TutorDto>>(paginatedResult.Data);

            return new PaginatedResult<TutorDto>(
                tutorDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<Tutor, bool>>? BuildFilterPredicate(
            string? search,
            string? specialization,
            string? teachingStyle,
            bool? isVerified,
            decimal? minHourlyRate,
            decimal? maxHourlyRate,
            double? minRating,
            double? maxRating,
            int? minYearsOfExperience)
        {
            if (string.IsNullOrEmpty(search) && string.IsNullOrEmpty(specialization) &&
                string.IsNullOrEmpty(teachingStyle) && !isVerified.HasValue &&
                !minHourlyRate.HasValue && !maxHourlyRate.HasValue &&
                !minRating.HasValue && !maxRating.HasValue && !minYearsOfExperience.HasValue)
                return null;

            return t =>
                (string.IsNullOrEmpty(search) || t.UserId.Contains(search) || t.Qualification.Contains(search) || t.Bio.Contains(search)) &&
                (string.IsNullOrEmpty(specialization) || t.Specialization == specialization) &&
                (string.IsNullOrEmpty(teachingStyle) || t.TeachingStyle == teachingStyle) &&
                (!isVerified.HasValue || t.IsVerified == isVerified.Value) &&
                (!minHourlyRate.HasValue || t.HourlyRate >= minHourlyRate.Value) &&
                (!maxHourlyRate.HasValue || t.HourlyRate <= maxHourlyRate.Value) &&
                (!minRating.HasValue || t.Rating >= minRating.Value) &&
                (!maxRating.HasValue || t.Rating <= maxRating.Value) &&
                (!minYearsOfExperience.HasValue || t.YearsOfExperience >= minYearsOfExperience.Value);
        }

        private Func<IQueryable<Tutor>, IOrderedQueryable<Tutor>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "userid" => sortDescending
                    ? query => query.OrderByDescending(t => t.UserId).ThenByDescending(t => t.Id)
                    : query => query.OrderBy(t => t.UserId).ThenBy(t => t.Id),
                "specialization" => sortDescending
                    ? query => query.OrderByDescending(t => t.Specialization).ThenByDescending(t => t.Id)
                    : query => query.OrderBy(t => t.Specialization).ThenBy(t => t.Id),
                "rating" => sortDescending
                    ? query => query.OrderByDescending(t => t.Rating).ThenByDescending(t => t.Id)
                    : query => query.OrderBy(t => t.Rating).ThenBy(t => t.Id),
                "hourlyrate" => sortDescending
                    ? query => query.OrderByDescending(t => t.HourlyRate).ThenByDescending(t => t.Id)
                    : query => query.OrderBy(t => t.HourlyRate).ThenBy(t => t.Id),
                "yearsofexperience" => sortDescending
                    ? query => query.OrderByDescending(t => t.YearsOfExperience).ThenByDescending(t => t.Id)
                    : query => query.OrderBy(t => t.YearsOfExperience).ThenBy(t => t.Id),
                "teachingstyle" => sortDescending
                    ? query => query.OrderByDescending(t => t.TeachingStyle).ThenByDescending(t => t.Id)
                    : query => query.OrderBy(t => t.TeachingStyle).ThenBy(t => t.Id),
                "createdat" => sortDescending
                    ? query => query.OrderByDescending(t => t.CreatedAt).ThenByDescending(t => t.Id)
                    : query => query.OrderBy(t => t.CreatedAt).ThenBy(t => t.Id),
                _ => sortDescending
                    ? query => query.OrderByDescending(t => t.Id)
                    : query => query.OrderBy(t => t.Id)
            };
        }

        public async Task<TutorDto> UpdateTutorAsync(long id, UpdateTutorDto updateTutorDto)
        {
            var existingTutor = await _tutorRepository.GetAsync(
                predicate: t => t.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Tutor with ID {id} not found");

            _mapper.Map(updateTutorDto, existingTutor);
            await _tutorRepository.UpdateAsync(existingTutor);
            return _mapper.Map<TutorDto>(existingTutor);
        }

        public async Task<bool> DeleteTutorAsync(long id)
        {
            var tutor = await _tutorRepository.GetAsync(
                predicate: t => t.Id == id,
                disableTracking: false);

            if (tutor == null)
                return false;

            await _tutorRepository.DeleteAsync(tutor);
            return true;
        }

        public async Task<int> CountTutorsAsync(string? specialization = null, bool? isVerified = null)
        {
            if (string.IsNullOrEmpty(specialization) && !isVerified.HasValue)
                return await _tutorRepository.CountAsync();

            Expression<Func<Tutor, bool>> predicate = t =>
                (string.IsNullOrEmpty(specialization) || t.Specialization == specialization) &&
                (!isVerified.HasValue || t.IsVerified == isVerified.Value);

            return await _tutorRepository.CountAsync(predicate);
        }

        public async Task<bool> TutorExistsAsync(long id)
        {
            return await _tutorRepository.ExistsAsync(t => t.Id == id);
        }
    }
}