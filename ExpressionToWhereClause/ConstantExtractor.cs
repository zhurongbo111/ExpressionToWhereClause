using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ExpressionToWhereClause
{
    public static class ConstantExtractor
    {
        public static object ParseConstant(Expression expression)
        {
            if (expression is ConstantExpression constantExpression)
            {
                return ParseConstantExpression(constantExpression);
            }
            else if (expression is MemberExpression memberExpression)
            {
                return ParseMemberConstantExpression(memberExpression);
            }
            else if (expression is MethodCallExpression methodCallExpression)
            {
                return ParseMethodCallConstantExpression(methodCallExpression);
            }
            else if (expression is ConditionalExpression conditionalExpression)
            {
                return ParseConditionalExpression(conditionalExpression);
            }
            else if (expression is BinaryExpression methodBinaryExpression && expression.GetType().Name == "MethodBinaryExpression")
            {
                return ParseMethodBinaryExpression(methodBinaryExpression);
            }
            else if (expression is BinaryExpression simpleBinaryExpression
                && simpleBinaryExpression.GetType().Name == "SimpleBinaryExpression")
            {
                return ParseSimpleBinaryExpression(simpleBinaryExpression);
            }
            else if (expression is UnaryExpression convertExpression
                && expression.NodeType == ExpressionType.Convert)
            {
                return ParseConvertExpression(convertExpression);
            }
            else
            {
                throw new NotSupportedException($"Unknow expression {expression.GetType()}");
            }
        }

        private static object ParseConstantExpression(ConstantExpression constantExpression)
        {
            return constantExpression.Value;
        }

        /// <summary>
        /// for example: get the age value from u.Age
        /// </summary>
        /// <param name="memberExpression"></param>
        /// <returns></returns>
        private static object ParseMemberConstantExpression(MemberExpression memberExpression)
        {
            // Firstly: Get the value of u
            object value = ParseConstant(memberExpression.Expression);

            //Secondly: get Age using reflect
            Type type = value.GetType();
            switch (memberExpression.Member.MemberType)
            {
                case MemberTypes.Field:
                    FieldInfo fieldInfo = type.GetField(memberExpression.Member.Name);
                    return fieldInfo.GetValue(value);
                case MemberTypes.Property:
                    PropertyInfo propertyInfo = type.GetProperty(memberExpression.Member.Name);
                    return propertyInfo.GetValue(value);
                default:
                    throw new NotSupportedException($"Unknow Member type {memberExpression.Member.MemberType}");
            }
        }

        /// <summary>
        /// For example: execute the method to get the value, like: u.Name.SubString(1,2), call the 'SubString' mehod
        /// </summary>
        /// <param name="methodCallExpression"></param>
        /// <returns></returns>
        private static object ParseMethodCallConstantExpression(MethodCallExpression methodCallExpression)
        {
            MethodInfo mi = methodCallExpression.Method;
            object instance = null;
            object[] parameters = null;
            if (methodCallExpression.Object != null)
            {
                instance = ParseConstant(methodCallExpression.Object);
            }
            if (methodCallExpression.Arguments != null && methodCallExpression.Arguments.Count > 0)
            {
                parameters = new object[methodCallExpression.Arguments.Count];
                for (int i = 0; i < methodCallExpression.Arguments.Count; i++)
                {
                    Expression expression = methodCallExpression.Arguments[i];
                    parameters[i] = ParseConstant(expression);
                }
            }

            return mi.Invoke(instance, parameters);
        }

        private static object ParseConditionalExpression(ConditionalExpression conditionalExpression)
        {
            bool condition = (bool)ParseConstant(conditionalExpression.Test);
            if (condition)
            {
                return ParseConstant(conditionalExpression.IfTrue);
            }
            else
            {
                return ParseConstant(conditionalExpression.IfFalse);
            }
        }

        private static object ParseMethodBinaryExpression(BinaryExpression methodBinaryExpression)
        {
            object left = ParseConstant(methodBinaryExpression.Left);
            object right = ParseConstant(methodBinaryExpression.Right);
            MethodInfo methodInfo = methodBinaryExpression.Method;
            if (methodInfo.IsStatic)
            {
                return methodInfo.Invoke(null, new object[] { left, right });
            }
            else
            {
                return methodInfo.Invoke(left, new object[] { right });
            }
        }

        private static object ParseSimpleBinaryExpression(BinaryExpression simpleBinaryExpression)
        {
            if (simpleBinaryExpression.NodeType == ExpressionType.ArrayIndex)
            {
                var array = ParseConstant(simpleBinaryExpression.Left) as Array;
                var index = (int)ParseConstant(simpleBinaryExpression.Right);
                return array.GetValue(index);
            }
            else
            {
                return new NotSupportedException();
            }
        }

        private static object ParseConvertExpression(UnaryExpression convertExpression)
        {
            object value = ParseConstant(convertExpression.Operand);

            return Convert.ChangeType(value, convertExpression.Type);
        }
    }
}
