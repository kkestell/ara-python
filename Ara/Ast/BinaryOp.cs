namespace Ara.Ast
{
    public class BinaryOp : IExpr
    {
        public BinaryOpType Type { get; }
        public IExpr Left { get; }
        public IExpr Right { get; }

        public BinaryOp(BinaryOpType type, IExpr left, IExpr right)
        {
            Type = type;
            Left = left;
            Right = right;
        }

        public bool Equals(IExpr other) =>
            other is BinaryOp b && Type == b.Type && Left.Equals(b.Left) && Right.Equals(b.Right);
    }
}