namespace Ara
{
    public class Number : IExpr
    {
        public double Value { get; }

        public Number(double value)
        {
            Value = value;
        }

        public bool Equals(IExpr other) =>
            other is Number l && Value == l.Value;
    }
}