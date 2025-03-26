using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionToWhereClause.Test
{

    public class TestSqlAdapter : DefaultSqlAdapter
    {

    }

    [TestClass]
    public class ExpressionTest
    {
        [TestMethod]
        public void ValidateBool()
        {
            Expression<Func<User, bool>> expression = u => !u.Sex;

            (string sql, Dictionary<string, object> parameters) = expression.ToWhereClause(new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex", false);
            Assert.AreEqual("Sex = @Sex", sql);
            AssertParameters(expectedParameters, parameters);
        }
        [TestMethod]
        public void ValidateBool2()
        {
            Expression<Func<User, bool>> expression = u => u.Sex && u.Name.StartsWith("Foo");
            (string sql, Dictionary<string, object> parameters) = expression.ToWhereClause(new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex", true);
            expectedParameters.Add("@Name", "Foo%");
            Assert.AreEqual("(Sex = @Sex) AND (Name like @Name)", sql);
            AssertParameters(expectedParameters, parameters);

        }

        [TestMethod]
        public void ValidateConstant()
        {
            Expression<Func<User, bool>> expression = u => u.Age >= 20;
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Age >= @Age", whereClause);
            AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateVariable()
        {
            int age = 20;
            Expression<Func<User, bool>> expression = u => u.Age >= age;
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Age >= @Age", whereClause);
            AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateAnd()
        {
            Expression<Func<User, bool>> expression = u => u.Sex && u.Age > 20;

            (string sql, Dictionary<string, object> parameters) = expression.ToWhereClause(new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex", true);
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("(Sex = @Sex) AND (Age > @Age)", sql);
            AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateOr()
        {
            Expression<Func<User, bool>> expression = u => u.Sex || u.Age > 20;

            (string sql, Dictionary<string, object> parameters) = expression.ToWhereClause(new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex", true);
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("(Sex = @Sex) OR (Age > @Age)", sql);
            AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateDuplicateField()
        {
            Expression<Func<User, bool>> expression = u => u.Age < 15 || u.Age > 20;

            (string sql, Dictionary<string, object> parameters) = expression.ToWhereClause(new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 15);
            expectedParameters.Add("@Age1", 20);
            Assert.AreEqual("(Age < @Age) OR (Age > @Age1)", sql);
            AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateMemberConstant()
        {
            UserFilter userFilter = new UserFilter();
            userFilter.Age = 20;
            Expression<Func<User, bool>> expression = u => u.Age < userFilter.Age;
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Age < @Age", whereClause);
            AssertParameters(expectedParameters, parameters);
        }



        [TestMethod]
        public void ValidateDeepMemberConstant()
        {
            UserFilter userFilter = new UserFilter();
            userFilter.Internal.Age = 20;
            Expression<Func<User, bool>> expression = u => u.Age < userFilter.Internal.Age;
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Age < @Age", whereClause);
            AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateInstanceMethodConstant()
        {
            Expression<Func<User, bool>> expression = u => u.Age < GetInt();
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Age < @Age", whereClause);
            AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateStaticMethodConstant()
        {
            Expression<Func<User, bool>> expression = u => u.Age < UserFilter.GetInt(20);
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Age < @Age", whereClause);
            AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateMethodChainConstant()
        {
            Expression<Func<User, bool>> expression = u => u.Age < int.Parse(GetInt().ToString());
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Age < @Age", whereClause);
            AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateMethodConstant2()
        {
            UserFilter userFilter = new UserFilter();
            userFilter.Internal.Age = 20;
            Expression<Func<User, bool>> expression = u => u.Age < GetInt(userFilter.Internal.Age);
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Age < @Age", whereClause);
            AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateEqualMethod()
        {
            UserFilter filter = new UserFilter();
            filter.Name = "Name";
            Expression<Func<User, bool>> expression = u => u.Name.Equals(filter.Name.Substring(1));
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Name", "ame");
            Assert.AreEqual("Name = @Name", whereClause);
            AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateTernary()
        {
            string name = "Gary";
            Expression<Func<User, bool>> expression = u => u.Name == (name == null ? "Foo" : name);
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Name", "Gary");
            Assert.AreEqual("Name = @Name", whereClause);
            AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateNotEqual()
        {
            Expression<Func<User, bool>> expression = u => u.Age != 20;
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);

            Assert.AreEqual("Age <> @Age", whereClause);
            AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateStartsWithMethod()
        {
            UserFilter filter = new UserFilter();
            filter.Name = "Name";
            Expression<Func<User, bool>> expression = u => u.Name.StartsWith(filter.Name);
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Name", "Name%");
            Assert.AreEqual("Name like @Name", whereClause);
            AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateStartWith2()
        {
            UserFilter filter = new UserFilter();
            filter.Name = "Name";
            Expression<Func<User, bool>> expression = u => u.Name.StartsWith(filter.Name) || u.Name.Contains("Start");
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Name", "Name%");
            expectedParameters.Add("@Name1", "%Start%");
            Assert.AreEqual("(Name like @Name) OR (Name like @Name1)", whereClause);
            AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateEndsWithMethod()
        {
            UserFilter filter = new UserFilter();
            filter.Name = "Name";
            Expression<Func<User, bool>> expression = u => u.Name.EndsWith(filter.Name);
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Name", "%Name");
            Assert.AreEqual("Name like @Name", whereClause);
            AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateContainsMethod()
        {
            UserFilter filter = new UserFilter();
            filter.Name = "Name";
            Expression<Func<User, bool>> expression = u => u.Name.Contains(filter.Name);
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Name", "%Name%");
            Assert.AreEqual("Name like @Name", whereClause);
            AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateUnary()
        {
            try
            {
                Expression<Func<User, bool>> expression = u => !u.Name.Contains("Name");
                expression.ToWhereClause(new TestSqlAdapter());
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(NotSupportedException));
            }
        }

        [TestMethod]
        public void ValidateAll()
        {
            UserFilter filter = new UserFilter();
            filter.Internal.Age = 20;
            filter.Name = "Gary";
            Expression<Func<User, bool>> expression =
                u => ((u.Sex && u.Age > 18) || (u.Sex == false && u.Age > filter.Internal.Age))
                  && (u.Name == filter.Name || u.Name.Contains(filter.Name.Substring(1, 2)));
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex", true);
            expectedParameters.Add("@Age", 18);
            expectedParameters.Add("@Sex1", false);
            expectedParameters.Add("@Age1", 20);
            expectedParameters.Add("@Name", "Gary");
            expectedParameters.Add("@Name1", "%ar%");
            Assert.AreEqual("(((Sex = @Sex) AND (Age > @Age)) OR ((Sex = @Sex1) AND (Age > @Age1))) AND ((Name = @Name) OR (Name like @Name1))", whereClause);
            AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateArrayIn()
        {
            List<string> values = new List<string> { "a", "b" };
            Expression<Func<User, bool>> expression = u => values.Contains(u.Name);
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            Assert.AreEqual("Name in @Name", whereClause);
            Assert.IsTrue(parameters["@Name"] is List<string>);
            Assert.AreEqual(string.Join(',', values), string.Join(',', (List<string>)parameters["@Name"]));
        }

        [TestMethod]
        public void ValidateEnumerableIn()
        {
            string[] values = new string[] { "a", "b" };
            Expression<Func<User, bool>> expression = u => values.Contains(u.Name);
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            Assert.AreEqual("Name in @Name", whereClause);
            Assert.IsTrue(parameters["@Name"] is string[]);
            Assert.AreEqual(string.Join(',', values), string.Join(',', (string[])parameters["@Name"]));
        }

        [TestMethod]
        public void ValidateNotArrayIn()
        {
            List<string> values = new List<string> { "a", "b" };
            Expression<Func<User, bool>> expression = u => !values.Contains(u.Name);
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            Assert.AreEqual("Name not in @Name", whereClause);
            Assert.IsTrue(parameters["@Name"] is List<string>);
            Assert.AreEqual(string.Join(',', values), string.Join(',', (List<string>)parameters["@Name"]));
        }

        [TestMethod]
        public void ValidateEnumerableNotIn()
        {
            string[] values = new string[] { "a", "b" };
            Expression<Func<User, bool>> expression = u => !values.Contains(u.Name);
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            Assert.AreEqual("Name not in @Name", whereClause);
            Assert.IsTrue(parameters["@Name"] is string[]);
            Assert.AreEqual(string.Join(',', values), string.Join(',', (string[])parameters["@Name"]));
        }

        [TestMethod]
        public void ValidateEnum()
        {
            Expression<Func<User, bool>> expression = u => u.Sex2 == Sex.Female;
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(new TestSqlAdapter());
            Assert.AreEqual("Sex2 = @Sex2", whereClause);
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex2", (int)Sex.Female);
            AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateEnum2()
        {
            Sex[] sexes = new Sex[] { Sex.Female, Sex.Female };
            Expression<Func<User, bool>> expression = u => u.Sex2 == sexes[1];
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(new TestSqlAdapter());
            Assert.AreEqual("Sex2 = @Sex2", whereClause);
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex2", (int)Sex.Female);
            AssertParameters(expectedParameters, parameters);
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
            foreach (string key in expectedParameters.Keys)
            {
                Assert.IsTrue(actualParameters.ContainsKey(key), $"The parameters does not contain key '{key}'");
                Assert.IsTrue(expectedParameters[key].Equals(actualParameters[key]), $"The expected value is {expectedParameters[key]}, the actual value is {actualParameters[key]}");
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
        [Column("Username")]
        public string Name { get; set; }

        public bool Sex { get; set; }

        public Sex Sex2 { get; set; }

        public int Age { get; set; }
    }

    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute(string name)
        {
            this.Name = name;
        }


        public string Name { get; set; }
    }

    public enum Sex
    {
        Male = 1,
        Female = 2
    }
}
