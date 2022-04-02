using System;

namespace Ara.Ast
{
    public class Field
    {
        public IExpr Name { get; }
        public IExpr Value { get; }

        public Field(IExpr name, IExpr value)
        {
            Name = name;
            Value = value;
        }
    }
}
