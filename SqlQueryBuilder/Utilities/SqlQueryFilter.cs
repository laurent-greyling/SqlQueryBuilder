using SqlQueryBuilder.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SqlQueryBuilder.Utilities
{
    public static class SqlQueryFilter
    {
        public static (Dictionary<string, object> parameters, string conditional) Where<T>(Expression<Func<T, bool>> filter)
        {
            var paramaterDictionary = new Dictionary<string, object>();

            var binaryExpression = (BinaryExpression)filter.Body;

            var filterString = new StringBuilder();

            var mainExpression = ExpressionEvaluator.MainExpression(binaryExpression);

            var condition = ExpressionEvaluator.IsFinalExpression(mainExpression.Right, mainExpression.Left)
                ? Condition(mainExpression, filterString, paramaterDictionary, binaryExpression)
                : Conditions(mainExpression, paramaterDictionary);

            return (paramaterDictionary, condition);
        }

        private static string Condition(ExpressionModel expression,
            StringBuilder filterString,
            Dictionary<string, object> paramaterDictionary,
            BinaryExpression binaryExpression)
        {
            (string expressionKey, object expressionValue) = ExpressionEvaluator.ExpressionKeyValue(binaryExpression);

            filterString.Append($" { expressionKey } { ExpressionEvaluator.Operators[expression.Node] } { $"@{expressionKey}" }");

            expressionValue = expressionValue is bool
                       ? Convert.ToByte(expressionValue)
                       : expressionValue;

            paramaterDictionary.Add($"@{expressionKey}", expressionValue);

            return $"WHERE { filterString }";
        }

        private static string Conditions(ExpressionModel expression,
            Dictionary<string, object> paramaterDictionary)
        {
            var leftText = LeftString(expression, paramaterDictionary);
            var rightText = RightString(expression, paramaterDictionary);

            return $"WHERE ({ leftText }) { ExpressionEvaluator.Operators[expression.Node] } ({rightText})";
        }

        private static string LeftString(ExpressionModel expression,
            Dictionary<string, object> paramaterDictionary)
        {
            var leftSqlModels = new List<SqlQueryModel>();

            ExpressionEvaluator.Evaluate((BinaryExpression)expression.Right, leftSqlModels, paramaterDictionary);

            return BuildString(leftSqlModels);
        }

        private static string RightString(ExpressionModel expression,
            Dictionary<string, object> paramaterDictionary)
        {
            var rightSqlModels = new List<SqlQueryModel>();

            ExpressionEvaluator.Evaluate((BinaryExpression)expression.Left, rightSqlModels, paramaterDictionary);

            return BuildString(rightSqlModels);
        }

        private static string BuildString(List<SqlQueryModel> model)
        {
            var builder = new StringBuilder();

            for (int i = 0; i < model.Count; i++)
            {
                builder.Append($" { model[i].Key }");
                builder.Append($" { model[i].Operator}");
                builder.Append($" { model[i].Value} ");

                if (i != model.Count - 1)
                {
                    builder.Append($" { model[i].ConditionOperator} ");
                }
            }

            return builder.ToString();
        }
    }
}
