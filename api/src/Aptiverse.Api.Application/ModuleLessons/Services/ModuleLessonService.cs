using Aptiverse.Api.Application.ModuleLessons.Dtos;
using Aptiverse.Api.Domain.Models.Courses;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Aptiverse.Api.Application.ModuleLessons.Services
{
    public class ModuleLessonService(
        IRepository<ModuleLesson> lessonRepository,
        IMapper mapper) : IModuleLessonService
    {
        private readonly IRepository<ModuleLesson> _lessonRepository = lessonRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<ModuleLessonDto> CreateLessonAsync(CreateModuleLessonDto createLessonDto)
        {
            ArgumentNullException.ThrowIfNull(createLessonDto);

            var lesson = _mapper.Map<ModuleLesson>(createLessonDto);

            await _lessonRepository.AddAsync(lesson);
            return _mapper.Map<ModuleLessonDto>(lesson);
        }

        public async Task<ModuleLessonDto?> GetLessonByIdAsync(long id)
        {
            var lesson = await _lessonRepository.GetAsync(
                predicate: l => l.Id == id,
                disableTracking: false);

            if (lesson == null)
                return null;

            return _mapper.Map<ModuleLessonDto>(lesson);
        }

        public async Task<PaginatedResult<ModuleLessonDto>> GetLessonsAsync(
            ClaimsPrincipal currentUser,
            long? moduleId = null,
            string? search = null,
            bool? isFreePreview = null,
            string? sortBy = "Order",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<ModuleLesson, bool>>? predicate = BuildRoleBasedPredicate(currentUser);

            if (moduleId.HasValue || !string.IsNullOrEmpty(search) || isFreePreview.HasValue)
            {
                Expression<Func<ModuleLesson, bool>> filterPredicate = l =>
                    (!moduleId.HasValue || l.ModuleId == moduleId.Value) &&
                    (string.IsNullOrEmpty(search) || l.Title.Contains(search)) &&
                    (!isFreePreview.HasValue || l.IsFreePreview == isFreePreview.Value);

                predicate = predicate == null
                    ? filterPredicate
                    : CombinePredicates(predicate, filterPredicate);
            }

            Func<IQueryable<ModuleLesson>, IOrderedQueryable<ModuleLesson>>? orderBy =
                GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _lessonRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy);

            var lessonDtos = _mapper.Map<List<ModuleLessonDto>>(paginatedResult.Data);

            return new PaginatedResult<ModuleLessonDto>(
                lessonDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<ModuleLesson, bool>>? BuildRoleBasedPredicate(ClaimsPrincipal user)
        {
            var userId = UserContextHelper.GetUserId(user);

            if (UserContextHelper.IsSuperUser(user))
            {
                return null;
            }
            else if (UserContextHelper.IsTeacher(user))
            {
                return l => l.Module != null &&
                           l.Module.Course != null &&
                           l.Module.Course.Teacher != null &&
                           l.Module.Course.Teacher.UserId == userId;
            }
            else if (UserContextHelper.IsTutor(user))
            {
                return l => l.Module != null &&
                           l.Module.Course != null &&
                           l.Module.Course.Tutor != null &&
                           l.Module.Course.Tutor.UserId == userId;
            }
            else if (UserContextHelper.IsStudent(user))
            {
                return l => l.Module != null &&
                           l.Module.Course != null &&
                           l.Module.Course.Enrollments != null &&
                           l.Module.Course.Enrollments.Any(e => e.Student != null && e.Student.UserId == userId);
            }
            else
            {
                return l => l.IsFreePreview;
            }
        }

        private Expression<Func<ModuleLesson, bool>> CombinePredicates(
            Expression<Func<ModuleLesson, bool>> left,
            Expression<Func<ModuleLesson, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(ModuleLesson), "l");
            var leftBody = Expression.Invoke(left, parameter);
            var rightBody = Expression.Invoke(right, parameter);
            var combined = Expression.AndAlso(leftBody, rightBody);

            return Expression.Lambda<Func<ModuleLesson, bool>>(combined, parameter);
        }

        private Func<IQueryable<ModuleLesson>, IOrderedQueryable<ModuleLesson>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "title" => sortDescending
                    ? query => query.OrderByDescending(l => l.Title)
                    : query => query.OrderBy(l => l.Title),
                "order" => sortDescending
                    ? query => query.OrderByDescending(l => l.Order)
                    : query => query.OrderBy(l => l.Order),
                "durationminutes" => sortDescending
                    ? query => query.OrderByDescending(l => l.DurationMinutes)
                    : query => query.OrderBy(l => l.DurationMinutes),
                _ => sortDescending
                    ? query => query.OrderByDescending(l => l.Id)
                    : query => query.OrderBy(l => l.Id)
            };
        }

        public async Task<ModuleLessonDto> UpdateLessonAsync(long id, UpdateModuleLessonDto updateLessonDto)
        {
            var existingLesson = await _lessonRepository.GetAsync(
                predicate: l => l.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Lesson with ID {id} not found");

            _mapper.Map(updateLessonDto, existingLesson);

            await _lessonRepository.UpdateAsync(existingLesson);
            return _mapper.Map<ModuleLessonDto>(existingLesson);
        }

        public async Task<bool> DeleteLessonAsync(long id)
        {
            var lesson = await _lessonRepository.GetAsync(
                predicate: l => l.Id == id,
                disableTracking: false);

            if (lesson == null)
                return false;

            await _lessonRepository.DeleteAsync(lesson);
            return true;
        }

        public async Task<int> CountLessonsAsync(ClaimsPrincipal currentUser, long? moduleId = null, bool? isFreePreview = null)
        {
            var predicate = BuildRoleBasedPredicate(currentUser);

            Expression<Func<ModuleLesson, bool>> filterPredicate = l =>
                (!moduleId.HasValue || l.ModuleId == moduleId.Value) &&
                (!isFreePreview.HasValue || l.IsFreePreview == isFreePreview.Value);

            predicate = predicate == null
                ? filterPredicate
                : CombinePredicates(predicate, filterPredicate);

            return await _lessonRepository.CountAsync(predicate);
        }

        public async Task<bool> LessonExistsAsync(long id)
        {
            return await _lessonRepository.ExistsAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<ModuleLessonDto>> GetLessonsByModuleAsync(long moduleId)
        {
            var lessons = await _lessonRepository.GetManyAsync(
                predicate: l => l.ModuleId == moduleId,
                orderBy: query => query.OrderBy(l => l.Order),
                disableTracking: true);

            return _mapper.Map<IEnumerable<ModuleLessonDto>>(lessons);
        }

        public async Task ReorderLessonsAsync(long moduleId, List<long> lessonIdsInOrder)
        {
            if (lessonIdsInOrder == null || !lessonIdsInOrder.Any())
                return;

            var lessons = await _lessonRepository.GetManyAsync(
                predicate: l => l.ModuleId == moduleId,
                disableTracking: false);

            var lessonDict = lessons.ToDictionary(l => l.Id);

            for (int i = 0; i < lessonIdsInOrder.Count; i++)
            {
                if (lessonDict.TryGetValue(lessonIdsInOrder[i], out var lesson))
                {
                    lesson.Order = i + 1;
                }
            }

            await _lessonRepository.SaveChangesAsync();
        }

        public async Task<decimal> GetModuleTotalDurationAsync(long moduleId)
        {
            var lessons = await _lessonRepository.GetManyAsync(
                predicate: l => l.ModuleId == moduleId,
                disableTracking: true);

            return lessons.Sum(l => l.DurationMinutes);
        }
    }
}