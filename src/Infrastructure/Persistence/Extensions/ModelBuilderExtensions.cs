using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyGlobalFilters<TInterface>(this ModelBuilder modelBuilder, Expression<Func<TInterface, bool>> expression)
    {
        IEnumerable<Type> entities = modelBuilder.Model.GetEntityTypes()
                                                 .Where(e => e.ClrType.GetInterface(typeof(TInterface).Name) != null)
                                                 .Select(e => e.ClrType);
        foreach (Type entity in entities)
        {
            ParameterExpression newParam = Expression.Parameter(entity);
            Expression          newBody  = ReplacingExpressionVisitor.Replace(expression.Parameters.Single(), newParam, expression.Body);
            modelBuilder.Entity(entity)
                        .HasQueryFilter(Expression.Lambda(newBody, newParam));
        }
    }
}