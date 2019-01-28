using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionToWhereClause
{
    internal class BinaryExpressionVisitor : BaseExpressionVisitor
    {
        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.Left is MemberExpression && IsDataComparator(node.NodeType))
            {
                sb.Append(InternalGetSqlByExpression(node));
            }
            else if (IsLogicType(node.NodeType))
            {
                string leftClause = GetSqlByExpression(node.Left);
                sb.Append($"({leftClause})");

                sb.Append($" {ConvertExpressionTypeToSymbol(node.NodeType)} ");

                string rightClause = GetSqlByExpression(node.Right);
                sb.Append($"({rightClause})");
            }
            else
            {
                throw new NotSupportedException($"Unknow Left:{node.Left.GetType()} Right:{node.Right.GetType()} NodeType:{node.NodeType}");
            }
            return node;
        }

        private string InternalGetSqlByExpression(BinaryExpression node)
        {
            MemberExpressionVisitor memberExpressionVisitor = new MemberExpressionVisitor();
            memberExpressionVisitor.Visit(node.Left);
            string fieldName = memberExpressionVisitor.GetResult();
            string parameterName = ExpressionEntry.EnsureKey(fieldName);
            string sql = $"{fieldName} {ConvertExpressionTypeToSymbol(node.NodeType)} @{parameterName}";
            ExpressionEntry.Parameters.Add($"@{parameterName}", ExpressionEntry.GetConstantByExpression(node.Right));
            return sql;
        }


        private string GetSqlByExpression(Expression expression)
        {
            BaseExpressionVisitor expressionVisitor = null;
            if(expression is BinaryExpression)
            {
                expressionVisitor = new BinaryExpressionVisitor();
            }
            else if(expression is MemberExpression)
            {
                expressionVisitor = new BooleanMemberExpressionVisitor();
            }
            else if(expression is MethodCallExpression)
            {
                expressionVisitor = new MethodCallExpressionVisitor();
            }
            else
            {
                throw new NotSupportedException($"Unknow expression {expression.GetType()}");
            }

            expressionVisitor.Visit(expression);
            string sql = expressionVisitor.GetResult();
            return sql;
        }
    }
}
