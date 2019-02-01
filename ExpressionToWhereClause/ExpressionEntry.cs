using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionToWhereClause
{
    internal class ExpressionEntry :ExpressionVisitor
    {
        [ThreadStatic]
        internal static Dictionary<string, object> Parameters = new Dictionary<string, object>();

        internal static Func<System.Reflection.MemberInfo, string> FieldNameSelector = null;

        private string sql = string.Empty;

        public (string,Dictionary<string,object>) GetResult()
        {
            Dictionary<string, object> CloneParameters(Dictionary<string, object> parameters)
            {
                Dictionary<string, object> clonedParameters = new Dictionary<string, object>();
                foreach (var keyvaluePair in parameters)
                {
                    clonedParameters.Add(keyvaluePair.Key, keyvaluePair.Value);
                }
                return clonedParameters;
            }
            var clonePara = CloneParameters(Parameters);
            Parameters.Clear();
            return (sql, clonePara);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            return base.VisitMember(node); ;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            return base.VisitBinary(node);
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            return base.VisitBlock(node);
        }

        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            return base.VisitCatchBlock(node);
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            return base.VisitConditional(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            return base.VisitConstant(node);
        }

        protected override Expression VisitDefault(DefaultExpression node)
        {
            return base.VisitDefault(node);
        }

        protected override Expression VisitDynamic(DynamicExpression node)
        {
            return base.VisitDynamic(node);
        }

        protected override ElementInit VisitElementInit(ElementInit node)
        {
            return base.VisitElementInit(node);
        }

        protected override Expression VisitExtension(Expression node)
        {
            return base.VisitExtension(node);
        }

        protected override Expression VisitGoto(GotoExpression node)
        {
            return base.VisitGoto(node);
        }

        protected override Expression VisitIndex(IndexExpression node)
        {
            return base.VisitIndex(node);
        }

        protected override Expression VisitInvocation(InvocationExpression node)
        {
            return base.VisitInvocation(node);
        }

        protected override Expression VisitLabel(LabelExpression node)
        {
            return base.VisitLabel(node);
        }

        protected override LabelTarget VisitLabelTarget(LabelTarget node)
        {
            return base.VisitLabelTarget(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            if(node.Body is BinaryExpression)
            {
                BinaryExpressionVisitor binaryExpressionVisitor = new BinaryExpressionVisitor();
                binaryExpressionVisitor.Visit(node.Body);
                sql = binaryExpressionVisitor.GetResult();
            }
            else if(node.Body is MemberExpression)
            {
                BooleanMemberExpressionVisitor booleanMemberExpressionVisitor = new BooleanMemberExpressionVisitor();
                booleanMemberExpressionVisitor.Visit(node.Body);
                sql = booleanMemberExpressionVisitor.GetResult();
            }
            else if(node.Body is MethodCallExpression)
            {
                MethodCallExpressionVisitor methodCallExpressionVisitor = new MethodCallExpressionVisitor();
                methodCallExpressionVisitor.Visit(node.Body);
                sql = methodCallExpressionVisitor.GetResult();
            }
            else
            {
                throw new NotSupportedException($"Unknown expression {node.Body.GetType()}");
            }
            return node;
        }

        protected override Expression VisitListInit(ListInitExpression node)
        {
            return base.VisitListInit(node);
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            return base.VisitMemberAssignment(node);
        }

        protected override MemberBinding VisitMemberBinding(MemberBinding node)
        {
            return base.VisitMemberBinding(node);
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            return base.VisitMemberInit(node);
        }
        protected override Expression VisitLoop(LoopExpression node)
        {
            return base.VisitLoop(node);
        }

        protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
        {
            return base.VisitMemberListBinding(node);
        }

        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
        {
            return base.VisitMemberMemberBinding(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            return base.VisitMethodCall(node);
        }

        protected override Expression VisitNew(NewExpression node)
        {
            return base.VisitNew(node);
        }

        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            return base.VisitNewArray(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return base.VisitParameter(node);
        }

        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            return base.VisitRuntimeVariables(node);
        }

        protected override Expression VisitSwitch(SwitchExpression node)
        {
            return base.VisitSwitch(node);
        }

        protected override SwitchCase VisitSwitchCase(SwitchCase node)
        {
            return base.VisitSwitchCase(node);
        }

        protected override Expression VisitTry(TryExpression node)
        {
            return base.VisitTry(node);
        }

        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            return base.VisitTypeBinary(node);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            return base.VisitUnary(node);
        }

        protected override Expression VisitDebugInfo(DebugInfoExpression node)
        {
            return base.VisitDebugInfo(node);
        }

        internal static string EnsureKey(string key)
        {
            int seed = 1;
            while (Parameters.ContainsKey($"@{key}"))
            {
                key = key + seed;
                seed++;
            }
            return key;
        }

        internal static object GetConstantByExpression(Expression expression)
        {
            ConstantExpressionVisitor constantExpressionVisitor = null;
            if (expression is ConstantExpression)
            {
                constantExpressionVisitor = new ConstantExpressionVisitor();
            }
            else if (expression is MemberExpression)
            {
                constantExpressionVisitor = new MemberConstantExpressionVisitor();
            }
            else if (expression is MethodCallExpression)
            {
                constantExpressionVisitor = new MethodConstantExpressionVisitor();
            }
            else if (expression is ConditionalExpression)
            {
                constantExpressionVisitor = new ConditionalConstantExpressionVisitor();
            }
            else if( expression.GetType().Name == "MethodBinaryExpression")
            {
                constantExpressionVisitor = new MethodBinaryConstantExpressionVisitor();
            }
            else
            {
                throw new Exception($"Unknow expression {expression.GetType()}");
            }
            constantExpressionVisitor.Visit(expression);
            return constantExpressionVisitor.Value;
        }
    }
}
