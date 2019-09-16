using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ExpressionToWhereClause
{
    internal class ExpressionEntry : ExpressionVisitor
    {
        private Dictionary<string, object> _parameters = null;
        private bool? NonParametric = null;
        private ISqlAdapter SqlAdapter = null;

        public ExpressionEntry(bool? nonParametric, ISqlAdapter sqlAdapter)
        {
            _parameters = new Dictionary<string, object>();
            NonParametric = nonParametric;
            SqlAdapter = sqlAdapter;
        }
        

        private string whereClause = string.Empty;

        public (string,Dictionary<string,object>) GetResult()
        {            
            return (whereClause, _parameters);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            whereClause = GetWhereClauseByExpression(node.Body, _parameters, NonParametric, SqlAdapter).ToString();
            return node;
        }


        internal static object GetConstantByExpression(Expression expression)
        {
            ConstantExpressionVisitor constantExpressionVisitor = null;
            if (expression is ConstantExpression)
            {
                constantExpressionVisitor = new ConstantExpressionVisitor();
            }
            else if (expression is MemberExpression)
            {
                constantExpressionVisitor = new MemberConstantExpressionVisitor();
            }
            else if (expression is MethodCallExpression)
            {
                constantExpressionVisitor = new MethodConstantExpressionVisitor();
            }
            else if (expression is ConditionalExpression)
            {
                constantExpressionVisitor = new ConditionalConstantExpressionVisitor();
            }
            else if( expression.GetType().Name == "MethodBinaryExpression")
            {
                constantExpressionVisitor = new MethodBinaryConstantExpressionVisitor();
            }
            else
            {
                throw new Exception($"Unknow expression {expression.GetType()}");
            }
            constantExpressionVisitor.Visit(expression);
            return constantExpressionVisitor.Value;
        }

        internal static StringBuilder GetWhereClauseByExpression(Expression expression,Dictionary<string,object> p, bool? nonP, ISqlAdapter sqlAdapter)
        {
            BaseExpressionVisitor expressionVisitor = null;
            if (expression is BinaryExpression)
            {
                expressionVisitor = new BinaryExpressionVisitor(nonP,p,sqlAdapter);
            }
            else if (expression is MemberExpression)
            {
                expressionVisitor = new BooleanMemberExpressionVisitor(nonP,p,sqlAdapter);
            }
            else if (expression is MethodCallExpression)
            {
                expressionVisitor = new MethodCallExpressionVisitor(nonP, p, sqlAdapter);
            }
            else if (expression is UnaryExpression unaryExpression
                && unaryExpression.Operand is MemberExpression
                && unaryExpression.Type == typeof(bool))
            {
                expressionVisitor = new UnaryBooleanMemberExpressionVisitor(nonP, p, sqlAdapter);
            }
            else
            {
                throw new NotSupportedException($"Unknow expression {expression.GetType()}");
            }

            expressionVisitor.Visit(expression);
            return expressionVisitor.GetResult();
        }
    }
}
