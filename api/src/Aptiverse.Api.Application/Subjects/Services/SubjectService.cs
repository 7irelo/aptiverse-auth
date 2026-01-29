using Aptiverse.Api.Application.Subjects.Dtos;
using Aptiverse.Api.Domain.Models.Students;
using Aptiverse.Api.Domain.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Aptiverse.Api.Application.Subjects.Services
{
    public class SubjectService(
        IRepository<Subject> subjectRepository,
        IMapper mapper) : ISubjectService
    {
        private readonly IRepository<Subject> _subjectRepository = subjectRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto createSubjectDto)
        {
            ArgumentNullException.ThrowIfNull(createSubjectDto);

            Subject subject = _mapper.Map<Subject>(createSubjectDto);
            subject.Id = Guid.NewGuid().ToString();
            await _subjectRepository.AddAsync(subject);
            return _mapper.Map<SubjectDto>(subject);
        }

        public async Task<SubjectDto?> GetSubjectByIdAsync(string id)
        {
            var subject = await _subjectRepository.GetAsync(
                predicate: s => s.Id == id,
                include: query => query
                    .Include(s => s.StudentSubjects)
                    .Include(s => s.Topics),
                disableTracking: false);

            if (subject == null)
                return null;

            return _mapper.Map<SubjectDto>(subject);
        }

        public async Task<PaginatedResult<SubjectDto>> GetSubjectsAsync(
            string? search = null,
            string? code = null,
            string? sortBy = "Id",
            bool sortDescending = false,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<Subject, bool>>? predicate = BuildFilterPredicate(search, code);
            Func<IQueryable<Subject>, IOrderedQueryable<Subject>>? orderBy = GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _subjectRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                include: query => query
                    .Include(s => s.StudentSubjects)
                    .Include(s => s.Topics));

            var subjectDtos = _mapper.Map<List<SubjectDto>>(paginatedResult.Data);

            return new PaginatedResult<SubjectDto>(
                subjectDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<Subject, bool>>? BuildFilterPredicate(string? search, string? code)
        {
            if (string.IsNullOrEmpty(search) && string.IsNullOrEmpty(code))
                return null;

            return s =>
                (string.IsNullOrEmpty(search) || s.Name.Contains(search) || s.Description.Contains(search)) &&
                (string.IsNullOrEmpty(code) || s.Code == code);
        }

        private Func<IQueryable<Subject>, IOrderedQueryable<Subject>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "name" => sortDescending
                    ? query => query.OrderByDescending(s => s.Name).ThenByDescending(s => s.Id)
                    : query => query.OrderBy(s => s.Name).ThenBy(s => s.Id),
                "code" => sortDescending
                    ? query => query.OrderByDescending(s => s.Code).ThenByDescending(s => s.Id)
                    : query => query.OrderBy(s => s.Code).ThenBy(s => s.Id),
                _ => sortDescending
                    ? query => query.OrderByDescending(s => s.Id)
                    : query => query.OrderBy(s => s.Id)
            };
        }

        public async Task<SubjectDto> UpdateSubjectAsync(string id, UpdateSubjectDto updateSubjectDto)
        {
            var existingSubject = await _subjectRepository.GetAsync(
                predicate: s => s.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Subject with ID {id} not found");

            _mapper.Map(updateSubjectDto, existingSubject);
            await _subjectRepository.UpdateAsync(existingSubject);
            return _mapper.Map<SubjectDto>(existingSubject);
        }

        public async Task<bool> DeleteSubjectAsync(string id)
        {
            var subject = await _subjectRepository.GetAsync(
                predicate: s => s.Id == id,
                disableTracking: false);

            if (subject == null)
                return false;

            await _subjectRepository.DeleteAsync(subject);
            return true;
        }

        public async Task<int> CountSubjectsAsync(string? search = null)
        {
            if (string.IsNullOrEmpty(search))
                return await _subjectRepository.CountAsync();

            Expression<Func<Subject, bool>> predicate = s => s.Name.Contains(search) || s.Description.Contains(search);
            return await _subjectRepository.CountAsync(predicate);
        }

        public async Task<bool> SubjectExistsAsync(string id)
        {
            return await _subjectRepository.ExistsAsync(s => s.Id == id);
        }
    }
}