using System.Linq.Expressions;

namespace Aptiverse.Api.Infrastructure.Utilities
{
    public static class QueryFilterBuilder<T> where T : class
    {
        public static Expression<Func<T, bool>> BuildPredicateFromFilters(Dictionary<string, object>? filters)
        {
            if (filters == null || filters.Count == 0)
                return x => true;

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression? combinedExpression = null;

            foreach (var filter in filters)
            {
                try
                {
                    var property = Expression.Property(parameter, filter.Key);
                    var constant = Expression.Constant(filter.Value);
                    var equality = Expression.Equal(property, constant);

                    combinedExpression = combinedExpression == null
                        ? equality
                        : Expression.AndAlso(combinedExpression, equality);
                }
                catch (ArgumentException ex)
                {
                    throw new ArgumentException($"Invalid filter property '{filter.Key}' for type {typeof(T).Name}", ex);
                }
            }

            if (combinedExpression == null)
                return x => true;

            return Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
        }
    }
}
