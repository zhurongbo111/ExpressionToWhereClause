using System;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionToWhereClause
{
    internal class BaseExpressionVisitor : ExpressionVisitor
    {
        protected StringBuilder sb;

        public BaseExpressionVisitor()
        {
            sb = new StringBuilder();
        }

        public virtual StringBuilder GetResult()
        {
           
            if (ExpressionEntry.NonParametric != null && ExpressionEntry.NonParametric.Value)
            {
                foreach (var kv in ExpressionEntry.Parameters)
                {
                    sb = sb.Replace($"{kv.Key}", $"'{kv.Value.ToString()}'");
                    sb = sb.Replace("%''", "%");
                    sb = sb.Replace("''%", "%");
                }
                ExpressionEntry.Parameters.Clear();
            }
            return sb;
        }

        protected string ConvertExpressionTypeToSymbol(ExpressionType expressionType)
        {
            
            switch(expressionType)
            {
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.AndAlso:
                    return "AND";
                case ExpressionType.OrElse:
                    return "OR";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.NotEqual:
                    return "<>";
                default:
                    throw new NotSupportedException($"Unknown ExpressionType {expressionType}");
            }
        }

        protected bool IsDataComparator(ExpressionType expressionType)
        {
            switch (expressionType)
            {
                case ExpressionType.Equal:
                case ExpressionType.LessThan:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.NotEqual:
                    return true;
                default:
                    return false;
            }
        }
        protected bool IsLogicType(ExpressionType expressionType)
        {
            switch (expressionType)
            {
                case ExpressionType.OrElse:
                case ExpressionType.AndAlso:
                    return true;
                default:
                    return false;
            }
        }
    }
}
