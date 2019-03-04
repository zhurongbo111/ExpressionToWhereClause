using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ExpressionToWhereClause
{
    public class DefaultSqlAdapter : ISqlAdapter
    {

        public virtual string GetColumnName(MemberInfo mi)
        {
            return mi.Name;
        }

        public virtual string GetParameterName(MemberInfo mi)
        {
            return mi.Name;
        }
    }
}
