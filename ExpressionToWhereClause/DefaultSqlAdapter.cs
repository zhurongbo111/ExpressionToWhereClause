using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ExpressionToWhereClause
{
    public class DefaultSqlAdapter : ISqlAdapter
    {
        public string FormatColumnName(MemberInfo memberInfo)
        {
            return memberInfo.Name;
        }
    }
}
