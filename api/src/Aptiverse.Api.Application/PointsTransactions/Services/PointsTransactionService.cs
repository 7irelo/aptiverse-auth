using Aptiverse.Api.Application.PointsTransactions.Dtos;
using Aptiverse.Api.Domain.Models.Rewards;
using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Aptiverse.Api.Application.PointsTransactions.Services
{
    public class PointsTransactionService(
        IRepository<PointsTransaction> pointsTransactionRepository,
        IMapper mapper) : IPointsTransactionService
    {
        private readonly IRepository<PointsTransaction> _pointsTransactionRepository = pointsTransactionRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<PointsTransactionDto> CreatePointsTransactionAsync(CreatePointsTransactionDto createPointsTransactionDto)
        {
            ArgumentNullException.ThrowIfNull(createPointsTransactionDto);

            var pointsTransaction = _mapper.Map<PointsTransaction>(createPointsTransactionDto);

            await _pointsTransactionRepository.AddAsync(pointsTransaction);
            return _mapper.Map<PointsTransactionDto>(pointsTransaction);
        }

        public async Task<PointsTransactionDto?> GetPointsTransactionByIdAsync(long id)
        {
            var pointsTransaction = await _pointsTransactionRepository.GetAsync(
                predicate: pt => pt.Id == id,
                disableTracking: false);

            return _mapper.Map<PointsTransactionDto>(pointsTransaction);
        }

        public async Task<PaginatedResult<PointsTransactionDto>> GetPointsTransactionsAsync(
            ClaimsPrincipal currentUser,
            long? studentPointsId = null,
            long? studentId = null,
            string? transactionType = null,
            string? source = null,
            long? relatedGoalId = null,
            long? relatedRewardId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? sortBy = "TransactionDate",
            bool sortDescending = true,
            int page = 1,
            int pageSize = 20)
        {
            Expression<Func<PointsTransaction, bool>>? predicate = BuildRoleBasedPredicate(currentUser);

            if (studentPointsId.HasValue || studentId.HasValue || !string.IsNullOrEmpty(transactionType) ||
                !string.IsNullOrEmpty(source) || relatedGoalId.HasValue || relatedRewardId.HasValue ||
                fromDate.HasValue || toDate.HasValue)
            {
                Expression<Func<PointsTransaction, bool>> filterPredicate = pt =>
                    (!studentPointsId.HasValue || pt.StudentPointsId == studentPointsId.Value) &&
                    (!studentId.HasValue || pt.StudentPoints != null && pt.StudentPoints.StudentId == studentId.Value) &&
                    (string.IsNullOrEmpty(transactionType) || pt.TransactionType == transactionType) &&
                    (string.IsNullOrEmpty(source) || pt.Source == source) &&
                    (!relatedGoalId.HasValue || pt.RelatedGoalId == relatedGoalId.Value) &&
                    (!relatedRewardId.HasValue || pt.RelatedRewardId == relatedRewardId.Value) &&
                    (!fromDate.HasValue || pt.TransactionDate >= fromDate.Value) &&
                    (!toDate.HasValue || pt.TransactionDate <= toDate.Value);

                predicate = predicate == null
                    ? filterPredicate
                    : CombinePredicates(predicate, filterPredicate);
            }

            Func<IQueryable<PointsTransaction>, IOrderedQueryable<PointsTransaction>>? orderBy =
                GetOrderByFunction(sortBy, sortDescending);

            var paginatedResult = await _pointsTransactionRepository.GetPaginatedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy);

            var pointsTransactionDtos = _mapper.Map<List<PointsTransactionDto>>(paginatedResult.Data);

            return new PaginatedResult<PointsTransactionDto>(
                pointsTransactionDtos,
                paginatedResult.TotalRecords,
                paginatedResult.PageNumber,
                paginatedResult.PageSize);
        }

        private Expression<Func<PointsTransaction, bool>>? BuildRoleBasedPredicate(ClaimsPrincipal user)
        {
            var userId = UserContextHelper.GetUserId(user);

            if (UserContextHelper.IsSuperUser(user))
            {
                return null;
            }
            else if (UserContextHelper.IsStudent(user))
            {
                return pt => pt.StudentPoints != null &&
                           pt.StudentPoints.Student != null &&
                           pt.StudentPoints.Student.UserId == userId;
            }
            else if (UserContextHelper.IsTeacher(user))
            {
                return pt => pt.StudentPoints != null &&
                           pt.StudentPoints.Student != null &&
                           pt.StudentPoints.Student.TeacherStudents != null &&
                           pt.StudentPoints.Student.TeacherStudents.Any(ts => ts.Teacher != null && ts.Teacher.UserId == userId);
            }
            else if (UserContextHelper.IsTutor(user))
            {
                return pt => pt.StudentPoints != null &&
                           pt.StudentPoints.Student != null &&
                           pt.StudentPoints.Student.TutorStudents != null &&
                           pt.StudentPoints.Student.TutorStudents.Any(ts => ts.Tutor != null && ts.Tutor.UserId == userId);
            }
            else if (UserContextHelper.IsParent(user))
            {
                return pt => pt.StudentPoints != null &&
                           pt.StudentPoints.Student != null &&
                           pt.StudentPoints.Student.ParentStudents != null &&
                           pt.StudentPoints.Student.ParentStudents.Any(ps => ps.Parent != null && ps.Parent.UserId == userId);
            }
            else if (UserContextHelper.IsAdmin(user))
            {
                return pt => pt.StudentPoints != null &&
                           pt.StudentPoints.Student != null &&
                           pt.StudentPoints.Student.Admin != null &&
                           pt.StudentPoints.Student.Admin.UserId == userId;
            }
            else
            {
                return pt => false;
            }
        }

        private Expression<Func<PointsTransaction, bool>> CombinePredicates(
            Expression<Func<PointsTransaction, bool>> left,
            Expression<Func<PointsTransaction, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(PointsTransaction), "pt");
            var leftBody = Expression.Invoke(left, parameter);
            var rightBody = Expression.Invoke(right, parameter);
            var combined = Expression.AndAlso(leftBody, rightBody);

            return Expression.Lambda<Func<PointsTransaction, bool>>(combined, parameter);
        }

        private Func<IQueryable<PointsTransaction>, IOrderedQueryable<PointsTransaction>>? GetOrderByFunction(
            string sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return null;

            return sortBy.ToLower() switch
            {
                "transactiondate" => sortDescending
                    ? query => query.OrderByDescending(pt => pt.TransactionDate)
                    : query => query.OrderBy(pt => pt.TransactionDate),
                "points" => sortDescending
                    ? query => query.OrderByDescending(pt => pt.Points)
                    : query => query.OrderBy(pt => pt.Points),
                "transactiontype" => sortDescending
                    ? query => query.OrderByDescending(pt => pt.TransactionType)
                    : query => query.OrderBy(pt => pt.TransactionType),
                "source" => sortDescending
                    ? query => query.OrderByDescending(pt => pt.Source)
                    : query => query.OrderBy(pt => pt.Source),
                _ => sortDescending
                    ? query => query.OrderByDescending(pt => pt.Id)
                    : query => query.OrderBy(pt => pt.Id)
            };
        }

        public async Task<PointsTransactionDto> UpdatePointsTransactionAsync(long id, UpdatePointsTransactionDto updatePointsTransactionDto)
        {
            var existingPointsTransaction = await _pointsTransactionRepository.GetAsync(
                predicate: pt => pt.Id == id,
                disableTracking: false)
                ?? throw new KeyNotFoundException($"Points transaction with ID {id} not found");

            _mapper.Map(updatePointsTransactionDto, existingPointsTransaction);

            await _pointsTransactionRepository.UpdateAsync(existingPointsTransaction);
            return _mapper.Map<PointsTransactionDto>(existingPointsTransaction);
        }

        public async Task<bool> DeletePointsTransactionAsync(long id)
        {
            var pointsTransaction = await _pointsTransactionRepository.GetAsync(
                predicate: pt => pt.Id == id,
                disableTracking: false);

            if (pointsTransaction == null)
                return false;

            await _pointsTransactionRepository.DeleteAsync(pointsTransaction);
            return true;
        }

        public async Task<int> CountPointsTransactionsAsync(
            ClaimsPrincipal currentUser,
            long? studentPointsId = null,
            long? studentId = null,
            string? transactionType = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var predicate = BuildRoleBasedPredicate(currentUser);

            Expression<Func<PointsTransaction, bool>> filterPredicate = pt =>
                (!studentPointsId.HasValue || pt.StudentPointsId == studentPointsId.Value) &&
                (!studentId.HasValue || pt.StudentPoints != null && pt.StudentPoints.StudentId == studentId.Value) &&
                (string.IsNullOrEmpty(transactionType) || pt.TransactionType == transactionType) &&
                (!fromDate.HasValue || pt.TransactionDate >= fromDate.Value) &&
                (!toDate.HasValue || pt.TransactionDate <= toDate.Value);

            predicate = predicate == null
                ? filterPredicate
                : CombinePredicates(predicate, filterPredicate);

            return await _pointsTransactionRepository.CountAsync(predicate);
        }

        public async Task<bool> PointsTransactionExistsAsync(long id)
        {
            return await _pointsTransactionRepository.ExistsAsync(pt => pt.Id == id);
        }

        public async Task<IEnumerable<PointsTransactionDto>> GetTransactionsByStudentPointsAsync(long studentPointsId)
        {
            var transactions = await _pointsTransactionRepository.GetManyAsync(
                predicate: pt => pt.StudentPointsId == studentPointsId,
                orderBy: query => query.OrderByDescending(pt => pt.TransactionDate),
                disableTracking: true);

            return _mapper.Map<IEnumerable<PointsTransactionDto>>(transactions);
        }

        public async Task<IEnumerable<PointsTransactionDto>> GetTransactionsByStudentAsync(long studentId)
        {
            var transactions = await _pointsTransactionRepository.GetManyAsync(
                predicate: pt => pt.StudentPoints != null && pt.StudentPoints.StudentId == studentId,
                orderBy: query => query.OrderByDescending(pt => pt.TransactionDate),
                disableTracking: true);

            return _mapper.Map<IEnumerable<PointsTransactionDto>>(transactions);
        }

        public async Task<IEnumerable<PointsTransactionDto>> GetRecentTransactionsByStudentAsync(long studentId, int count = 10)
        {
            var transactions = await _pointsTransactionRepository.GetManyAsync(
                predicate: pt => pt.StudentPoints != null && pt.StudentPoints.StudentId == studentId,
                orderBy: query => query.OrderByDescending(pt => pt.TransactionDate),
                disableTracking: true);

            return _mapper.Map<IEnumerable<PointsTransactionDto>>(transactions.Take(count));
        }

        public async Task<IEnumerable<PointsTransactionDto>> GetEarnedTransactionsByStudentAsync(long studentId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            Expression<Func<PointsTransaction, bool>> predicate = pt =>
                pt.StudentPoints != null &&
                pt.StudentPoints.StudentId == studentId &&
                (pt.TransactionType == "Earned" || pt.TransactionType == "Bonus");

            if (fromDate.HasValue || toDate.HasValue)
            {
                predicate = pt => pt.StudentPoints != null &&
                               pt.StudentPoints.StudentId == studentId &&
                               (pt.TransactionType == "Earned" || pt.TransactionType == "Bonus") &&
                               (!fromDate.HasValue || pt.TransactionDate >= fromDate.Value) &&
                               (!toDate.HasValue || pt.TransactionDate <= toDate.Value);
            }

            var transactions = await _pointsTransactionRepository.GetManyAsync(
                predicate: predicate,
                orderBy: query => query.OrderByDescending(pt => pt.TransactionDate),
                disableTracking: true);

            return _mapper.Map<IEnumerable<PointsTransactionDto>>(transactions);
        }

        public async Task<IEnumerable<PointsTransactionDto>> GetSpentTransactionsByStudentAsync(long studentId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            Expression<Func<PointsTransaction, bool>> predicate = pt =>
                pt.StudentPoints != null &&
                pt.StudentPoints.StudentId == studentId &&
                (pt.TransactionType == "Spent" || pt.TransactionType == "Penalty");

            if (fromDate.HasValue || toDate.HasValue)
            {
                predicate = pt => pt.StudentPoints != null &&
                               pt.StudentPoints.StudentId == studentId &&
                               (pt.TransactionType == "Spent" || pt.TransactionType == "Penalty") &&
                               (!fromDate.HasValue || pt.TransactionDate >= fromDate.Value) &&
                               (!toDate.HasValue || pt.TransactionDate <= toDate.Value);
            }

            var transactions = await _pointsTransactionRepository.GetManyAsync(
                predicate: predicate,
                orderBy: query => query.OrderByDescending(pt => pt.TransactionDate),
                disableTracking: true);

            return _mapper.Map<IEnumerable<PointsTransactionDto>>(transactions);
        }

        public async Task<Dictionary<string, int>> GetTransactionSummaryAsync(long studentId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            Expression<Func<PointsTransaction, bool>> predicate = pt =>
                pt.StudentPoints != null && pt.StudentPoints.StudentId == studentId;

            if (fromDate.HasValue || toDate.HasValue)
            {
                predicate = pt => pt.StudentPoints != null &&
                               pt.StudentPoints.StudentId == studentId &&
                               (!fromDate.HasValue || pt.TransactionDate >= fromDate.Value) &&
                               (!toDate.HasValue || pt.TransactionDate <= toDate.Value);
            }

            var transactions = await _pointsTransactionRepository.GetManyAsync(
                predicate: predicate,
                disableTracking: true);

            var summary = new Dictionary<string, int>
            {
                ["TotalTransactions"] = transactions.Count(),
                ["TotalEarned"] = transactions.Where(t => t.TransactionType == "Earned" || t.TransactionType == "Bonus").Sum(t => t.Points),
                ["TotalSpent"] = transactions.Where(t => t.TransactionType == "Spent" || t.TransactionType == "Penalty").Sum(t => t.Points),
                ["EarnedCount"] = transactions.Count(t => t.TransactionType == "Earned" || t.TransactionType == "Bonus"),
                ["SpentCount"] = transactions.Count(t => t.TransactionType == "Spent" || t.TransactionType == "Penalty")
            };

            return summary;
        }

        public async Task<int> GetTotalEarnedPointsAsync(long studentId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            Expression<Func<PointsTransaction, bool>> predicate = pt =>
                pt.StudentPoints != null &&
                pt.StudentPoints.StudentId == studentId &&
                (pt.TransactionType == "Earned" || pt.TransactionType == "Bonus");

            if (fromDate.HasValue || toDate.HasValue)
            {
                predicate = pt => pt.StudentPoints != null &&
                               pt.StudentPoints.StudentId == studentId &&
                               (pt.TransactionType == "Earned" || pt.TransactionType == "Bonus") &&
                               (!fromDate.HasValue || pt.TransactionDate >= fromDate.Value) &&
                               (!toDate.HasValue || pt.TransactionDate <= toDate.Value);
            }

            var transactions = await _pointsTransactionRepository.GetManyAsync(
                predicate: predicate,
                disableTracking: true);

            return transactions.Sum(t => t.Points);
        }

        public async Task<int> GetTotalSpentPointsAsync(long studentId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            Expression<Func<PointsTransaction, bool>> predicate = pt =>
                pt.StudentPoints != null &&
                pt.StudentPoints.StudentId == studentId &&
                (pt.TransactionType == "Spent" || pt.TransactionType == "Penalty");

            if (fromDate.HasValue || toDate.HasValue)
            {
                predicate = pt => pt.StudentPoints != null &&
                               pt.StudentPoints.StudentId == studentId &&
                               (pt.TransactionType == "Spent" || pt.TransactionType == "Penalty") &&
                               (!fromDate.HasValue || pt.TransactionDate >= fromDate.Value) &&
                               (!toDate.HasValue || pt.TransactionDate <= toDate.Value);
            }

            var transactions = await _pointsTransactionRepository.GetManyAsync(
                predicate: predicate,
                disableTracking: true);

            return transactions.Sum(t => t.Points);
        }
    }
}