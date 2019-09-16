using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionToWhereClause
{
    public static class ExpressionExtensions
    {
        public static (string, Dictionary<string, object>) ToWhereClause<T>(this Expression<Func<T, bool>> expression, bool? nonParametric = null, ISqlAdapter sqlAdapter = null) where T : class
        {
            ExpressionEntry expressionEntry = new ExpressionEntry(nonParametric ?? ExpressionConfigurations.NonParametric, sqlAdapter ?? ExpressionConfigurations.Adapter);
            expressionEntry.Visit(expression);
            
            return expressionEntry.GetResult();
        }

        public static string ToWhereClauseNonParametric<T>(this Expression<Func<T, bool>> expression, ISqlAdapter sqlAdapter = null) where T : class
        {
            return ToWhereClause(expression, true, sqlAdapter).Item1;
        }
    }
}
