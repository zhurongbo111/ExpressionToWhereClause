using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ExpressionToWhereClause
{
    internal class MethodBinaryConstantExpressionVisitor : ConstantExpressionVisitor
    {
        protected override Expression VisitBinary(BinaryExpression node)
        {
            object left = ExpressionEntry.GetConstantByExpression(node.Left);
            object right = ExpressionEntry.GetConstantByExpression(node.Right);
            MethodInfo methodInfo = node.Method;
            if (methodInfo.IsStatic)
            {
                Value = methodInfo.Invoke(null, new object[] { left, right });
            }
            else
            {
                Value = methodInfo.Invoke(left, new object[] { right });
            }

            return node;
        }
    }
}
