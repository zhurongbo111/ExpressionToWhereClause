using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionToWhereClause
{
    internal class BinaryExpressionVisitor : BaseExpressionVisitor
    {
        public BinaryExpressionVisitor(bool? nonParametric, Dictionary<string, object> parameters, ISqlAdapter sqlAdapter) : base(nonParametric, parameters, sqlAdapter)
        {

        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.Left is MemberExpression && IsDataComparator(node.NodeType))
            {
                sb.Append(InternalGetSqlByExpression(node));
            }
            else if (IsLogicType(node.NodeType))
            {
                var leftClause = ExpressionEntry.GetWhereClauseByExpression(node.Left,Parameters, NonParametric,SqlAdapter);
                sb.Append($"({leftClause})");

                sb.Append($" {ConvertExpressionTypeToSymbol(node.NodeType)} ");

                var rightClause = ExpressionEntry.GetWhereClauseByExpression(node.Right,Parameters, NonParametric,SqlAdapter);
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
            MemberExpressionVisitor memberExpressionVisitor = new MemberExpressionVisitor(NonParametric, Parameters,SqlAdapter);
            memberExpressionVisitor.Visit(node.Left);
            string fieldName = memberExpressionVisitor.GetResult().ToString();
            string parameterName = EnsurePatameter(memberExpressionVisitor.MemberInfo);
            string sql = $"{fieldName} {ConvertExpressionTypeToSymbol(node.NodeType)} @{parameterName}";
            Parameters.Add($"@{parameterName}", ExpressionEntry.GetConstantByExpression(node.Right));
            return sql;
        }
    }
}
