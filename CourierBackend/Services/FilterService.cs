namespace CourierBackend.Services
{
    using System.Linq.Expressions;

    public static class FilterService
    {
        public static IQueryable<TEntity> Apply<TEntity>(
            IQueryable<TEntity> query,
            Dictionary<string, Dictionary<string, string>> filters
        )
        {
            var parameter = Expression.Parameter(typeof(TEntity), "x");

            foreach (var field in filters)
            {
                var property = typeof(TEntity).GetProperty(field.Key);
                if (property == null) continue;

                foreach (var operation in field.Value)
                {
                    var expression = BuildExpression(parameter, property, operation.Key, operation.Value);
                    if (expression == null) continue;

                    var lambda = Expression.Lambda<Func<TEntity, bool>>(expression, parameter);
                    query = query.Where(lambda);
                }
            }

            return query;
        }

        private static Expression? BuildExpression(
            ParameterExpression param,
            System.Reflection.PropertyInfo property,
            string op,
            string value
        )
        {
            var member = Expression.Property(param, property);
            var constant = Expression.Constant(Convert.ChangeType(value, property.PropertyType));

            return op.ToLower() switch
            {
                "eq" => Expression.Equal(member, constant),
                "neq" => Expression.NotEqual(member, constant),
                "gt" => Expression.GreaterThan(member, constant),
                "gte" => Expression.GreaterThanOrEqual(member, constant),
                "lt" => Expression.LessThan(member, constant),
                "lte" => Expression.LessThanOrEqual(member, constant),

                "like" => Expression.Call(
                    member,
                    typeof(string).GetMethod("Contains", new[] { typeof(string) })!,
                    Expression.Constant(value)
                ),

                _ => null
            };
        }
    }
}