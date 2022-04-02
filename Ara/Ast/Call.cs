using System.Collections.Immutable;
using System.Linq;

namespace Ara.Ast
{
    public class Call : IExpr
    {
        public IExpr Expr { get; }
        public ImmutableArray<IExpr> Arguments { get; }

        public Call(IExpr expr, ImmutableArray<IExpr> arguments)
        {
            Expr = expr;
            Arguments = arguments;
        }

        public bool Equals(IExpr other) =>
            other is Call c && Expr.Equals(c.Expr) && Arguments.SequenceEqual(c.Arguments);
    }
}