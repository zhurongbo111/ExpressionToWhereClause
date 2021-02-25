using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ExpressionToWhereClause
{
    public static class WhereClauseExpressionExtensions
    {
        public static (string, Dictionary<string, object>) ToWhereClause<T>(this Expression<Func<T, bool>> expression, ISqlAdapter sqlAdapter = default) where T : class
        {
            WhereClauseAdhesive adhesive = new WhereClauseAdhesive(sqlAdapter ?? new DefaultSqlAdapter(), new Dictionary<string, object>());
            var sql = WhereCaluseParser.Parse(expression.Body, adhesive);

            return (sql.ToString(), adhesive.Parameters);
        }
    }
}
