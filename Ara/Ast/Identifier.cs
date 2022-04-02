namespace Ara.Ast
{
    public class Identifier : IExpr
    {
        public string Name { get; }

        public Identifier(string name)
        {
            Name = name;
        }

        public bool Equals(IExpr other) =>
            other is Identifier i && Name == i.Name;
    }
}