using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ExpressionToWhereClause
{
    internal class BaseExpressionVisitor : ExpressionVisitor
    {
        protected StringBuilder sb;

        protected bool? NonParametric;

        protected Dictionary<string, object> Parameters;

        protected ISqlAdapter SqlAdapter;

        public BaseExpressionVisitor(bool? nonParametric, Dictionary<string,object> parameters, ISqlAdapter sqlAdapter)
        {
            sb = new StringBuilder();
            NonParametric = nonParametric;
            Parameters = parameters;
            SqlAdapter = sqlAdapter;
        }

        public virtual StringBuilder GetResult()
        {
           
            if (NonParametric != null && NonParametric.Value)
            {
                foreach (var kv in Parameters)
                {
                    sb = sb.Replace($"{kv.Key}", $"'{kv.Value.ToString()}'");
                    sb = sb.Replace("%''", "%");
                    sb = sb.Replace("''%", "%");
                }
                Parameters.Clear();
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

        protected string EnsurePatameter(MemberInfo mi)
        {
            string key = SqlAdapter.GetParameterName(mi);
            int seed = 1;
            while (Parameters.ContainsKey($"@{key}"))
            {
                key = key + seed;
                seed++;
            }
            return key;
        }
    }
}
