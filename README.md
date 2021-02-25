# ExpressionToWhereClause.Net 
A simple tool library for converting the Expression to sql where clause

[![nuget publish](https://github.com/zhurongbo111/ExpressionToWhereClause/actions/workflows/dotnet-core.yml/badge.svg)](https://github.com/zhurongbo111/ExpressionToWhereClause/actions/workflows/dotnet-core.yml)

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

See the [Unit Test](https://github.com/zhurongbo111/ExpressionToWhereClause/blob/master/ExpressionToWhereClause.Test/ExpressionTest.cs) 

