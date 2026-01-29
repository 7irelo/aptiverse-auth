using Aptiverse.Api.Application.ResourceDownloads.Dtos;
using Aptiverse.Api.Domain.Models.Resources;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Aptiverse.Api.Application.ResourceDownloads.Services
{
    public class ResourceDownloadService(
        IRepository<ResourceDownload> resourceDownloadRepository,
        IMapper mapper) : IResourceDownloadService
    {
        private readonly IRepository<ResourceDownload> _resourceDownloadRepository = resourceDownloadRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<ResourceDownloadDto> CreateResourceDownloadAsync(CreateResourceDownloadDto createResourceDownloadDto)
        {
            ArgumentNullException.ThrowIfNull(createResourceDownloadDto);

            var resourceDownload = _mapper.Map<ResourceDownload>(createResourceDownloadDto);

            await _resourceDownloadRepository.AddAsync(resourceDownload);
            return _mapper.Map<ResourceDownloadDto>(resourceDownload);
        }

        public async Task<ResourceDownloadDto?> GetResourceDownloadByIdAsync(long id)
        {
            var resourceDownload = await _resourceDownloadRepository.GetAsync(
                predicate: rd => rd.Id == id,
                disableTracking: false);

            return _mapper.Map<ResourceDownloadDto>(resourceDownload);
        }

        public async Task<PaginatedResult<ResourceDownloadDto>> GetResourceDownloadsAsync(
            ClaimsPrincipal currentUser,
            long? resourceId = null,
            long? studentId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? sortBy = "DownloadedAt",
            bool sortDescending = true,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<ResourceDownload, bool>>? predicate = BuildRoleBasedPredicate(currentUser);

            if (resourceId.HasValue || studentId.HasValue || fromDate.HasValue || toDate.HasValue)
            {
                Expression<Func<ResourceDownload, bool>> filterPredicate = rd =>
                    (!resourceId.HasValue || rd.ResourceId == resourceId.Value) &&
                    (!studentId.HasValue || rd.StudentId == studentId.Value) &&
                    (!fromDate.HasValue || rd.DownloadedAt >= fromDate.Value) &&
                    (!toDate.HasValue || rd.DownloadedAt <= toDate.Value);

                predicate = predicate == null
                    ? filterPredicate
                    : CombinePredicates(predicate, filterPredicate);
            }

            Func<IQueryable<ResourceDownload>, IOrderedQueryable<ResourceDownload>>? orderBy =
                GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _resourceDownloadRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy);

            var resourceDownloadDtos = _mapper.Map<List<ResourceDownloadDto>>(paginatedResult.Data);

            return new PaginatedResult<ResourceDownloadDto>(
                resourceDownloadDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<ResourceDownload, bool>>? BuildRoleBasedPredicate(ClaimsPrincipal user)
        {
            var userId = UserContextHelper.GetUserId(user);

            if (UserContextHelper.IsSuperUser(user))
            {
                return null;
            }
            else if (UserContextHelper.IsStudent(user))
            {
                return rd => rd.Student != null && rd.Student.UserId == userId;
            }
            else if (UserContextHelper.IsTeacher(user))
            {
                return rd => rd.Resource != null &&
                           rd.Resource.Teacher != null &&
                           rd.Resource.Teacher.UserId == userId;
            }
            else if (UserContextHelper.IsTutor(user))
            {
                return rd => rd.Resource != null &&
                           rd.Resource.Tutor != null &&
                           rd.Resource.Tutor.UserId == userId;
            }
            else if (UserContextHelper.IsAdmin(user))
            {
                return null;
            }
            else
            {
                return rd => false;
            }
        }

        private Expression<Func<ResourceDownload, bool>> CombinePredicates(
            Expression<Func<ResourceDownload, bool>> left,
            Expression<Func<ResourceDownload, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(ResourceDownload), "rd");
            var leftBody = Expression.Invoke(left, parameter);
            var rightBody = Expression.Invoke(right, parameter);
            var combined = Expression.AndAlso(leftBody, rightBody);

            return Expression.Lambda<Func<ResourceDownload, bool>>(combined, parameter);
        }

        private Func<IQueryable<ResourceDownload>, IOrderedQueryable<ResourceDownload>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "downloadedat" => sortDescending
                    ? query => query.OrderByDescending(rd => rd.DownloadedAt)
                    : query => query.OrderBy(rd => rd.DownloadedAt),
                _ => sortDescending
                    ? query => query.OrderByDescending(rd => rd.Id)
                    : query => query.OrderBy(rd => rd.Id)
            };
        }

        public async Task<bool> DeleteResourceDownloadAsync(long id)
        {
            var resourceDownload = await _resourceDownloadRepository.GetAsync(
                predicate: rd => rd.Id == id,
                disableTracking: false);

            if (resourceDownload == null)
                return false;

            await _resourceDownloadRepository.DeleteAsync(resourceDownload);
            return true;
        }

        public async Task<int> CountResourceDownloadsAsync(
            ClaimsPrincipal currentUser,
            long? resourceId = null,
            long? studentId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var predicate = BuildRoleBasedPredicate(currentUser);

            Expression<Func<ResourceDownload, bool>> filterPredicate = rd =>
                (!resourceId.HasValue || rd.ResourceId == resourceId.Value) &&
                (!studentId.HasValue || rd.StudentId == studentId.Value) &&
                (!fromDate.HasValue || rd.DownloadedAt >= fromDate.Value) &&
                (!toDate.HasValue || rd.DownloadedAt <= toDate.Value);

            predicate = predicate == null
                ? filterPredicate
                : CombinePredicates(predicate, filterPredicate);

            return await _resourceDownloadRepository.CountAsync(predicate);
        }

        public async Task<bool> ResourceDownloadExistsAsync(long id)
        {
            return await _resourceDownloadRepository.ExistsAsync(rd => rd.Id == id);
        }

        public async Task<IEnumerable<ResourceDownloadDto>> GetDownloadsByResourceAsync(long resourceId)
        {
            var downloads = await _resourceDownloadRepository.GetManyAsync(
                predicate: rd => rd.ResourceId == resourceId,
                orderBy: query => query.OrderByDescending(rd => rd.DownloadedAt),
                disableTracking: true);

            return _mapper.Map<IEnumerable<ResourceDownloadDto>>(downloads);
        }

        public async Task<IEnumerable<ResourceDownloadDto>> GetDownloadsByStudentAsync(long studentId)
        {
            var downloads = await _resourceDownloadRepository.GetManyAsync(
                predicate: rd => rd.StudentId == studentId,
                orderBy: query => query.OrderByDescending(rd => rd.DownloadedAt),
                disableTracking: true);

            return _mapper.Map<IEnumerable<ResourceDownloadDto>>(downloads);
        }

        public async Task<IEnumerable<ResourceDownloadDto>> GetRecentDownloadsAsync(long studentId, int count = 10)
        {
            var downloads = await _resourceDownloadRepository.GetManyAsync(
                predicate: rd => rd.StudentId == studentId,
                orderBy: query => query.OrderByDescending(rd => rd.DownloadedAt),
                disableTracking: true);

            return _mapper.Map<IEnumerable<ResourceDownloadDto>>(downloads.Take(count));
        }

        public async Task<bool> HasStudentDownloadedResourceAsync(long studentId, long resourceId)
        {
            return await _resourceDownloadRepository.ExistsAsync(rd =>
                rd.StudentId == studentId && rd.ResourceId == resourceId);
        }

        public async Task<int> GetDownloadCountByResourceAsync(long resourceId)
        {
            return await _resourceDownloadRepository.CountAsync(rd => rd.ResourceId == resourceId);
        }

        public async Task<int> GetDownloadCountByStudentAsync(long studentId)
        {
            return await _resourceDownloadRepository.CountAsync(rd => rd.StudentId == studentId);
        }

        public async Task<Dictionary<long, int>> GetPopularResourcesAsync(int count = 10, DateTime? fromDate = null, DateTime? toDate = null)
        {
            Expression<Func<ResourceDownload, bool>> predicate = fromDate.HasValue || toDate.HasValue
                ? rd => (!fromDate.HasValue || rd.DownloadedAt >= fromDate.Value) &&
                       (!toDate.HasValue || rd.DownloadedAt <= toDate.Value)
                : null;

            var downloads = await _resourceDownloadRepository.GetManyAsync(
                predicate: predicate,
                disableTracking: true);

            var popularResources = downloads
                .GroupBy(rd => rd.ResourceId)
                .Select(g => new { ResourceId = g.Key, DownloadCount = g.Count() })
                .OrderByDescending(x => x.DownloadCount)
                .Take(count)
                .ToDictionary(x => x.ResourceId, x => x.DownloadCount);

            return popularResources;
        }
    }
}