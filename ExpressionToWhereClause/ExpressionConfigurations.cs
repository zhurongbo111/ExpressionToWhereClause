using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionToWhereClause
{
    public class ExpressionConfigurations
    {
        public static void SetFieldNameSelector(Func<System.Reflection.MemberInfo, string> fieldNameSelector )
        {
            ExpressionEntry.FieldNameSelector = fieldNameSelector;
        }

    }
}
