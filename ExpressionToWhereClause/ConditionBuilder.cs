using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ExpressionToWhereClause
{
    public static class ConditionBuilder
    {
        public static StringBuilder BuildCondition(MemberInfo memberInfo, WhereClauseAdhesive adhesive, ExpressionType comparison, object value)
        {
            string fieldName = adhesive.SqlAdapter.FormatColumnName(memberInfo);
            string parameterName = EnsureParameter(memberInfo, adhesive);
            adhesive.Parameters.Add($"@{parameterName}", value);
            return new StringBuilder($"{fieldName} {comparison.ToComparisonSymbol()} @{parameterName}");
        }

        public static StringBuilder BuildLikeOrEqualCondition(MethodCallExpression methodCallExpression, WhereClauseAdhesive adhesive)
        {
            string symbol;
            string valueSymbol;
            switch (methodCallExpression.Method.Name)
            {
                case "Equals":
                    symbol = "= {0}";
                    valueSymbol = "{0}";
                    break;
                case "StartsWith":
                    symbol = "like {0}";
                    valueSymbol = "{0}%";
                    break;
                case "EndsWith":
                    symbol = "like {0}";
                    valueSymbol = "%{0}";
                    break;
                case "Contains":
                    symbol = "like {0}";
                    valueSymbol = "%{0}%";
                    break;
                default:
                    throw new NotSupportedException($"Not support method name:{methodCallExpression.Method.Name}");
            }

            if (methodCallExpression.Object is MemberExpression memberExpression)
            {
                var memberInfo = memberExpression.Member;
                string fieldName = adhesive.SqlAdapter.FormatColumnName(memberInfo);
                string parameterName = EnsureParameter(memberInfo, adhesive);
                object value = ConstantExtractor.ParseConstant(methodCallExpression.Arguments[0]);
                adhesive.Parameters.Add($"@{parameterName}", string.Format(valueSymbol, value));
                return new StringBuilder(string.Format($"{fieldName} {symbol}", $"@{parameterName}"));
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public static StringBuilder BuildInCondition(MemberExpression memberExpression, Expression valueExpression, WhereClauseAdhesive adhesive, bool isNotContains)
        {
            var memberInfo = memberExpression.Member;
            string fieldName = adhesive.SqlAdapter.FormatColumnName(memberInfo);
            string parameterName = EnsureParameter(memberInfo, adhesive);
            object value = ConstantExtractor.ParseConstant(valueExpression);
            adhesive.Parameters.Add($"@{parameterName}", value);
            return isNotContains ? new StringBuilder(string.Format("{0} not in {1}", fieldName, $"@{parameterName}")) : new StringBuilder(string.Format("{0} in {1}", fieldName, $"@{parameterName}"));
        }

        private static string EnsureParameter(MemberInfo mi, WhereClauseAdhesive adhesive)
        {
            string key = mi.Name;
            int seed = 1;
            string tempKey = key;
            while (adhesive.Parameters.ContainsKey($"@{tempKey}"))
            {
                tempKey = key + seed;
                seed++;
            }
            return tempKey;
        }

        private static string ToComparisonSymbol(this ExpressionType expressionType)
        {
            switch (expressionType)
            {
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.NotEqual:
                    return "<>";
                default:
                    throw new NotSupportedException($"Unknown ExpressionType {expressionType}");
            }
        }
    }
}
