using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ExpressionToWhereClause
{
    public static class WhereCaluseParser
    {
        public static StringBuilder Parse(Expression expression, WhereClauseAdhesive adhesive)
        {
            if (expression is BinaryExpression binaryExpression)
            {
                if (IsLogicType(binaryExpression.NodeType))
                {
                    StringBuilder sqlBuilder = new StringBuilder();
                    var leftClause = Parse(binaryExpression.Left, adhesive);
                    sqlBuilder.Append($"({leftClause})");

                    sqlBuilder.Append($" {binaryExpression.NodeType.ToLogicSymbol()} ");

                    var rightClause = Parse(binaryExpression.Right, adhesive);
                    sqlBuilder.Append($"({rightClause})");

                    return sqlBuilder;
                }
                else if (binaryExpression.Left is UnaryExpression convertExpression
                    && convertExpression.NodeType == ExpressionType.Convert
                    && convertExpression.Operand.Type.IsEnum
                    && convertExpression.Operand is MemberExpression enumMemberExpression
                    && IsDataComparator(binaryExpression.NodeType))
                {
                    //Support the enum Property, For example: u.UserType == UserType.Admin
                    return ConditionBuilder.BuildCondition(enumMemberExpression.Member, adhesive, binaryExpression.NodeType, ConstantExtractor.ParseConstant(binaryExpression.Right));
                }
                else if (binaryExpression.Left is MemberExpression memberExpression && IsDataComparator(binaryExpression.NodeType))
                {
                    //Basic case, For example: u.Age > 18
                    return ConditionBuilder.BuildCondition(memberExpression.Member, adhesive, binaryExpression.NodeType, ConstantExtractor.ParseConstant(binaryExpression.Right));
                }
                else
                {
                    throw new NotSupportedException($"Unknow Left:{binaryExpression.Left.GetType()} Right:{binaryExpression.Right.GetType()} NodeType:{binaryExpression.NodeType}");
                }
            }
            else if (expression is MethodCallExpression methodCallExpression)
            {
                if (methodCallExpression.Method.DeclaringType == typeof(string)
                    && (methodCallExpression.Method.Name == "Contains"
                      || methodCallExpression.Method.Name == "StartsWith"
                      || methodCallExpression.Method.Name == "EndsWith")
                      || methodCallExpression.Method.Name == "Equals")
                {
                    //"Like" condition for string property, For example: u.Name.Contains("A")
                    return ConditionBuilder.BuildLikeOrEqualCondition(methodCallExpression, adhesive);
                }
                else if (methodCallExpression.Method.DeclaringType == typeof(System.Linq.Enumerable)
                    && methodCallExpression.Arguments?.Count == 2
                    && methodCallExpression.Method.Name == "Contains")
                {
                    //"In" condition, Support the `Contains` extension Method of IEnumerable<TSource> Type
                    //For example: List<string> values = new List<string> { "foo", "bar"};
                    //             values.Contains(u.Name)  
                    return ConditionBuilder.BuildInCondition(methodCallExpression.Arguments[1] as MemberExpression, methodCallExpression.Arguments[0], adhesive);
                }
                else if (methodCallExpression.Method.DeclaringType.IsGenericType
                    && methodCallExpression.Method.DeclaringType.GetGenericTypeDefinition() == typeof(List<>)
                    && methodCallExpression.Arguments?.Count == 1
                    && methodCallExpression.Method.Name == "Contains")
                {
                    //"In" Condition, Support the `Contains` Method of List<T> type
                    //For example: string[] values = new string[]{ "foo", "bar"};
                    //             values.Contains(u.Name)  
                    return ConditionBuilder.BuildInCondition(methodCallExpression.Arguments[0] as MemberExpression, methodCallExpression.Object, adhesive);
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
            else if (expression is MemberExpression trueMemberExpression && trueMemberExpression.Type == typeof(bool))
            {
                //Support bool type property, For example: u.Sex
                return ConditionBuilder.BuildCondition(trueMemberExpression.Member, adhesive, ExpressionType.Equal, true);
            }
            else if (expression is UnaryExpression unaryExpression
                && unaryExpression.NodeType == ExpressionType.Not
                && unaryExpression.Operand is MemberExpression falseMemberExpression
                && unaryExpression.Type == typeof(bool))
            {
                //Support bool type property, For example: !u.Sex
                return ConditionBuilder.BuildCondition(falseMemberExpression.Member, adhesive, ExpressionType.Equal, false);
            }
            else
            {
                throw new NotSupportedException($"Unknow expression {expression.GetType()}");
            }
        }

        private static bool IsDataComparator(ExpressionType expressionType)
        {
            switch (expressionType)
            {
                case ExpressionType.Equal:
                case ExpressionType.LessThan:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.NotEqual:
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsLogicType(ExpressionType expressionType)
        {
            switch (expressionType)
            {
                case ExpressionType.OrElse:
                case ExpressionType.AndAlso:
                    return true;
                default:
                    return false;
            }
        }

        private static string ToLogicSymbol(this ExpressionType expressionType)
        {
            switch (expressionType)
            {
                case ExpressionType.AndAlso:
                    return "AND";
                case ExpressionType.OrElse:
                    return "OR";
                default:
                    throw new NotSupportedException($"Unknown ExpressionType {expressionType}");
            }
        }
    }
}
