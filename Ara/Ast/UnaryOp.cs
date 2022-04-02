namespace Ara.Ast
{
    public class UnaryOp : IExpr
    {
        public UnaryOpType Type { get; }
        public IExpr Expr { get; }

        public UnaryOp(UnaryOpType type, IExpr expr)
        {
            Type = type;
            Expr = expr;
        }

        public bool Equals(IExpr other) =>
            other is UnaryOp u && Type == u.Type && Expr.Equals(u.Expr);
    }
}