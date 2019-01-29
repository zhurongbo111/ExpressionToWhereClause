using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ExpressionToWhereClause
{
    internal class IntangibleConstantExpressionVisitor : ExpressionVisitor
    {
        public object Value { get; private set; }
        protected override Expression VisitConstant(ConstantExpression node)
        {
            Value = node.Value;
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            MethodInfo mi = node.Method;
            object instance = null;
            object[] parameters = null;
            if (node.Object != null)
            {
                instance = InternalGetConstantByExpression(node.Object);
            }
            if (node.Arguments != null && node.Arguments.Count > 0)
            {
                parameters = new object[node.Arguments.Count];
                for (int i = 0; i < node.Arguments.Count; i++)
                {
                    Expression expression = node.Arguments[i];
                    parameters[i] = InternalGetConstantByExpression(expression);
                }
            }

            Value = mi.Invoke(instance, parameters);

            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            object v = InternalGetConstantByExpression(node.Expression);
            Type type = v.GetType();
            switch (node.Member.MemberType)
            {
                case MemberTypes.Field:
                    FieldInfo fieldInfo = type.GetField(node.Member.Name);
                    Value = fieldInfo.GetValue(v);
                    break;
                case MemberTypes.Property:
                    PropertyInfo propertyInfo = type.GetProperty(node.Member.Name);
                    Value = propertyInfo.GetValue(v);
                    break;
                default:
                    throw new NotSupportedException($"Unknow Member type {node.Member.MemberType}");
            }
            return node;
        }

        private object InternalGetConstantByExpression(Expression expression)
        {
            if (expression is ConstantExpression
                || expression is MemberExpression
                || expression is MethodCallExpression)
            {
                IntangibleConstantExpressionVisitor constantExpressionVisitor = new IntangibleConstantExpressionVisitor();
                constantExpressionVisitor.Visit(expression);
                return constantExpressionVisitor.Value;
            }
            else
            {
                throw new Exception($"Unknow expression {expression.GetType()}");
            }

        }
    }

    
}
