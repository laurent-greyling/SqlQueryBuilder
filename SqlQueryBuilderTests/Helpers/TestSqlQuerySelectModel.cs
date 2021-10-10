using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace SqlQueryBuilderTests.Helpers
{
    [ExcludeFromCodeCoverage]
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
