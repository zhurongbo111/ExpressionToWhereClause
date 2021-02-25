using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionToWhereClause
{
    public class WhereClauseAdhesive
    {
        public WhereClauseAdhesive(ISqlAdapter sqlAdapter, Dictionary<string, object> parameters)
        {
            Parameters = parameters;
            SqlAdapter = sqlAdapter;
        }

        public Dictionary<string, object> Parameters { get; }

        public ISqlAdapter SqlAdapter { get; }
    }
}
