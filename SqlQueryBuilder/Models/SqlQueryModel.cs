namespace SqlQueryBuilder.Models
{
    public class SqlQueryModel
    {
        public string Key { get; set; }
        public string Operator { get; set; }

        public string Value { get; set; }

        public string ConditionOperator { get; set; }
    }
}
