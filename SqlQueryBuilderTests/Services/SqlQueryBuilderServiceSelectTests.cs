using Moq;
using SqlQueryBuilder.Services;
using SqlQueryBuilderTests.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Xunit;

namespace SqlQueryBuilderTests.Services
{
    [ExcludeFromCodeCoverage]
    public class SqlQueryBuilderServiceSelectTests
    {
        private readonly string _fakeAccountName = "FakeAccountName";
        private readonly SqlQueryBuilderService _sqlQueryBuilder;
        private readonly byte _isCurrentVersion = Convert.ToByte(true);
        private readonly DateTime _runDate = DateTime.UtcNow;
        private readonly TestSqlQuerySelectModel _testModel;
        private readonly string _fakeName = "FakeName";
        private readonly int _fakeAge = 500;
        private readonly DateTime _fakeBday = DateTime.UtcNow;
        private readonly bool _fakeIsMonster = true;
        private readonly decimal _fakeDollars = 500.95M;
        private readonly string _tableName = "Some.Table";
        private readonly string _neededColumns = "[Col1],[Col2],[Col3]";

        public SqlQueryBuilderServiceSelectTests()
        {
            _testModel = new TestSqlQuerySelectModel
            {
                Name = _fakeName,
                Age = _fakeAge,
                Bday = _fakeBday,
                IsMonster = _fakeIsMonster,
                Dollars = _fakeDollars
            };

            _sqlQueryBuilder = new SqlQueryBuilderService();
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_Filter_Or_Same_Key_Params()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.Name == _testModel.Name || x.Name == _fakeAccountName);

            Assert.Equal(2, parameters.Count);

            var keys = parameters.Select(x => x.Key).ToList();
            var values = parameters.Select(x => x.Value).ToList();

            Assert.Equal("@Name", keys[0]);
            Assert.Contains("@Name", keys[1]);

            Assert.Equal(_testModel.Name, values[0]);
            Assert.Equal(_fakeAccountName, values[1]);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_Filter_With_Or_Same_Value_Added_Once_As_Param()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => (x.Name == _testModel.Name && x.Age == _testModel.Age)
            || (x.Name == _testModel.Name && x.IsMonster == _testModel.IsMonster));

            Assert.Equal(TestSqlQueries.ExpectedQuerySameFilter, query);
            Assert.Equal(3, parameters.Count);
        }


        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_Filter_And_Or_And_Return_Correct_Query()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => (x.Name == _testModel.Name && x.Age == _testModel.Age)
            || (x.Dollars == _testModel.Dollars && x.IsMonster == _testModel.IsMonster));

            Assert.Equal(TestSqlQueries.ExpectedQueryAndOrAndFilter, query);
            Assert.Equal(4, parameters.Count);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_Filter_Or_And_Or_Return_Correct_Query()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => (x.Name == _testModel.Name || x.Age == _testModel.Age)
            && (x.Dollars == _testModel.Dollars || x.IsMonster == _testModel.IsMonster));

            Assert.Equal(TestSqlQueries.ExpectedQueryOrAndOrFilter, query);
            Assert.Equal(4, parameters.Count);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_Filter_Or_And_Return_Correct_Query()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => (x.Name == _testModel.Name || x.Age == _testModel.Age)
            && x.IsMonster == _testModel.IsMonster);

            Assert.Equal(TestSqlQueries.ExpectedQueryOrAndFilter, query);
            Assert.Equal(3, parameters.Count);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_Filter_And_Or_Return_Correct_Query()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => (x.Name == _testModel.Name && x.Age == _testModel.Age)
            || x.IsMonster == _testModel.IsMonster);

            Assert.Equal(TestSqlQueries.ExpectedQueryAndOrFilter, query);
            Assert.Equal(3, parameters.Count);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_With_No_Filter_Return_Correct_Query()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(null);

            Assert.Equal(TestSqlQueries.ExpectedQueryNoFilter, query);
            Assert.Null(parameters);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_With_Filter_Decimal_From_Model_Return_Correct_Query()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.Dollars == _testModel.Dollars);

            Assert.Equal(TestSqlQueries.ExpectedQueryDecimalFilter, query);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_With_Filter_Decimal_From_Model_Return_Correct_Parameter()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.Dollars == _testModel.Dollars);

            var keys = parameters.Select(x => x.Key).ToList();
            var result = parameters.FirstOrDefault(x => x.Key == "@Dollars").Value;

            Assert.Equal(_fakeDollars, result);

            Assert.Equal("@Dollars", keys[0]);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_With_Filter_Decimal_From_Variable_Return_Correct_Query()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.Dollars == _fakeDollars);

            Assert.Equal(TestSqlQueries.ExpectedQueryDecimalFilter, query);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_With_Filter_Decimal_From_Variable_Return_Correct_Parameter()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.Dollars == _fakeDollars);

            var keys = parameters.Select(x => x.Key).ToList();
            var result = parameters.FirstOrDefault(x => x.Key == "@Dollars").Value;

            Assert.Equal(_fakeDollars, result);

            Assert.Equal("@Dollars", keys[0]);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_With_Filter_Decimal_From_Constant_Return_Correct_Query()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.Dollars == 500.95M);

            Assert.Equal(TestSqlQueries.ExpectedQueryDecimalFilter, query);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_With_Filter_Decimal_From_Constant_Return_Correct_Parameter()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.Dollars == 500.95M);

            var keys = parameters.Select(x => x.Key).ToList();
            var result = parameters.FirstOrDefault(x => x.Key == "@Dollars").Value;

            Assert.Equal(_fakeDollars, result);

            Assert.Equal("@Dollars", keys[0]);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_With_Filter_Bool_From_Model_Return_Correct_Query()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.IsMonster == _testModel.IsMonster);

            Assert.Equal(TestSqlQueries.ExpectedQueryBoolFilter, query);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_With_Filter_Bool_From_Model_Return_Correct_Parameter()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.IsMonster == _testModel.IsMonster);

            var keys = parameters.Select(x => x.Key).ToList();
            var result = parameters.FirstOrDefault(x => x.Key == "@IsMonster").Value;

            Assert.Equal(Convert.ToByte(_fakeIsMonster), result);

            Assert.Equal("@IsMonster", keys[0]);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_With_Filter_Bool_From_Variable_Return_Correct_Query()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.IsMonster == _fakeIsMonster);

            Assert.Equal(TestSqlQueries.ExpectedQueryBoolFilter, query);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_With_Filter_Bool_From_Variable_Return_Correct_Parameter()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.IsMonster == _fakeIsMonster);

            var keys = parameters.Select(x => x.Key).ToList();
            var result = parameters.FirstOrDefault(x => x.Key == "@IsMonster").Value;

            Assert.Equal(Convert.ToByte(_fakeIsMonster), result);

            Assert.Equal("@IsMonster", keys[0]);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_With_Filter_Bool_From_Constant_Return_Correct_Query()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.IsMonster == true);

            Assert.Equal(TestSqlQueries.ExpectedQueryBoolFilter, query);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_With_Filter_Bool_From_Constant_Return_Correct_Parameter()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.IsMonster == true);

            var keys = parameters.Select(x => x.Key).ToList();
            var result = parameters.FirstOrDefault(x => x.Key == "@IsMonster").Value;

            Assert.Equal(Convert.ToByte(_fakeIsMonster), result);

            Assert.Equal("@IsMonster", keys[0]);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_With_Filter_Date_From_Model_Return_Correct_Query()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.Bday == _testModel.Bday);

            Assert.Equal(TestSqlQueries.ExpectedQueryDateFilter, query);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_With_Filter_Date_From_Model_Return_Correct_Parameter()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.Bday == _testModel.Bday);

            var keys = parameters.Select(x => x.Key).ToList();
            var result = parameters.FirstOrDefault(x => x.Key == "@Bday").Value;

            Assert.Equal(_fakeBday, result);

            Assert.Equal("@Bday", keys[0]);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_With_Filter_Date_From_Variable_Return_Correct_Query()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.Bday == _fakeBday);

            Assert.Equal(TestSqlQueries.ExpectedQueryDateFilter, query);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_With_Filter_Date_From_Variable_Return_Correct_Parameter()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.Bday == _fakeBday);

            var keys = parameters.Select(x => x.Key).ToList();
            var result = parameters.FirstOrDefault(x => x.Key == "@Bday").Value;

            Assert.Equal(_fakeBday, result);

            Assert.Equal("@Bday", keys[0]);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_With_Filter_Int_From_Model_Return_Correct_Query()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.Age == _testModel.Age);

            Assert.Equal(TestSqlQueries.ExpectedQueryIntFilter, query);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_With_Filter_Int_From_Model_Return_Correct_Parameter()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.Age == _testModel.Age);

            var keys = parameters.Select(x => x.Key).ToList();
            var result = parameters.FirstOrDefault(x => x.Key == "@Age").Value;

            Assert.Equal(_fakeAge, result);

            Assert.Equal("@Age", keys[0]);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_With_Filter_Int_From_Variable_Return_Correct_Query()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.Age == _fakeAge);

            Assert.Equal(TestSqlQueries.ExpectedQueryIntFilter, query);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_With_Filter_Int_From_Variable_Return_Correct_Parameter()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.Age == _fakeAge);

            var keys = parameters.Select(x => x.Key).ToList();
            var result = parameters.FirstOrDefault(x => x.Key == "@Age").Value;

            Assert.Equal(_fakeAge, result);

            Assert.Equal("@Age", keys[0]);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_With_Filter_Int_From_Constant_Return_Correct_Query()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.Age == 500);

            Assert.Equal(TestSqlQueries.ExpectedQueryIntFilter, query);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_With_Filter_Int_From_Constant_Return_Correct_Parameter()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.Age == 500);

            var keys = parameters.Select(x => x.Key).ToList();
            var result = parameters.FirstOrDefault(x => x.Key == "@Age").Value;

            Assert.Equal(_fakeAge, result);

            Assert.Equal("@Age", keys[0]);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_With_Filter_String_From_Model_Return_Correct_Query()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.Name == _testModel.Name);

            Assert.Equal(TestSqlQueries.ExpectedQueryStringFilter, query);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_With_Filter_String_From_Model_Return_Correct_Parameter()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.Name == _testModel.Name);

            var keys = parameters.Select(x => x.Key).ToList();
            var result = parameters.FirstOrDefault(x => x.Key == "@Name").Value;

            Assert.Equal(_fakeName, result);

            Assert.Equal("@Name", keys[0]);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_With_Filter_String_From_Variable_Return_Correct_Query()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.Name == _fakeName);

            Assert.Equal(TestSqlQueries.ExpectedQueryStringFilter, query);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_With_Filter_String_From_Variable_Return_Correct_Parameter()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.Name == _fakeName);

            var keys = parameters.Select(x => x.Key).ToList();
            var result = parameters.FirstOrDefault(x => x.Key == "@Name").Value;

            Assert.Equal(_fakeName, result);

            Assert.Equal("@Name", keys[0]);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_With_Filter_String_From_Constant_Return_Correct_Query()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.Name == "FakeName");

            Assert.Equal(TestSqlQueries.ExpectedQueryStringFilter, query);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_TestSqlQuerySelectModel_With_Filter_String_From_Constant_Return_Correct_Parameter()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.Name == "FakeName");

            var keys = parameters.Select(x => x.Key).ToList();
            var result = parameters.FirstOrDefault(x => x.Key == "@Name").Value;

            Assert.Equal(_fakeName, result);

            Assert.Equal("@Name", keys[0]);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_Dynamic_Without_Filter_Return_Correct_Query()
        {
            var (query, parameters) = _sqlQueryBuilder.Select(_tableName, _neededColumns, It.IsAny<Expression<Func<TestSqlQuerySelectModel, bool>>>());
            Assert.Equal(TestSqlQueries.ExpectedDynamicSelectQueryWithoutFilter, query);
        }

        [Fact]
        public void SqlQueryBuilderService_Select_Dynamic_With_Filter_Return_Correct_Query()
        {
            var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(_tableName, _neededColumns, x => x.Name == _fakeName);
            Assert.Equal(TestSqlQueries.ExpectedDynamicSelectQuery, query);
        }
    }
}
