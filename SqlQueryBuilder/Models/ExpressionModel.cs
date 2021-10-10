using System.Linq.Expressions;

namespace SqlQueryBuilder.Models
{
    public class ExpressionModel
    {
        public Expression Left { get; set; }
        public Expression Right { get; set; }
        public ExpressionType Node { get; set; }
    }
}
