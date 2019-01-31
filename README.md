# ExpressionToWhereClause.Net 
a simple tool library for converting the Expression to sql where clause

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

**Explain of Type `Expression<Func<TModel, bool>>` to the Parametric sql where clause and the parameters list**

```csharp
 public static (string, Dictionary<string, object>) ToWhereClause<T>(this Expression<Func<T, bool>> expression) where T : class
```
Example usage:

```csharp
public class User
{
    public string Name { get;set; }

    public bool Sex { get; set; }

    public int Age { get; set; }
}
private void AssertParameters(Dictionary<string, object> expectedParameters, Dictionary<string, object> actualParameters)
{
    Assert.AreEqual(expectedParameters.Count, actualParameters.Count);
    foreach(string key in expectedParameters.Keys)
    {
        Assert.IsTrue(actualParameters.ContainsKey(key),$"The parameters does not contain key '{key}'");
        Assert.IsTrue(expectedParameters[key].Equals(actualParameters[key]),$"The expected value is {expectedParameters[key]}, the actual value is {actualParameters[key]}");
    }
}

Expression<Func<User, bool>> expression = u => u.Sex && u.Age > 20;

(string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause();
Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
expectedParameters.Add("@Sex", true);
expectedParameters.Add("@Age", 20);
Assert.AreEqual("(Sex = @Sex) AND (Age > @Age)", whereClause);
AssertParameters(expectedParameters, parameters);

```
