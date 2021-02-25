using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionToWhereClause
{
    public class WhereClauseAdhesive
    {
        public WhereClauseAdhesive(ISqlAdapter sqlAdapter, Dictionary<string, object> parameters = null)
        {
            Parameters = parameters ?? new Dictionary<string, object>();
            SqlAdapter = sqlAdapter;
        }

        public Dictionary<string, object> Parameters { get; }

        public ISqlAdapter SqlAdapter { get; }
    }
}
