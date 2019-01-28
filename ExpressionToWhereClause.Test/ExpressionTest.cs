using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ExpressionToWhereClause.Test
{
    [TestClass]
    public class ExpressionTest
    {
        [TestMethod]
        public void ValidateBool()
        {
            (string sql, Dictionary<string, object> parameters) = ExpressionHelper.ToWhereClause<User>(
                u => u.Sex && u.Age > 20);
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex", true);
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("(Sex = @Sex) AND (Age > @Age)", sql);
            AssertParameters(expectedParameters, parameters);

        }

        [TestMethod]
        public void ValidateBool2()
        {
            (string sql, Dictionary<string, object> parameters) = ExpressionHelper.ToWhereClause<User>(
                u => u.Sex == true);
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex", true);
            Assert.AreEqual("Sex = @Sex", sql);
            AssertParameters(expectedParameters, parameters);

        }

        [TestMethod]
        public void ValidateConstant()
        {
            (string sql, Dictionary<string, object> parameters) = ExpressionHelper.ToWhereClause<User>(
                u => u.Age >= 20);
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Age >= @Age", sql);
            AssertParameters(expectedParameters, parameters);
            
        }

        [TestMethod]
        public void ValidateMemberConstant()
        {
            UserFilter userFilter = new UserFilter();
            userFilter.Internal.Age = 20;
            (string sql, Dictionary<string, object> parameters) = ExpressionHelper.ToWhereClause<User>(
                u => u.Age < userFilter.Internal.Age);
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Age < @Age", sql);
            AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateMethodConstant()
        {
            (string sql, Dictionary<string, object> parameters) = ExpressionHelper.ToWhereClause<User>(
                u => u.Age < int.Parse(GetInt().ToString()));
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Age < @Age", sql);
            AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateMethodConstant2()
        {
            UserFilter userFilter = new UserFilter();
            userFilter.Internal.Age = 20;
            (string sql, Dictionary<string, object> parameters) = ExpressionHelper.ToWhereClause<User>(
                u => u.Age < GetInt(userFilter.Internal.Age));
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Age < @Age", sql);
            AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateComplexSql()
        {
            UserFilter filter = new UserFilter();
            filter.Internal.Age = 20;
            (string sql, Dictionary<string, object> parameters) = ExpressionHelper.ToWhereClause<User>(
                u => ((u.Sex && u.Age > 18) || (u.Sex==false && u.Age > filter.Internal.Age)) 
                && (u.Name == GetInt().ToString() || u.Name == int.Parse(UserFilter.GetInt(20).ToString()).ToString())
                );
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex", true);
            expectedParameters.Add("@Age", 18);
            expectedParameters.Add("@Sex1", false);
            expectedParameters.Add("@Age1", 20);
            expectedParameters.Add("@Name", "20");
            expectedParameters.Add("@Name1", "20");
            Assert.AreEqual("(((Sex = @Sex) AND (Age > @Age)) OR ((Sex = @Sex1) AND (Age > @Age1))) AND ((Name = @Name) OR (Name = @Name1))", sql);
            AssertParameters(expectedParameters, parameters);

        }

        [TestMethod]
        public void ValidateEqual()
        {
            UserFilter filter = new UserFilter();
            filter.Name = "Name";
            (string sql, Dictionary<string, object> parameters) = ExpressionHelper.ToWhereClause<User>(
                u => u.Name.Equals("Name".Substring(1))
                );
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Name", "ame");
            Assert.AreEqual("Name = @Name", sql);
            AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateStartWith()
        {
            UserFilter filter = new UserFilter();
            filter.Name = "Name";
            (string sql, Dictionary<string, object> parameters) = ExpressionHelper.ToWhereClause<User>(
                u => u.Name.StartsWith("Name")
                );
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Name", "Name");
            Assert.AreEqual("Name like @Name'%'", sql);
            AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateStartWith2()
        {
            UserFilter filter = new UserFilter();
            filter.Name = "Name";
            (string sql, Dictionary<string, object> parameters) = ExpressionHelper.ToWhereClause<User>(
                u => u.Name.StartsWith("Name") || u.Name.Contains("Start")
                );
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Name", "Name");
            expectedParameters.Add("@Name1", "Start");
            Assert.AreEqual("(Name like @Name'%') OR (Name like '%'@Name1'%')", sql);
            AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateEndsWith()
        {
            UserFilter filter = new UserFilter();
            filter.Name = "Name";
            (string sql, Dictionary<string, object> parameters) = ExpressionHelper.ToWhereClause<User>(
                u => u.Name.EndsWith("Name")
                );
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Name", "Name");
            Assert.AreEqual("Name like '%'@Name", sql);
            AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateContains()
        {
            UserFilter filter = new UserFilter();
            filter.Name = "Name";
            (string sql, Dictionary<string, object> parameters) = ExpressionHelper.ToWhereClause<User>(
                u => u.Name.Contains(filter.Name)
                );
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Name", "Name");
            Assert.AreEqual("Name like '%'@Name'%'", sql);
            AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateUnary()
        {
            try
            {
                ExpressionHelper.ToWhereClause<User>(u => !u.Name.Contains("Name"));
            }
            catch(Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(NotSupportedException));
            }
            
            
        }

        private int GetInt()
        {
            return 20;
        }



        private int GetInt(int i)
        {
            return i;
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

    }

    public class UserFilter
    {
        public string Name { get; set; }

        public bool Sex { get; set; }

        public int Age { get; set; }

        public Internal Internal { get; set; } = new Internal();

        public static int GetInt(int i)
        {
            return i;
        }
    }

    public class Internal
    {
        public int Age { get; set; }
    }
    public class User
    {
        public string Name { get;set; }

        public bool Sex { get; set; }

        public int Age { get; set; }
    }
}
