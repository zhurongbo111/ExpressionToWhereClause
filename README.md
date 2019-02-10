# ExpressionToWhereClause.Net 
A simple tool library for converting the Expression to sql where clause

[![Build Status](https://zhurongbo.visualstudio.com/Normal/_apis/build/status/ETWC%20Publish?branchName=master)](https://zhurongbo.visualstudio.com/Normal/_build/latest?definitionId=9&branchName=master)

Packages
--------

NuGet feed: https://www.nuget.org/packages/ExpressionToWhereClause/

| Package | NuGet Stable | NuGet Pre-release | Downloads |
| ------- | ------------ | ----------------- | --------- |
| [ExpressionToWhereClause](https://www.nuget.org/packages/ExpressionToWhereClause/) | [![ExpressionToWhereClause](https://img.shields.io/nuget/v/ExpressionToWhereClause.svg)](https://www.nuget.org/packages/ExpressionToWhereClause/) | [![ExpressionToWhereClause](https://img.shields.io/nuget/vpre/ExpressionToWhereClause.svg)](https://www.nuget.org/packages/ExpressionToWhereClause/) | [![ExpressionToWhereClause](https://img.shields.io/nuget/dt/ExpressionToWhereClause.svg)](https://www.nuget.org/packages/ExpressionToWhereClause/) |

Features
--------
ExpressionToWhereClause is a [NuGet library](https://www.nuget.org/packages/ExpressionToWhereClause) that you can add into your project that will extend your `Expression<Func<TModel, bool>>` type.

It provides only one Method:

**Explain of Type `Expression<Func<TModel, bool>>` to the `parametric` sql where clause and the parameter list**

```csharp
 public static (string, Dictionary<string, object>) ToWhereClause<T>(this Expression<Func<T, bool>> expression, bool? nonParametric = null) where T : class
```

The the right part of `Func<TModel, bool>` must like:

`[model].[PropertyName]`   `[comparator]`   `[Value]`, or the combinations.

**Example:**
```csharp
u.Name == "Foo"
```
Or
```csharp
u.Name == "Foo" || u.Name == "Bar"
```

The `[Value]` can be from many places, not only the constant. For the detailed information, please see the example usage.

![#f03c15](https://placehold.it/15/f03c15/000000?text=+)  Warning: This library dose not support unary, like `u => !(u.Name == "Foo")`, but support `u => u.Name != "Foo"` and `u => !u.Sex` Sex is bool type
--------




**Example usage:**

```csharp
internal class ExpressionEntry : ExpressionVisitor
{
    [ThreadStatic]
    private static Dictionary<string, object> _parameters = null;
    
    internal static Dictionary<string, object> Parameters
    {
        get
        {
            if(_parameters == null)
            {
                _parameters = new Dictionary<string, object>();
            }
            return _parameters;
        }
    }

    [ThreadStatic]
    internal static bool? NonParametric = null;

    internal static Func<System.Reflection.MemberInfo, string> FieldNameSelector = null;

    private string whereClause = string.Empty;

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
        return (whereClause, clonePara);
    }

    protected override Expression VisitLambda<T>(Expression<T> node)
    {
        whereClause = GetWhereClauseByExpression(node.Body).ToString();
        return node;
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

    internal static StringBuilder GetWhereClauseByExpression(Expression expression)
    {
        BaseExpressionVisitor expressionVisitor = null;
        if (expression is BinaryExpression)
        {
            expressionVisitor = new BinaryExpressionVisitor();
        }
        else if (expression is MemberExpression)
        {
            expressionVisitor = new BooleanMemberExpressionVisitor();
        }
        else if (expression is MethodCallExpression)
        {
            expressionVisitor = new MethodCallExpressionVisitor();
        }
        else if (expression is UnaryExpression unaryExpression
            && unaryExpression.Operand is MemberExpression
            && unaryExpression.Type == typeof(bool))
        {
            expressionVisitor = new UnaryBooleanMemberExpressionVisitor();
        }
        else
        {
            throw new NotSupportedException($"Unknow expression {expression.GetType()}");
        }

        expressionVisitor.Visit(expression);
        return expressionVisitor.GetResult();
    }
}

```
