using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ExpressionToWhereClause
{
    internal class MemberExpressionVisitor : BaseExpressionVisitor
    {
        protected override Expression VisitMember(MemberExpression node)
        {
            MemberInfo memberInfo = node.Member;
            sb.Append(ExpressionEntry.FieldNameSelector == null ? 
                memberInfo.Name : ExpressionEntry.FieldNameSelector.Invoke(memberInfo));
            return node;
        }
    }
}
