using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionToWhereClause
{
    internal class MethodCallExpressionVisitor : BaseExpressionVisitor
    {
        public MethodCallExpressionVisitor(bool? nonParametric, Dictionary<string, object> parameters, ISqlAdapter sqlAdapter) : base(nonParametric, parameters,sqlAdapter)
        {

        }
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            string symbol = string.Empty;
            switch (node.Method.Name)
            {
                case "Equals":
                    symbol = "= {0}";
                    break;
                case "StartsWith":
                    symbol = "like {0}'%'";
                    break;
                case "EndsWith":
                    symbol = "like '%'{0}";
                    break;
                case "Contains":
                    symbol = "like '%'{0}'%'";
                    break;
                default:
                    throw new NotSupportedException($"Not support method name:{node.Method.Name}");
            }

            if (node.Object is MemberExpression)
            {
                MemberExpressionVisitor memberExpressionVisitor = new MemberExpressionVisitor(NonParametric, Parameters, SqlAdapter);
                memberExpressionVisitor.Visit(node.Object);
                string fieldName = memberExpressionVisitor.GetResult().ToString();
                string parameterName = EnsurePatameter(memberExpressionVisitor.MemberInfo);
                string sql = string.Format($"{fieldName} {symbol}", $"@{parameterName}");
                sb.Append(sql);
                Parameters.Add($"@{parameterName}", ExpressionEntry.GetConstantByExpression(node.Arguments[0]));
            }
            
            return node;
        }
    }
}
