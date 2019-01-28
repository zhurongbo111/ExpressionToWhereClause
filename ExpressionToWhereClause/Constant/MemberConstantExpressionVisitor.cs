using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ExpressionToWhereClause
{
    internal class MemberConstantExpressionVisitor : ConstantExpressionVisitor
    {
        protected override Expression VisitMember(MemberExpression node)
        {
            var visitor = new MemberConstantExpressionVisitor();
            visitor.Visit(node.Expression);
            Type type = visitor.Value.GetType();
            switch (node.Member.MemberType)
            {
                case MemberTypes.Field:
                    FieldInfo fieldInfo = type.GetField(node.Member.Name);
                    Value = fieldInfo.GetValue(visitor.Value);
                    break;
                case MemberTypes.Property:
                    PropertyInfo propertyInfo = type.GetProperty(node.Member.Name);
                    Value = propertyInfo.GetValue(visitor.Value);
                    break;
                default:
                    throw new NotSupportedException($"Unknow Member type {node.Member.MemberType}");
            }
            return node;
        }
    }
}
