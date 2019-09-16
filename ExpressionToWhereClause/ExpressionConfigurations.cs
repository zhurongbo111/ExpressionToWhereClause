using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionToWhereClause
{
    public class ExpressionConfigurations
    {
        static internal bool? NonParametric = null;

        static internal ISqlAdapter Adapter = new DefaultSqlAdapter();

        public static void SetSqlAdapter(ISqlAdapter sqlAdapter)
        {
            Adapter = sqlAdapter ?? new DefaultSqlAdapter();
        }

        /// <summary>
        /// The the global NonParametric.
        /// If Set it true, the generated string doesn't contain placeholder
        /// If Set it false, the generated string contains placeholder
        /// By default. it is false;
        /// </summary>
        /// <param name="nonParametric">The boolean value</param>
        public static void SetNonParametricSelector(bool nonParametric)
        {
            NonParametric = nonParametric;
        }

    }
}
