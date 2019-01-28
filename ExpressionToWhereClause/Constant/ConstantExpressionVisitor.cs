using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionToWhereClause
{
    internal class ConstantExpressionVisitor: ExpressionVisitor
    {
        public object Value { get; set; }
        protected override Expression VisitConstant(ConstantExpression node)
        {
            Value = node.Value;
            return node;
        }
    }
}
