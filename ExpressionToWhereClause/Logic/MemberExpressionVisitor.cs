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
            sb.Append(ExpressionEntry.SqlAdapter.GetColumnName(memberInfo));
            MemberInfo = memberInfo;
            return node;
        }

        public MemberInfo MemberInfo { get; private set; }
    }
}
