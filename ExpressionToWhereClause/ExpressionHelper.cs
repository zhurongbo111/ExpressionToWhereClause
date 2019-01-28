using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionToWhereClause
{
    public class ExpressionHelper
    {
        public static (string, Dictionary<string, object>) ToWhereClause<T>(Expression<Func<T,bool>>  expression) where T : class
        {
            ExpressionEntry expressionEntry = new ExpressionEntry();
            expressionEntry.Visit(expression);
            return expressionEntry.GetResult();
        }

        public static void SetFieldNameSelector(Func<System.Reflection.MemberInfo, string> fieldNameSelector )
        {
            ExpressionEntry.FieldNameSelector = fieldNameSelector;
        }

    }
}
