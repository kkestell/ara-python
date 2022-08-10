namespace Ara
{
    public class UnaryOp : IExpr
    {
        public UnaryOperatorType Type { get; }
        public IExpr Expr { get; }

        public UnaryOp(UnaryOperatorType type, IExpr expr)
        {
            Type = type;
            Expr = expr;
        }

        public bool Equals(IExpr other) =>
            other is UnaryOp u && Type == u.Type && Expr.Equals(u.Expr);
    }
}