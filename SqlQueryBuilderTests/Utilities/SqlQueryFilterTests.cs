using SqlQueryBuilder.Utilities;
using SqlQueryBuilderTests.Helpers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace SqlQueryBuilderTests.Utilities
{
    [ExcludeFromCodeCoverage]
    public class SqlQueryFilterTests
    {
        private readonly string _fakeName = "fakeName";
        private readonly decimal _fakeDollars = 40.95M;
        private readonly int _fakeAge = 40;

        [Fact]
        public void SqlFilter_Where_Basic_Condition()
        {
            var (param, condition) = SqlQueryFilter.Where<TestSqlQuerySelectModel>(x => x.Name == _fakeName);

            Assert.Equal(TestSqlQueries.BasicFilter, condition);

            var keys = param.Select(x => x.Key).ToList();
            var values = param.Select(x => x.Value).ToList();

            Assert.Equal("@Name", keys[0]);
            Assert.Equal(_fakeName, values[0]);
        }

        [Fact]
        public void SqlFilter_Where_Basic_Or_Condition()
        {
            var (param, condition) = SqlQueryFilter.Where<TestSqlQuerySelectModel>(x => x.Name == _fakeName || x.Age == _fakeAge);

            Assert.Equal(TestSqlQueries.BasicOrFilter, condition);

            var keys = param.Select(x => x.Key).ToList();
            var values = param.Select(x => x.Value).ToList();

            Assert.Equal("@Name", keys[0]);
            Assert.Equal("@Age", keys[1]);
            Assert.Equal(_fakeName, values[0]);
            Assert.Equal(_fakeAge, values[1]);
        }

        [Fact]
        public void SqlFilter_Where_Basic_And_Condition()
        {
            var (param, condition) = SqlQueryFilter.Where<TestSqlQuerySelectModel>(x => x.Name == _fakeName && x.Age == _fakeAge);

            Assert.Equal(TestSqlQueries.BasicAndFilter, condition);

            var keys = param.Select(x => x.Key).ToList();
            var values = param.Select(x => x.Value).ToList();

            Assert.Equal("@Name", keys[0]);
            Assert.Equal("@Age", keys[1]);
            Assert.Equal(_fakeName, values[0]);
            Assert.Equal(_fakeAge, values[1]);
        }

        [Fact]
        public void SqlFilter_Where_Basic_And_Or_Condition()
        {
            var (param, condition) = SqlQueryFilter.Where<TestSqlQuerySelectModel>(x => (x.Name == _fakeName && x.Age == _fakeAge) || x.Dollars == _fakeDollars);

            Assert.Equal(TestSqlQueries.BasicAndOrFilter, condition);

            var keys = param.Select(x => x.Key).ToList();
            var values = param.Select(x => x.Value).ToList();


            Assert.Equal("@Age", keys[0]);
            Assert.Equal("@Name", keys[1]);
            Assert.Equal("@Dollars", keys[2]);

            Assert.Equal(_fakeAge, values[0]);
            Assert.Equal(_fakeName, values[1]);
            Assert.Equal(_fakeDollars, values[2]);
        }

        [Fact]
        public void SqlFilter_Where_Basic_Or_And_Condition()
        {
            var (param, condition) = SqlQueryFilter.Where<TestSqlQuerySelectModel>(x => (x.Name == _fakeName || x.Age == _fakeAge) && x.Dollars == _fakeDollars);

            Assert.Equal(TestSqlQueries.BasicOrAndFilter, condition);

            var keys = param.Select(x => x.Key).ToList();
            var values = param.Select(x => x.Value).ToList();


            Assert.Equal("@Age", keys[0]);
            Assert.Equal("@Name", keys[1]);
            Assert.Equal("@Dollars", keys[2]);

            Assert.Equal(_fakeAge, values[0]);
            Assert.Equal(_fakeName, values[1]);
            Assert.Equal(_fakeDollars, values[2]);
        }

        [Fact]
        public void SqlFilter_Where_Basic_And_Or_And_Condition()
        {
            var (param, condition) = SqlQueryFilter.Where<TestSqlQuerySelectModel>(x => (x.Name == _fakeName && x.Age == _fakeAge)
            || (x.Dollars == _fakeDollars && x.IsMonster == true));

            Assert.Equal(TestSqlQueries.BasicAndOrAndFilter, condition);

            var keys = param.Select(x => x.Key).ToList();
            var values = param.Select(x => x.Value).ToList();

            Assert.Equal("@Age", keys[0]);
            Assert.Equal("@Name", keys[1]);
            Assert.Equal("@IsMonster", keys[2]);
            Assert.Equal("@Dollars", keys[3]);

            Assert.Equal(_fakeAge, values[0]);
            Assert.Equal(_fakeName, values[1]);
            Assert.Equal(Convert.ToByte(true), values[2]);
            Assert.Equal(_fakeDollars, values[3]);
        }

        [Fact]
        public void SqlFilter_Where_Basic_Or_And_Or_Condition()
        {
            var (param, condition) = SqlQueryFilter.Where<TestSqlQuerySelectModel>(x => (x.Name == _fakeName || x.Age == _fakeAge)
            && (x.Dollars == _fakeDollars || x.IsMonster == true));

            Assert.Equal(TestSqlQueries.BasicOrAndOrFilter, condition);

            var keys = param.Select(x => x.Key).ToList();
            var values = param.Select(x => x.Value).ToList();

            Assert.Equal("@Age", keys[0]);
            Assert.Equal("@Name", keys[1]);
            Assert.Equal("@IsMonster", keys[2]);
            Assert.Equal("@Dollars", keys[3]);

            Assert.Equal(_fakeAge, values[0]);
            Assert.Equal(_fakeName, values[1]);
            Assert.Equal(Convert.ToByte(true), values[2]);
            Assert.Equal(_fakeDollars, values[3]);
        }
    }
}
