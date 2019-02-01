using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ExpressionToWhereClause
{
    internal class ConditionalConstantExpressionVisitor : ConstantExpressionVisitor
    {
        protected override Expression VisitConditional(ConditionalExpression node)
        {
            bool condition = (bool)ExpressionEntry.GetConstantByExpression(node.Test);
            if(condition)
            {
                Value = ExpressionEntry.GetConstantByExpression(node.IfTrue);
            }
            else
            {
                Value = ExpressionEntry.GetConstantByExpression(node.IfFalse);
            }
            return node;
        }
    }
}
