using System.Collections.Immutable;
using Antlr4.Runtime;
using Ara.Ast.Nodes;

var stream = CharStreams.fromString(@"
    void foo(int bar) {
      int x
      x = 1
      return x
    }
".Trim());

var parser = new AraParser(new CommonTokenStream(new AraLexer(stream)));
parser.BuildParseTree = true;

var program = NodeBuilder.Program(parser.program());

return 0;

namespace Ara.Ast.Nodes
{
    public enum UnaryOp
    {
    }

    public enum InfixOp
    {
    }

    public readonly record struct Location(int Line, int Column);

    public abstract record class AstNode(Location Location);

    public abstract record class Statement(
        Location Location
    ) : AstNode(Location);

    public abstract record class Expression(
        Location Location
    ) : AstNode(Location);

    public record class Name(
        string Value,
        Location Location
    ) : AstNode(Location);

    public record class Atom(
        string Value,
        Location Location
    ) : AstNode(Location);

    public record class Type(
        string Value,
        Location Location
    ) : AstNode(Location);

    public record class Program(
        IImmutableList<Function> Functions,
        Location Location
    ) : AstNode(Location);

    public record class Function(
        Name Name,
        Type Type,
        IImmutableList<Parameter> Parameters,
        Block Block,
        Location Location
    ) : AstNode(Location);

    public record class Parameter(
        Name Name,
        Type Type,
        Location Location
    ) : AstNode(Location);

    public record class Block(
        IImmutableList<Statement> Statements,
        Location Location
    ) : AstNode(Location);

    public record class VariableInitialization(
        Name Name,
        Expression Expression,
        Location Location
    ) : Statement(Location);

    public record class VariableDeclaration(
        Name Name,
        Type Type,
        Location Location
    ) : Statement(Location);

    public record class Return(
        Expression ReturnValue,
        Location Location
    ) : Statement(Location);

    public record class If(
        Expression Condition,
        Block Block,
        Location Location
    ) : Statement(Location);

    public record class Assignment(
        Name Name,
        Expression Expression,
        Location Location
    ) : Statement(Location);

    public record class Parens(
        Expression Expression,
        Location Location
    ) : Expression(Location);

    public record class Unary(
        UnaryOp Op,
        Expression Right,
        Location Location
    ) : Expression(Location);

    public record class Infix(
        InfixOp Op,
        Expression Left,
        Expression Right,
        Location Location
    ) : Expression(Location);

    public record class FunctionCall(
        Name Name,
        IImmutableList<Expression> Arguments,
        Location Location
    ) : Expression(Location);

    public record class AtomExpr(
        Atom Atom,
        Location Location
    ) : Expression(Location);
}

static class NodeBuilder
{
    public static Ara.Ast.Nodes.Program Program(AraParser.ProgramContext context) =>
        new(Functions(context._functions), Location(context));

    public static Block Block(AraParser.BlockContext context) =>
        new(Statements(context._statements), Location(context));

    public static IImmutableList<Function> Functions(IList<AraParser.FunctionContext> context) =>
        context.Select(x =>
            new Function(
                Name(x.name()),
                Type(x.type()),
                Parameters(x._parameters),
                Block(x.block()),
                Location(x)))
            .ToImmutableList();

    public static VariableInitialization VariableInitialization(AraParser.VariableInitializationContext context) =>
        new(Name(context.name()),
            Expression(context.expression()),
            Location(context));

    public static VariableDeclaration VariableDeclaration(AraParser.VariableDeclarationContext context) =>
        new(Name(context.name()),
            Type(context.type()),
            Location(context));

    public static Return Return(AraParser.ReturnContext context) =>
        new(Expression(context.expression()),
            Location(context));

    public static If If(AraParser.IfContext context) =>
        new(Expression(context.expression()),
            Block(context.block()),
            Location(context));

    public static Assignment Assignment(AraParser.AssignmentContext context) =>
        new(Name(context.name()),
            Expression(context.expression()),
            Location(context));

    public static Parens Parens(AraParser.ParensContext context) =>
        new(Expression(context.expression()),
            Location(context));

    public static Unary Unary(AraParser.UnaryContext context) =>
        new((UnaryOp)Enum.Parse(typeof(UnaryOp), context.op.Text),
            Expression(context.expression()),
            Location(context));

    public static Infix Infix(AraParser.InfixContext context) =>
        new((InfixOp)Enum.Parse(typeof(InfixOp), context.op.Text),
            Expression(context.left),
            Expression(context.right),
            Location(context));

    public static FunctionCall FunctionCall(AraParser.FunctionCallContext context) =>
        new(Name(context.name()),
            Arguments(context._arguments),
            Location(context));

    public static AtomExpr AtomExpr(AraParser.AtomExprContext context) =>
        new(Atom(context.atom()), Location(context));

    private static Atom Atom(AraParser.AtomContext context) =>
        new(context.GetText(), Location(context));

    private static Name Name(AraParser.NameContext context) =>
        new(context.GetText(), Location(context));

    private static Ara.Ast.Nodes.Type Type(AraParser.TypeContext context) =>
        new(context.GetText(), Location(context));

    private static IImmutableList<Expression> Arguments(IList<AraParser.ExpressionContext> context) =>
        context.Select(x => Expression(x)).ToImmutableList();

    private static IImmutableList<Parameter> Parameters(IList<AraParser.ParameterContext> context) =>
        context.Select(x =>
            new Parameter(
                Name(x.name()),
                Type(x.type()),
                Location(x)))
            .ToImmutableList();

    private static IImmutableList<Statement> Statements(IList<AraParser.StatementContext> context) =>
        context.Select(x => Statement(x)).ToImmutableList();

    private static Location Location(ParserRuleContext context) =>
        new(context.Start.Line, context.Start.Column);

    private static Statement Statement(AraParser.StatementContext context) =>
        context switch
        {
            AraParser.VariableInitializationContext c =>
                VariableInitialization(c),
            AraParser.VariableDeclarationContext c =>
                VariableDeclaration(c),
            AraParser.ReturnContext c =>
                Return(c),
            AraParser.IfContext c =>
                If(c),
            AraParser.AssignmentContext c =>
                Assignment(c),
            _ => throw new NotImplementedException()
        };

    private static Expression Expression(AraParser.ExpressionContext context) =>
        context switch
        {
            AraParser.ParensContext c =>
                Parens(c),
            AraParser.UnaryContext c =>
                Unary(c),
            AraParser.InfixContext c =>
                Infix(c),
            AraParser.FunctionCallContext c =>
                FunctionCall(c),
            AraParser.AtomExprContext c =>
                AtomExpr(c),
            _ => throw new NotImplementedException()
        };
}