using System.Linq.Expressions;
using System.Reflection;

namespace Blazor.Server.UI.Components.Common;

public static class DataGridExtensions
{
    public static IQueryable<T> EfOrderBySortDefinitions<T, T1>(this IQueryable<T> source, GridState<T1> state)
    {
        return EfOrderBySortDefinitions(source, state.SortDefinitions);
    }

    public static IQueryable<T> EfOrderBySortDefinitions<T, T1>(this IQueryable<T> source, ICollection<SortDefinition<T1>> sortDefinitions)
    {
        // avoid multiple enumeration
        IQueryable<T> sourceQuery = source;

        if (sortDefinitions.Count == 0)
        {
            return sourceQuery;
        }

        IOrderedQueryable<T>? orderedQuery = null;

        foreach (SortDefinition<T1> sortDefinition in sortDefinitions)
        {
            ParameterExpression parameter       = Expression.Parameter(typeof(T), "x");
            MemberExpression    orderByProperty = Expression.Property(parameter, sortDefinition.SortBy);
            LambdaExpression    sortLambda      = Expression.Lambda(orderByProperty, parameter);
            if (orderedQuery is null)
            {
                MethodInfo sortMethod = typeof(Queryable).GetMethods()
                                                         .Where(m => m.Name == (sortDefinition.Descending ? "OrderByDescending" : "OrderBy") && m.IsGenericMethodDefinition) // ensure selecting the right overload
                                                         .Single(m => m.GetParameters()
                                                                       .ToList()
                                                                       .Count == 2);
                MethodInfo genericMethod = sortMethod.MakeGenericMethod(typeof(T), orderByProperty.Type);
                orderedQuery = (IOrderedQueryable<T>?)genericMethod.Invoke(genericMethod, new object[]
                                                                                          {
                                                                                              source,
                                                                                              sortLambda
                                                                                          });
            }
            else
            {
                MethodInfo sortMethod = typeof(Queryable).GetMethods()
                                                         .Where(m => m.Name == (sortDefinition.Descending ? "ThenByDescending" : "ThenBy") && m.IsGenericMethodDefinition) // ensure selecting the right overload
                                                         .Single(m => m.GetParameters()
                                                                       .ToList()
                                                                       .Count == 2);
                MethodInfo genericMethod = sortMethod.MakeGenericMethod(typeof(T), orderByProperty.Type);
                orderedQuery = (IOrderedQueryable<T>?)genericMethod.Invoke(genericMethod, new object[]
                                                                                          {
                                                                                              source,
                                                                                              sortLambda
                                                                                          });
            }
        }

        return orderedQuery ?? sourceQuery;
    }
}