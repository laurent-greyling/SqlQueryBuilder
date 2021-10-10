# SqlQueryBuilder

An expression evaluator that checks your model and specified expression, then build a sql query with paramatized values. The builder will escape column and table names and paramatize the values as per the expression to create a `where` condition with values.

## SqlQueryBuilderService

### Insert

This method will build an insert query based on the model you pass it. It will evaluate what is the `table name`__*__. There after it will based on reflection grab what you columns is based on the `model properties` you have with populated values. From here it will grab the values based on your populated model and paramatirize the values with the properties for insertion.

The end result of the builder will evaluate to:

```

Code =>
_model = new Model
{
    Name = string value,
    Age = int value,
    Bday = Datetime value,
    IsMonster = bool value,
    Dollars = decimal valuen
}

var (query, parameters) = _sqlQueryBuilder.Insert(_model);

Result =>
query:

INSERT INTO [Test].[SqlQuerySelect]([Name],[Age],[Bday],[IsMonster],[Dollars]) VALUES(@Name,@Age,@Bday,@IsMonster,@Dollars)"

parameters:

Dictionairy<string, object>
{
    {@Name, string},
    {@Age, int},
    {@Bday, DateTime},
    {@IsMonster, bool},
    {@Dollars, decimal},    
}

```

__*__ See naming of models

### Select

The base select will create a select query based on the model you provide it and the condition you require.  It will evaluate what is the `table name`__*__. There after it will based on reflection grab what you columns is based on the `model properties` you have with populated values.

If you have no conditions it will return a basic select query with __no__ where filter:

```
SELECT [Name],[Age],[Bday],[IsMonster],[Dollars] FROM [Test].[SqlQuerySelect]

``` 

If however you supply a condion it will return the query with the condition you have set as a requirement, with the condition paramatarized:

```

Code =>
var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(x => x.Dollars == 500.95M);

Result =>
query:

SELECT [Name],[Age],[Bday],[IsMonster],[Dollars] FROM [Test].[SqlQuerySelect] WHERE  Dollars = @Dollars

parameters:

Dictionairy<string, object>
{
    {@Dollars, 500.95}
}
```

__*__ See naming of models

### Select overload

This overload is intended for when you have a table and column names you do not want to determine from the model. The model will still be required for any condition expressions you want to pass, but it will not determine the table or column names from this model.

```
Code =>

var (query, parameters) = _sqlQueryBuilder.Select<TestSqlQuerySelectModel>(_tableName, _neededColumns, x => x.Col1 == "Col1");

Result =>
query:

SELECT [Col1],[Col2],[Col3] FROM [Some].[Table] WHERE  Col1 = @Col1

```

__*__ See naming of models

## Naming of models
To have the table names correctly escaped or in a correct manner, you need to add `DisplayName(Schema.TableName)` from `using System.ComponentModel` to your model. Based on this naming convention the code checks the name and correctly escape the name. This naming also need to be followed when you pass in your own table name in the select overload.

If you do not pass this display name, the code will take your model name as the tablename and escape that as your table name in the select query. If you add a display name or pass the name and it doesn't follow convention, exception will be thrown.

```
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace SqlQueryBuilderTests.Helpers
{
    [DisplayName("Test.SqlQuerySelect")]
    public class TestSqlQuerySelectModel
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime Bday { get; set; }
        public bool IsMonster { get; set; }
        public decimal Dollars { get; set; }
    }
}
```

## Conclusion

There are many other scenarios that still need to be added and some evaluation paths that need to still be added. But for the main ones that is currently supported see the tests implementations.