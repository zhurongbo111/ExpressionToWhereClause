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
                var leftClause = ExpressionEntry.GetWhereClauseByExpression(node.Left);
                sb.Append($"({leftClause})");

                sb.Append($" {ConvertExpressionTypeToSymbol(node.NodeType)} ");

                var rightClause = ExpressionEntry.GetWhereClauseByExpression(node.Right);
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
            string fieldName = memberExpressionVisitor.GetResult().ToString();
            string parameterName = ExpressionEntry.EnsureKey(fieldName);
            string sql = $"{fieldName} {ConvertExpressionTypeToSymbol(node.NodeType)} @{parameterName}";
            ExpressionEntry.Parameters.Add($"@{parameterName}", ExpressionEntry.GetConstantByExpression(node.Right));
            return sql;
        }
    }
}
