using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SqlQueryBuilder.Services
{
    public interface ISqlQueryBuilderService
    {
        /// <summary>
        /// Builds an Insert Query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns>The sqlquery and params based on the model and values given</returns>
        (string query, Dictionary<string, object> parameters) Insert<T>(T model);

        /// <summary>
        /// build a select query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        (string query, Dictionary<string, object> parameters) Select<T>(Expression<Func<T, bool>> filter) where T : new();

        /// <summary>
        /// build a select query with provided table and column names
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        (string query, Dictionary<string, object> parameters) Select<T>(string tableName, string columns, Expression<Func<T, bool>> filter) where T : new();
    }
}
