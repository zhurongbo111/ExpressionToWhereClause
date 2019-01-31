using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionToWhereClause
{
    public static class ExpressionExtensions
    {
        public static (string, Dictionary<string, object>) ToWhereClause<T>(this Expression<Func<T, bool>> expression) where T : class
        {
            ExpressionEntry expressionEntry = new ExpressionEntry();
            expressionEntry.Visit(expression);
            return expressionEntry.GetResult();
        }
    }
}
