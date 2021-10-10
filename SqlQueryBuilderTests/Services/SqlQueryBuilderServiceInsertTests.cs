using Moq;
using SqlQueryBuilder.Services;
using SqlQueryBuilderTests.Helpers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace SqlQueryBuilderTests.Services
{
    [ExcludeFromCodeCoverage]
    public class SqlQueryBuilderServiceInsertTests
    {
        private readonly string _fakeName = "FakeName";
        private readonly string _fakeName2 = "FakeName2";
        private readonly TestSqlQuerySelectModel _model;
        private readonly TestSqlQuerySelectModel2 _model2;

        private readonly SqlQueryBuilderService _sqlQueryBuilder;

        public SqlQueryBuilderServiceInsertTests()
        {
            _model = new TestSqlQuerySelectModel
            {
                Name = _fakeName
            };

            _model2 = new TestSqlQuerySelectModel2
            {
                Name = _fakeName2
            };

            _sqlQueryBuilder = new SqlQueryBuilderService();
        }

        [Fact]
        public void SqlQueryBuilderService_Insert_Model_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _sqlQueryBuilder.Insert(It.IsAny<object>()));
        }

        [Fact]
        public void SqlQueryBuilderService_Insert_Returns_Correct_Query()
        {
            var (query, parameters) = _sqlQueryBuilder.Insert(_model);

            Assert.Equal(TestSqlQueries.ExpectedInsertQuery, query);
        }

        [Fact]
        public void SqlQueryBuilderService_Insert_Different_Models_Returns_Different_Queries()
        {
            var (query, parameters) = _sqlQueryBuilder.Insert(_model2);

            Assert.NotEqual(TestSqlQueries.ExpectedInsertQuery, query);
            Assert.Equal(TestSqlQueries.ExpectedNonConformQuery, query);
        }

        [Fact]
        public void SqlQueryBuilderService_Insert_Contains_Correct_Table_Name()
        {
            var (query, parameters) = _sqlQueryBuilder.Insert(_model);

            Assert.Contains("[Test].[SqlQuerySelect]", query);
        }

        [Fact]
        public void SqlQueryBuilderService_Insert_Contains_Correct_Table_Name_If_No_DisplayName_Attribute()
        {
            var (query, parameters) = _sqlQueryBuilder.Insert(new TestSqlQuerySelectModel2());

            Assert.Contains("[TestSqlQuerySelectModel2]", query);
        }

        [Fact]
        public void SqlQueryBuilderService_Insert_Returns_Correct_Parameters_With_Values()
        {
            var (query, parameters) = _sqlQueryBuilder.Insert(_model);

            var keys = parameters.Select(x => x.Key).ToList();
            var nameValue = parameters.FirstOrDefault(x => x.Key == "@Name").Value;

            Assert.Equal(_fakeName, nameValue);

            Assert.Equal("@Name", keys[0]);
        }
    }
}
