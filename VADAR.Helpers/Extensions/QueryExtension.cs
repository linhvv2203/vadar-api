// <copyright file="QueryExtension.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System.Linq;
using System.Linq.Expressions;

namespace VADAR.Helpers.Extensions
{
    /// <summary>
    /// Query Extension.
    /// </summary>
    public static class QueryExtension
    {
        /// <summary>
        /// Order By Field Name.
        /// </summary>
        /// <typeparam name="T">Model.</typeparam>
        /// <param name="q">Queryable.</param>
        /// <param name="sortField">FieldName.</param>
        /// <param name="ascending">Ascending.</param>
        /// <returns>IQueryable.</returns>
        public static IQueryable<T> OrderByField<T>(this IQueryable<T> q, string sortField, bool ascending)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var prop = Expression.Property(param, sortField);
            var exp = Expression.Lambda(prop, param);
            var method = ascending ? "OrderBy" : "OrderByDescending";
            var types = new[] { q.ElementType, exp.Body.Type };
            var mce = Expression.Call(typeof(Queryable), method, types, q.Expression, exp);
            return q.Provider.CreateQuery<T>(mce);
        }
    }
}
