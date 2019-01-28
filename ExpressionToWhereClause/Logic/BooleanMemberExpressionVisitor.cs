using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionToWhereClause
{
    internal class BooleanMemberExpressionVisitor : MemberExpressionVisitor
    {
        protected override Expression VisitMember(MemberExpression node)
        {
            base.VisitMember(node);
            string fieldName = GetResult();
            sb.Clear();

            string parameterName = ExpressionEntry.EnsureKey(fieldName);
            sb.Append($"{fieldName} = @{parameterName}");
            ExpressionEntry.Parameters.Add($"@{parameterName}", true);
            return node;
        }
    }
}
