using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionToWhereClause
{
    public static class ExpressionExtensions
    {
        public static (string, Dictionary<string, object>) ToWhereClause<T>(this Expression<Func<T, bool>> expression, bool? nonParametric = null) where T : class
        {
            ExpressionEntry.NonParametric = nonParametric ?? ExpressionConfigurations.NonParametric;
            ExpressionEntry expressionEntry = new ExpressionEntry();
            expressionEntry.Visit(expression);
            
            return expressionEntry.GetResult();
        }

        public static string ToWhereClauseNonParametric<T>(this Expression<Func<T, bool>> expression) where T : class
        {
            return ToWhereClause(expression, true).Item1;
        }
    }
}
