using Aptiverse.Api.Application.Resources.Dtos;
using Aptiverse.Api.Domain.Models.Resources;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Aptiverse.Api.Application.Resources.Services
{
    public class ResourceService(
        IRepository<Resource> resourceRepository,
        IMapper mapper) : IResourceService
    {
        private readonly IRepository<Resource> _resourceRepository = resourceRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<ResourceDto> CreateResourceAsync(CreateResourceDto createResourceDto)
        {
            ArgumentNullException.ThrowIfNull(createResourceDto);

            var resource = _mapper.Map<Resource>(createResourceDto);

            await _resourceRepository.AddAsync(resource);
            return _mapper.Map<ResourceDto>(resource);
        }

        public async Task<ResourceDto?> GetResourceByIdAsync(long id)
        {
            var resource = await _resourceRepository.GetAsync(
                predicate: r => r.Id == id,
                disableTracking: false);

            return _mapper.Map<ResourceDto>(resource);
        }

        public async Task<PaginatedResult<ResourceDto>> GetResourcesAsync(
            ClaimsPrincipal currentUser,
            string? search = null,
            string? subjectId = null,
            string? resourceType = null,
            string? gradeLevel = null,
            long? teacherId = null,
            long? tutorId = null,
            long? courseId = null,
            bool? isFree = null,
            bool? isApproved = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? sortBy = "CreatedAt",
            bool sortDescending = true,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<Resource, bool>>? predicate = BuildRoleBasedPredicate(currentUser);

            if (!string.IsNullOrEmpty(search) || !string.IsNullOrEmpty(subjectId) ||
                !string.IsNullOrEmpty(resourceType) || !string.IsNullOrEmpty(gradeLevel) ||
                teacherId.HasValue || tutorId.HasValue || courseId.HasValue ||
                isFree.HasValue || isApproved.HasValue || minPrice.HasValue || maxPrice.HasValue)
            {
                Expression<Func<Resource, bool>> filterPredicate = r =>
                    (string.IsNullOrEmpty(search) ||
                     r.Title.Contains(search) ||
                     r.Description.Contains(search)) &&
                    (string.IsNullOrEmpty(subjectId) || r.SubjectId == subjectId) &&
                    (string.IsNullOrEmpty(resourceType) || r.ResourceType == resourceType) &&
                    (string.IsNullOrEmpty(gradeLevel) || r.GradeLevel == gradeLevel) &&
                    (!teacherId.HasValue || r.TeacherId == teacherId.Value) &&
                    (!tutorId.HasValue || r.TutorId == tutorId.Value) &&
                    (!courseId.HasValue || r.CourseId == courseId.Value) &&
                    (!isFree.HasValue || r.IsFree == isFree.Value) &&
                    (!isApproved.HasValue || r.IsApproved == isApproved.Value) &&
                    (!minPrice.HasValue || r.Price >= minPrice.Value) &&
                    (!maxPrice.HasValue || r.Price <= maxPrice.Value);

                predicate = predicate == null
                    ? filterPredicate
                    : CombinePredicates(predicate, filterPredicate);
            }

            Func<IQueryable<Resource>, IOrderedQueryable<Resource>>? orderBy =
                GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _resourceRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy);

            var resourceDtos = _mapper.Map<List<ResourceDto>>(paginatedResult.Data);

            return new PaginatedResult<ResourceDto>(
                resourceDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<Resource, bool>>? BuildRoleBasedPredicate(ClaimsPrincipal user)
        {
            var userId = UserContextHelper.GetUserId(user);

            if (UserContextHelper.IsSuperUser(user))
            {
                return null;
            }
            else if (UserContextHelper.IsTeacher(user))
            {
                return r => r.Teacher != null && r.Teacher.UserId == userId;
            }
            else if (UserContextHelper.IsTutor(user))
            {
                return r => r.Tutor != null && r.Tutor.UserId == userId;
            }
            else if (UserContextHelper.IsAdmin(user))
            {
                return null;
            }
            else if (UserContextHelper.IsStudent(user))
            {
                return r => r.IsApproved;
            }
            else
            {
                return r => r.IsApproved && r.IsFree;
            }
        }

        private Expression<Func<Resource, bool>> CombinePredicates(
            Expression<Func<Resource, bool>> left,
            Expression<Func<Resource, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(Resource), "r");
            var leftBody = Expression.Invoke(left, parameter);
            var rightBody = Expression.Invoke(right, parameter);
            var combined = Expression.AndAlso(leftBody, rightBody);

            return Expression.Lambda<Func<Resource, bool>>(combined, parameter);
        }

        private Func<IQueryable<Resource>, IOrderedQueryable<Resource>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "title" => sortDescending
                    ? query => query.OrderByDescending(r => r.Title)
                    : query => query.OrderBy(r => r.Title),
                "price" => sortDescending
                    ? query => query.OrderByDescending(r => r.Price)
                    : query => query.OrderBy(r => r.Price),
                "downloadcount" => sortDescending
                    ? query => query.OrderByDescending(r => r.DownloadCount)
                    : query => query.OrderBy(r => r.DownloadCount),
                "rating" => sortDescending
                    ? query => query.OrderByDescending(r => r.Rating)
                    : query => query.OrderBy(r => r.Rating),
                "createdat" => sortDescending
                    ? query => query.OrderByDescending(r => r.CreatedAt)
                    : query => query.OrderBy(r => r.CreatedAt),
                _ => sortDescending
                    ? query => query.OrderByDescending(r => r.Id)
                    : query => query.OrderBy(r => r.Id)
            };
        }

        public async Task<ResourceDto> UpdateResourceAsync(long id, UpdateResourceDto updateResourceDto)
        {
            var existingResource = await _resourceRepository.GetAsync(
                predicate: r => r.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Resource with ID {id} not found");

            _mapper.Map(updateResourceDto, existingResource);

            await _resourceRepository.UpdateAsync(existingResource);
            return _mapper.Map<ResourceDto>(existingResource);
        }

        public async Task<bool> DeleteResourceAsync(long id)
        {
            var resource = await _resourceRepository.GetAsync(
                predicate: r => r.Id == id,
                disableTracking: false);

            if (resource == null)
                return false;

            await _resourceRepository.DeleteAsync(resource);
            return true;
        }

        public async Task<int> CountResourcesAsync(
            ClaimsPrincipal currentUser,
            string? subjectId = null,
            string? resourceType = null,
            bool? isFree = null,
            bool? isApproved = null)
        {
            var predicate = BuildRoleBasedPredicate(currentUser);

            Expression<Func<Resource, bool>> filterPredicate = r =>
                (string.IsNullOrEmpty(subjectId) || r.SubjectId == subjectId) &&
                (string.IsNullOrEmpty(resourceType) || r.ResourceType == resourceType) &&
                (!isFree.HasValue || r.IsFree == isFree.Value) &&
                (!isApproved.HasValue || r.IsApproved == isApproved.Value);

            predicate = predicate == null
                ? filterPredicate
                : CombinePredicates(predicate, filterPredicate);

            return await _resourceRepository.CountAsync(predicate);
        }

        public async Task<bool> ResourceExistsAsync(long id)
        {
            return await _resourceRepository.ExistsAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<ResourceDto>> GetResourcesBySubjectAsync(string subjectId)
        {
            var resources = await _resourceRepository.GetManyAsync(
                predicate: r => r.SubjectId == subjectId && r.IsApproved,
                orderBy: query => query.OrderByDescending(r => r.DownloadCount),
                disableTracking: true);

            return _mapper.Map<IEnumerable<ResourceDto>>(resources);
        }

        public async Task<IEnumerable<ResourceDto>> GetResourcesByTeacherAsync(long teacherId)
        {
            var resources = await _resourceRepository.GetManyAsync(
                predicate: r => r.TeacherId == teacherId,
                orderBy: query => query.OrderByDescending(r => r.CreatedAt),
                disableTracking: true);

            return _mapper.Map<IEnumerable<ResourceDto>>(resources);
        }

        public async Task<IEnumerable<ResourceDto>> GetResourcesByTutorAsync(long tutorId)
        {
            var resources = await _resourceRepository.GetManyAsync(
                predicate: r => r.TutorId == tutorId,
                orderBy: query => query.OrderByDescending(r => r.CreatedAt),
                disableTracking: true);

            return _mapper.Map<IEnumerable<ResourceDto>>(resources);
        }

        public async Task<IEnumerable<ResourceDto>> GetResourcesByCourseAsync(long courseId)
        {
            var resources = await _resourceRepository.GetManyAsync(
                predicate: r => r.CourseId == courseId && r.IsApproved,
                orderBy: query => query.OrderBy(r => r.CreatedAt),
                disableTracking: true);

            return _mapper.Map<IEnumerable<ResourceDto>>(resources);
        }

        public async Task<IEnumerable<ResourceDto>> GetPopularResourcesAsync(int count = 10)
        {
            var resources = await _resourceRepository.GetManyAsync(
                predicate: r => r.IsApproved,
                orderBy: query => query.OrderByDescending(r => r.DownloadCount),
                disableTracking: true);

            return _mapper.Map<IEnumerable<ResourceDto>>(resources.Take(count));
        }

        public async Task<IEnumerable<ResourceDto>> GetFreeResourcesAsync()
        {
            var resources = await _resourceRepository.GetManyAsync(
                predicate: r => r.IsApproved && r.IsFree,
                orderBy: query => query.OrderByDescending(r => r.CreatedAt),
                disableTracking: true);

            return _mapper.Map<IEnumerable<ResourceDto>>(resources);
        }

        public async Task<ResourceDto> IncrementDownloadCountAsync(long id)
        {
            var resource = await _resourceRepository.GetAsync(
                predicate: r => r.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Resource with ID {id} not found");

            resource.DownloadCount++;
            await _resourceRepository.UpdateAsync(resource);
            return _mapper.Map<ResourceDto>(resource);
        }

        public async Task<ResourceDto> UpdateResourceRatingAsync(long id, double newRating)
        {
            var resource = await _resourceRepository.GetAsync(
                predicate: r => r.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Resource with ID {id} not found");

            resource.Rating = newRating;
            await _resourceRepository.UpdateAsync(resource);
            return _mapper.Map<ResourceDto>(resource);
        }
    }
}