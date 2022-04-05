using System.Collections.Immutable;
using System.Text.Json;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Ara.Ast.Nodes;

var stream = CharStreams.fromString(@"
    void foo(int bar) {
      return 1
    }
".Trim());
var lexer = new AraLexer(stream);
var tokens = new CommonTokenStream(lexer);
var parser = new AraParser(tokens);
parser.BuildParseTree = true;
var tree = parser.program();
var visitor = new ProgramBuilder();
var prog = visitor.Visit(tree);
Console.WriteLine(
    JsonSerializer.Serialize(
        prog,
        new JsonSerializerOptions { WriteIndented = true, MaxDepth = 0 }));

namespace Ara.Ast.Nodes
{
    public readonly record struct Name(string Value);
    public readonly record struct Atom(string Value);
    public readonly record struct Type(string Value);
    public readonly record struct Program(IImmutableList<Function> Functions);
    public readonly record struct Function(Name Name, Type Type, IImmutableList<Parameter> Parameters, Block Block);
    public readonly record struct Parameter(Name Name, Type Type);
    public readonly record struct Block(IImmutableList<IStatement> Statements);

    public interface IStatement
    {
    }

    public readonly record struct VariableInitializationStatement(Name Name, IExpression Expression) : IStatement;
    public readonly record struct VariableDeclarationStatement(Name Name, Type Type) : IStatement;
    public readonly record struct ReturnStatement(IExpression ReturnValue) : IStatement;
    public readonly record struct IfStatement(IExpression Condition, Block Block) : IStatement;

    public interface IExpression : IStatement
    {
    }

    public enum UnaryOp
    {
    }

    public enum InfixOp
    {
    }

    public readonly record struct ParensExpression(IExpression Expression) : IExpression;
    public readonly record struct UnaryExpression(UnaryOp Op, IExpression Right) : IExpression;
    public readonly record struct InfixExpression(InfixOp Op, IExpression Left, IExpression Right) : IExpression;
    public readonly record struct FunctionCallExpression(Name Name, IImmutableList<IExpression> Arguments) : IExpression;
    public readonly record struct AtomExpression(Atom Atom) : IExpression;
}

class ProgramBuilder : AraBaseVisitor<Ara.Ast.Nodes.Program>
{
    public override Ara.Ast.Nodes.Program VisitProgram([NotNull] AraParser.ProgramContext context)
    {
        var functionBuilder = new FunctionBuilder();
        var functions = context.function()
            .Select(x => functionBuilder.Visit(x))
            .ToImmutableList();

        return new Ara.Ast.Nodes.Program(functions);
    }
}

class FunctionBuilder : AraBaseVisitor<Function>
{
    public override Function VisitFunction([NotNull] AraParser.FunctionContext context)
    {
        var id = context.name().value.Text;

        var parameters = context._parameters
            .Select(x => new Parameter(
                new Name(x.name().GetText()),
                new Ara.Ast.Nodes.Type(x.type().GetText())))
            .ToImmutableList();

        var block = new BlockBuilder().Visit(context.block());

        return new Function(
            new Name(context.name().GetText()),
            new Ara.Ast.Nodes.Type(context.type().GetText()),
            parameters,
            block);
    }
}

class BlockBuilder : AraBaseVisitor<Block>
{
    public override Block VisitBlock([NotNull] AraParser.BlockContext context)
    {
        var statementBuilder = new StatementBuilder();
        var statements = context._statements
            .Select(x => statementBuilder.Visit(x))
            .ToImmutableList();

        return new Block(statements);
    }
}

class StatementBuilder : AraBaseVisitor<IStatement>
{
    private readonly ExpressionBuilder expressionBuilder = new();
    private readonly BlockBuilder blockBuilder = new();

    public override IStatement VisitVariableInitializationStatement([NotNull] AraParser.VariableInitializationStatementContext context)
    {
        return new VariableInitializationStatement(
            new Name(context.name().GetText()),
            expressionBuilder.Visit(context.expression()));
    }

    public override IStatement VisitVariableDeclarationStatement([NotNull] AraParser.VariableDeclarationStatementContext context)
    {
        return new VariableDeclarationStatement(
            new Name(context.name().GetText()),
            new Ara.Ast.Nodes.Type(context.type().GetText()));
    }

    public override IStatement VisitReturnStatement([NotNull] AraParser.ReturnStatementContext context)
    {
        return new ReturnStatement(expressionBuilder.Visit(context.expression()));
    }

    public override IStatement VisitIfStatement([NotNull] AraParser.IfStatementContext context)
    {
        return new IfStatement(
            expressionBuilder.Visit(context.expression()),
            blockBuilder.Visit(context.block()));
    }
}

class ExpressionBuilder : AraBaseVisitor<IExpression>
{
    public override IExpression VisitParensExpression([NotNull] AraParser.ParensExpressionContext context)
    {
        return new ParensExpression(Visit(context.expression()));
    }

    public override IExpression VisitUnaryExpression([NotNull] AraParser.UnaryExpressionContext context)
    {
        return new UnaryExpression(
            (UnaryOp)Enum.Parse(typeof(UnaryOp), context.op.Text),
            Visit(context.expression()));
    }

    public override IExpression VisitInfixExpression([NotNull] AraParser.InfixExpressionContext context)
    {
        return new InfixExpression(
            (InfixOp)Enum.Parse(typeof(InfixOp), context.op.Text),
            Visit(context.left),
            Visit(context.right));
    }

    public override IExpression VisitFunctionCallExpression([NotNull] AraParser.FunctionCallExpressionContext context)
    {
        return new FunctionCallExpression(
            new Name(context.name().GetText()),
            context._arguments.Select(x => Visit(x)).ToImmutableList());
    }

    public override IExpression VisitAtomExpression([NotNull] AraParser.AtomExpressionContext context)
    {
        return new AtomExpression(new Atom(context.GetText()));
    }
}


