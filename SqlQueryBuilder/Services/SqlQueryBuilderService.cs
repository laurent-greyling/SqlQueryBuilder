using SqlQueryBuilder.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlQueryBuilder.Services
{
    public class SqlQueryBuilderService : ISqlQueryBuilderService
    {
        public (string query, Dictionary<string, object> parameters) Insert<T>(T model)
        {
            Ensure.ArgumentNotNull(model, nameof(model));

            var tableName = TableName(model);

            var properties = model.GetType().GetProperties();

            var parameters = new Dictionary<string, object>();
            var paramKeys = new StringBuilder();

            var builder = new SqlCommandBuilder();

            var columnNames = properties.Aggregate(new StringBuilder(),
                (current, next) => current.Append(current.Length == 0 ? "" : ",").Append(builder.QuoteIdentifier(next.Name)))
                .ToString();

            foreach (var property in properties)
            {
                var name = property.Name;
                var value = model.GetType().GetProperty(name).GetValue(model, null);

                parameters.Add($"@{ name }", value);
                paramKeys.Append($"@{ name },");
            }

            return ($@"INSERT INTO { tableName }({ columnNames }) VALUES({ paramKeys.ToString().TrimEnd(',') })",
                parameters);
        }

        public (string query, Dictionary<string, object> parameters) Select<T>(Expression<Func<T, bool>> filter) where T : new()
        {
            var model = new T();

            var tableName = TableName(model);
            var columnNames = ColumnNames(model);

            if (filter == null)
            {
                return ($"SELECT { columnNames } FROM {tableName}", null);
            }

            var (parameters, conditional) = SqlQueryFilter.Where(filter);

            var query = $"SELECT { columnNames } FROM {tableName} {conditional}";

            return (query, parameters);
        }

        public (string query, Dictionary<string, object> parameters) Select<T>(string tableName,
            string columns,
            Expression<Func<T, bool>> filter) where T : new()
        {
            var builder = new SqlCommandBuilder();
            var escapedTableName = ExcapedTableName(tableName, builder);

            if (filter == null)
            {
                return ($"SELECT { columns } FROM { escapedTableName }", null);
            }

            var (parameters, conditional) = SqlQueryFilter.Where(filter);

            var query = $"SELECT { columns } FROM { escapedTableName } {conditional}";

            return (query, parameters);
        }

        private static string ColumnNames<T>(T model)
        {
            var builder = new SqlCommandBuilder();

            var properties = model.GetType().GetProperties();

            var columns = properties.Aggregate(new StringBuilder(),
                (current, next) => current.Append(current.Length == 0 ? "" : ",").Append(builder.QuoteIdentifier(next.Name)))
                .ToString();

            return columns;
        }

        private static string TableName<T>(T model)
        {
            var builder = new SqlCommandBuilder();

            var tableName = model.GetType().Name;
            var escapedTableName = builder.QuoteIdentifier(tableName);

            var attribute = model.GetType()
                .GetCustomAttributes(typeof(DisplayNameAttribute), true)
                .FirstOrDefault() as DisplayNameAttribute;

            if (attribute != null)
            {
                //Display Name should always contain the schema value as well schema.table
                tableName = attribute.DisplayName;
                escapedTableName = ExcapedTableName(tableName, builder);
            }

            return escapedTableName;
        }

        private static string ExcapedTableName(string tableName, SqlCommandBuilder builder)
        {
            var tableWithSchema = tableName.Split(".");
            var escapedSchema = builder.QuoteIdentifier(tableWithSchema[0]);
            var escapedTable = builder.QuoteIdentifier(tableWithSchema[1]);

            return $"{ escapedSchema }.{escapedTable}";
        }

    }
}
