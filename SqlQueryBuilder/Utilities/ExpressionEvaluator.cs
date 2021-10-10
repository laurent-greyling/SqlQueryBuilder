using SqlQueryBuilder.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;

namespace SqlQueryBuilder.Utilities
{
    public static class ExpressionEvaluator
    {
        public static readonly ImmutableDictionary<ExpressionType, string> Operators = new Dictionary<ExpressionType, string>
        {
            { ExpressionType.Equal, "=" },
            { ExpressionType.And, "AND" },
            { ExpressionType.Or, "OR" },
            { ExpressionType.AndAlso, "AND" },
            { ExpressionType.OrElse, "OR" },
            { ExpressionType.GreaterThan, ">" },
            { ExpressionType.LessThan, "<" },
            { ExpressionType.GreaterThanOrEqual, ">=" },
            { ExpressionType.LessThanOrEqual, "<=" },
            { ExpressionType.NotEqual, "<>" },
        }.ToImmutableDictionary();

        public static ExpressionModel MainExpression(BinaryExpression binaryExpression)
        {
            var left = binaryExpression.Right;
            var right = binaryExpression.Left;

            return new ExpressionModel
            {
                Left = left,
                Right = right,
                Node = binaryExpression.NodeType
            };
        }

        public static (string expressionKey, object expressionValue) ExpressionKeyValue(BinaryExpression binaryExpression)
        {
            var key = (MemberExpression)binaryExpression.Left;

            if (binaryExpression.Right.GetType() == typeof(ConstantExpression))
            {
                return (key.Member.Name, ((ConstantExpression)binaryExpression.Right).Value);
            }

            var value = (MemberExpression)binaryExpression.Right;
            if (value.Expression.GetType() == typeof(ConstantExpression))
            {
                return ConstantExpressionTypeHandler(key, value);
            }

            return MemberExpressionTypeHandler(key, value);
        }

        private static (string expressionKey, object expressionValue) MemberExpressionTypeHandler(MemberExpression key, MemberExpression value)
        {
            var memberExpression = (MemberExpression)value.Expression;
            var captureConst = (ConstantExpression)memberExpression.Expression;
            var details = ((FieldInfo)memberExpression.Member).GetValue(captureConst.Value);
            var valueMember = ((PropertyInfo)value.Member).GetValue(details, null);

            return (key.Member.Name, valueMember);
        }

        private static (string expressionKey, object expressionValue) ConstantExpressionTypeHandler(MemberExpression key, MemberExpression value)
        {
            var captureConst = (ConstantExpression)value.Expression;
            var details = ((FieldInfo)value.Member).GetValue(captureConst.Value);

            return (key.Member.Name, details);
        }

        public static void Evaluate(BinaryExpression binaryExpression,
            List<SqlQueryModel> sqlModels,
            Dictionary<string, object> paramaterDictionary,
            string nodeType = "")
        {
            var left = binaryExpression.Right;
            var right = binaryExpression.Left;

            if (right.GetType().Name == nameof(ExpressionValues.LogicalBinaryExpression))
            {
                var binaryExpressionRight = (BinaryExpression)right;
                Evaluate(binaryExpressionRight, sqlModels, paramaterDictionary, Operators[binaryExpression.NodeType]);
            }

            if (left.GetType().Name == nameof(ExpressionValues.LogicalBinaryExpression))
            {
                var binaryExpressionLeft = (BinaryExpression)left;
                Evaluate(binaryExpressionLeft, sqlModels, paramaterDictionary, Operators[binaryExpression.NodeType]);
            }

            if (right.GetType().Name == nameof(ExpressionValues.MethodBinaryExpression))
            {
                var binaryExpressionRight = (BinaryExpression)right;
                Evaluate(binaryExpressionRight, sqlModels, paramaterDictionary, Operators[binaryExpression.NodeType]);
            }

            if (left.GetType().Name == nameof(ExpressionValues.MethodBinaryExpression))
            {
                var binaryExpressionLeft = (BinaryExpression)left;
                Evaluate(binaryExpressionLeft, sqlModels, paramaterDictionary, Operators[binaryExpression.NodeType]);
            }

            if (IsFinalExpression(right, left))
            {
                (string expressionKey, object expressionValue) = ExpressionKeyValue(binaryExpression);

                var sqlQueryModel = new SqlQueryModel
                {
                    Key = expressionKey,
                    Operator = Operators[binaryExpression.NodeType],
                    Value = $"@{expressionKey}",
                    ConditionOperator = nodeType
                };

                sqlModels.Add(sqlQueryModel);

                if (!paramaterDictionary.ContainsValue(expressionValue))
                {
                    expressionValue = expressionValue is bool
                        ? Convert.ToByte(expressionValue)
                        : expressionValue;

                    sqlQueryModel.Value = paramaterDictionary.ContainsKey(sqlQueryModel.Value)
                    ? $"{ sqlQueryModel.Value }{ Guid.NewGuid().ToString().Replace("-", "") }"
                    : sqlQueryModel.Value;

                    paramaterDictionary.Add(sqlQueryModel.Value, expressionValue);
                }
            }
        }

        public static bool IsFinalExpression(Expression right, Expression left)
            => (IsPropertyExpression(right, left))
                || (IsFieldPropertyExpression(right, left))
                || (IsConstantPropertyExpression(right, left));

        private static bool IsPropertyExpression(Expression right, Expression left)
            => right.GetType().Name == nameof(ExpressionValues.PropertyExpression)
            && left.GetType().Name == nameof(ExpressionValues.PropertyExpression);

        private static bool IsFieldPropertyExpression(Expression right, Expression left)
            => left.GetType().Name == nameof(ExpressionValues.FieldExpression)
            && right.GetType().Name == nameof(ExpressionValues.PropertyExpression);

        private static bool IsConstantPropertyExpression(Expression right, Expression left)
            => left.GetType().Name == nameof(ExpressionValues.ConstantExpression)
            && right.GetType().Name == nameof(ExpressionValues.PropertyExpression);
    }
}
