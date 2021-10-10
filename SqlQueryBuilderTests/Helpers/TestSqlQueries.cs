namespace SqlQueryBuilderTests.Helpers
{
    public static class TestSqlQueries
    {
        #region TestQueries
        public const string ExpectedInsertQuery = "INSERT INTO [Test].[SqlQuerySelect]([Name],[Age],[Bday],[IsMonster],[Dollars]) VALUES(@Name,@Age,@Bday,@IsMonster,@Dollars)";
        public const string ExpectedNonConformQuery = "INSERT INTO [TestSqlQuerySelectModel2]([Name]) VALUES(@Name)";
        public const string ExpectedQueryNoFilter = "SELECT [Name],[Age],[Bday],[IsMonster],[Dollars] FROM [Test].[SqlQuerySelect]";
        public const string ExpectedQueryStringFilter = "SELECT [Name],[Age],[Bday],[IsMonster],[Dollars] FROM [Test].[SqlQuerySelect] WHERE  Name = @Name";
        public const string ExpectedQueryIntFilter = "SELECT [Name],[Age],[Bday],[IsMonster],[Dollars] FROM [Test].[SqlQuerySelect] WHERE  Age = @Age";
        public const string ExpectedQueryDateFilter = "SELECT [Name],[Age],[Bday],[IsMonster],[Dollars] FROM [Test].[SqlQuerySelect] WHERE  Bday = @Bday";
        public const string ExpectedQueryBoolFilter = "SELECT [Name],[Age],[Bday],[IsMonster],[Dollars] FROM [Test].[SqlQuerySelect] WHERE  IsMonster = @IsMonster";
        public const string ExpectedQueryDecimalFilter = "SELECT [Name],[Age],[Bday],[IsMonster],[Dollars] FROM [Test].[SqlQuerySelect] WHERE  Dollars = @Dollars";
        public const string ExpectedQueryAndOrFilter = "SELECT [Name],[Age],[Bday],[IsMonster],[Dollars] FROM [Test].[SqlQuerySelect] WHERE ( Age = @Age  AND  Name = @Name ) OR ( IsMonster = @IsMonster )";
        public const string ExpectedQueryOrAndFilter = "SELECT [Name],[Age],[Bday],[IsMonster],[Dollars] FROM [Test].[SqlQuerySelect] WHERE ( Age = @Age  OR  Name = @Name ) AND ( IsMonster = @IsMonster )";
        public const string ExpectedQueryOrAndOrFilter = "SELECT [Name],[Age],[Bday],[IsMonster],[Dollars] FROM [Test].[SqlQuerySelect] WHERE ( Age = @Age  OR  Name = @Name ) AND ( IsMonster = @IsMonster  OR  Dollars = @Dollars )";
        public const string ExpectedQueryAndOrAndFilter = "SELECT [Name],[Age],[Bday],[IsMonster],[Dollars] FROM [Test].[SqlQuerySelect] WHERE ( Age = @Age  AND  Name = @Name ) OR ( IsMonster = @IsMonster  AND  Dollars = @Dollars )";
        public const string ExpectedQuerySameFilter = "SELECT [Name],[Age],[Bday],[IsMonster],[Dollars] FROM [Test].[SqlQuerySelect] WHERE ( Age = @Age  AND  Name = @Name ) OR ( IsMonster = @IsMonster  AND  Name = @Name )";
        public const string ExpectedDynamicSelectQueryWithoutFilter = "SELECT [Col1],[Col2],[Col3] FROM [Some].[Table]";
        public const string ExpectedDynamicSelectQuery = "SELECT [Col1],[Col2],[Col3] FROM [Some].[Table] WHERE  Name = @Name";
        #endregion

        #region WhereConditions
        public const string BasicFilter = "WHERE  Name = @Name";
        public const string BasicOrFilter = "WHERE ( Name = @Name ) OR ( Age = @Age )";
        public const string BasicAndFilter = "WHERE ( Name = @Name ) AND ( Age = @Age )";
        public const string BasicAndOrFilter = "WHERE ( Age = @Age  AND  Name = @Name ) OR ( Dollars = @Dollars )";
        public const string BasicOrAndFilter = "WHERE ( Age = @Age  OR  Name = @Name ) AND ( Dollars = @Dollars )";
        public const string BasicAndOrAndFilter = "WHERE ( Age = @Age  AND  Name = @Name ) OR ( IsMonster = @IsMonster  AND  Dollars = @Dollars )";
        public const string BasicOrAndOrFilter = "WHERE ( Age = @Age  OR  Name = @Name ) AND ( IsMonster = @IsMonster  OR  Dollars = @Dollars )";
        #endregion
    }
}
