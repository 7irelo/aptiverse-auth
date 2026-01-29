using Aptiverse.Api.Application.PointsTransactions.Dtos;
using Aptiverse.Api.Domain.Repositories;
using System.Security.Claims;

namespace Aptiverse.Api.Application.PointsTransactions.Services
{
    public interface IPointsTransactionService
    {
        Task<PointsTransactionDto> CreatePointsTransactionAsync(CreatePointsTransactionDto createPointsTransactionDto);
        Task<PointsTransactionDto?> GetPointsTransactionByIdAsync(long id);

        Task<PaginatedResult<PointsTransactionDto>> GetPointsTransactionsAsync(
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
            int pageSize = 20);

        Task<PointsTransactionDto> UpdatePointsTransactionAsync(long id, UpdatePointsTransactionDto updatePointsTransactionDto);
        Task<bool> DeletePointsTransactionAsync(long id);
        Task<int> CountPointsTransactionsAsync(ClaimsPrincipal currentUser,
            long? studentPointsId = null,
            long? studentId = null,
            string? transactionType = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);
        Task<bool> PointsTransactionExistsAsync(long id);

        Task<IEnumerable<PointsTransactionDto>> GetTransactionsByStudentPointsAsync(long studentPointsId);
        Task<IEnumerable<PointsTransactionDto>> GetTransactionsByStudentAsync(long studentId);
        Task<IEnumerable<PointsTransactionDto>> GetRecentTransactionsByStudentAsync(long studentId, int count = 10);
        Task<IEnumerable<PointsTransactionDto>> GetEarnedTransactionsByStudentAsync(long studentId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<IEnumerable<PointsTransactionDto>> GetSpentTransactionsByStudentAsync(long studentId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<Dictionary<string, int>> GetTransactionSummaryAsync(long studentId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<int> GetTotalEarnedPointsAsync(long studentId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<int> GetTotalSpentPointsAsync(long studentId, DateTime? fromDate = null, DateTime? toDate = null);
    }
}