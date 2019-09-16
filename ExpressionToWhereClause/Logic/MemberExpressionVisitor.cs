using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ExpressionToWhereClause
{
    internal class MemberExpressionVisitor : BaseExpressionVisitor
    {
        public MemberExpressionVisitor(bool? nonParametric, Dictionary<string, object> parameters, ISqlAdapter sqlAdapter) : base(nonParametric, parameters, sqlAdapter)
        {

        }
        protected override Expression VisitMember(MemberExpression node)
        {
            MemberInfo memberInfo = node.Member;
            sb.Append(SqlAdapter.GetColumnName(memberInfo));
            MemberInfo = memberInfo;
            return node;
        }

        public MemberInfo MemberInfo { get; private set; }
    }
}
