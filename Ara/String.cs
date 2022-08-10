namespace Ara
{
    public class String : IExpr
    {
        public string Value { get; }

        public String(string value)
        {
            Value = value;
        }

        public bool Equals(IExpr other) =>
            other is String l && Value == l.Value;
    }
}