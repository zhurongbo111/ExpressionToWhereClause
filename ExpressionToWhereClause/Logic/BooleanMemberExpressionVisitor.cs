using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionToWhereClause
{
    internal class BooleanMemberExpressionVisitor : MemberExpressionVisitor
    {
        public BooleanMemberExpressionVisitor(bool? nonParametric, Dictionary<string, object> parameters, ISqlAdapter sqlAdapter) : base(nonParametric, parameters, sqlAdapter)
        {

        }
        protected virtual bool GetConstant()
        {
            return true;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            base.VisitMember(node);
            string fieldName = GetResult().ToString();
            sb.Clear();

            string parameterName = EnsurePatameter(MemberInfo);
            sb.Append($"{fieldName} = @{parameterName}");
            Parameters.Add($"@{parameterName}", GetConstant());
            return node;
        }
    }
}
