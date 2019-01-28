using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ExpressionToWhereClause
{
    internal class MethodConstantExpressionVisitor : ConstantExpressionVisitor
    {
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            MethodInfo mi = node.Method;
            object instance = null;
            object[] parameters = null;
            if(node.Object != null)
            {
                instance = ExpressionEntry.GetConstantByExpression(node.Object);
            }
            if(node.Arguments != null && node.Arguments.Count > 0)
            {
                parameters = new object[node.Arguments.Count];
                for(int i=0;i<node.Arguments.Count;i++)
                {
                    Expression expression = node.Arguments[i];
                    parameters[i] = ExpressionEntry.GetConstantByExpression(expression);
                }
            }

            Value = mi.Invoke(instance, parameters);

            return node;
        }
    }
}
