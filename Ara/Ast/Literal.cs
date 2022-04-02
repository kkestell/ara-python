using System.Text.Json;

namespace Ara.Ast
{
    public class Literal : IExpr
    {
        public JsonElement Value { get; }

        public Literal(object value)
        {
            Value = (JsonElement)value;
        }

        public bool Equals(IExpr? other) =>
            other is Literal literal && literal.Value.Equals(other);
    }
}