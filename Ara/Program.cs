using System.Collections.Immutable;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Ara.Ast.Nodes;

var stream = CharStreams.fromString(@"
    void foo(int bar) {
      int x
      x = 1
      return x
    }
".Trim());
var lexer = new AraLexer(stream);
var tokens = new CommonTokenStream(lexer);
var parser = new AraParser(tokens);
parser.BuildParseTree = true;
var tree = parser.program();
var visitor = new ProgramBuilder();
var prog = visitor.Visit(tree);
Console.WriteLine(prog.ToString());

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

    public abstract record class Statement(Location Location) : AstNode(Location);
    public abstract record class Expression(Location Location) : AstNode(Location);

    public record class Name(string Value, Location Location) : AstNode(Location);
    public record class Atom(string Value, Location Location) : AstNode(Location);
    public record class Type(string Value, Location Location) : AstNode(Location);

    public record class Program(IImmutableList<Function> Functions, Location Location) : AstNode(Location);
    public record class Function(Name Name, Type Type, IImmutableList<Parameter> Parameters, Block Block, Location Location) : AstNode(Location);
    public record class Parameter(Name Name, Type Type, Location Location) : AstNode(Location);
    public record class Block(IImmutableList<Statement> Statements, Location Location) : AstNode(Location);

    public record class VariableInitializationStatement(Name Name, Expression Expression, Location Location) : Statement(Location);
    public record class VariableDeclarationStatement(Name Name, Type Type, Location Location) : Statement(Location);
    public record class ReturnStatement(Expression ReturnValue, Location Location) : Statement(Location);
    public record class IfStatement(Expression Condition, Block Block, Location Location) : Statement(Location);
    public record class AssignmentStatement(Name Name, Expression Expression, Location Location) : Statement(Location);

    public record class ParensExpression(Expression Expression, Location Location) : Expression(Location);
    public record class UnaryExpression(UnaryOp Op, Expression Right, Location Location) : Expression(Location);
    public record class InfixExpression(InfixOp Op, Expression Left, Expression Right, Location Location) : Expression(Location);
    public record class FunctionCallExpression(Name Name, IImmutableList<Expression> Arguments, Location Location) : Expression(Location);
    public record class AtomExpression(Atom Atom, Location Location) : Expression(Location);
}

class ProgramBuilder : AraBaseVisitor<Ara.Ast.Nodes.Program>
{
    public override Ara.Ast.Nodes.Program VisitProgram([NotNull] AraParser.ProgramContext context)
    {
        return NodeBuilder.Program(context);
    }
}

static class NodeBuilder
{
    private static readonly StatementBuilder statementBuilder = new();
    private static readonly ExpressionBuilder expressionBuilder = new();

    public static Ara.Ast.Nodes.Program Program(AraParser.ProgramContext context) =>
        new Ara.Ast.Nodes.Program(Functions(context._functions), Location(context));

    public static Block Block(AraParser.BlockContext context) =>
        new(Statements(context._statements), Location(context));

    public static IImmutableList<Function> Functions(IList<AraParser.FunctionContext> context) =>
        context.Select(x => new Function(Name(x.name()), Type(x.type()), Parameters(x._parameters), Block(x.block()), Location(x))).ToImmutableList();

    public static VariableInitializationStatement VariableInitializationStatement(AraParser.VariableInitializationStatementContext context) =>
        new(Name(context.name()), Expression(context.expression()), Location(context));

    public static VariableDeclarationStatement VariableDeclarationStatement(AraParser.VariableDeclarationStatementContext context) =>
        new(Name(context.name()), Type(context.type()), Location(context));

    public static ReturnStatement ReturnStatement(AraParser.ReturnStatementContext context) =>
        new(Expression(context.expression()), Location(context));

    public static IfStatement IfStatement(AraParser.IfStatementContext context) =>
        new(Expression(context.expression()), Block(context.block()), Location(context));

    public static AssignmentStatement AssignmentStatement(AraParser.AssignmentStatementContext context) =>
        new(Name(context.name()), expressionBuilder.Visit(context.expression()), Location(context));

    public static ParensExpression ParensExpression(AraParser.ParensExpressionContext context) =>
        new(Expression(context.expression()), Location(context));

    public static UnaryExpression UnaryExpression(AraParser.UnaryExpressionContext context) =>
        new((UnaryOp)Enum.Parse(typeof(UnaryOp), context.op.Text), Expression(context.expression()), Location(context));

    public static InfixExpression InfixExpression(AraParser.InfixExpressionContext context) =>
        new((InfixOp)Enum.Parse(typeof(InfixOp), context.op.Text), Expression(context.left), Expression(context.right), Location(context));

    public static FunctionCallExpression FunctionCallExpression(AraParser.FunctionCallExpressionContext context) =>
        new(Name(context.name()), Arguments(context._arguments), Location(context));

    public static AtomExpression AtomExpression(AraParser.AtomExpressionContext context) =>
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
        context.Select(x => new Parameter(Name(x.name()), Type(x.type()), Location(x))).ToImmutableList();

    private static IImmutableList<Statement> Statements(IList<AraParser.StatementContext> context) =>
        context.Select(x => statementBuilder.Visit(x)).ToImmutableList();

    private static Location Location(ParserRuleContext context) =>
        new(context.Start.Line, context.Start.Column);

    private static Expression Expression(AraParser.ExpressionContext context) =>
        expressionBuilder.Visit(context);
}

class StatementBuilder : AraBaseVisitor<Statement>
{
    public override Statement VisitVariableInitializationStatement([NotNull] AraParser.VariableInitializationStatementContext context) =>
        NodeBuilder.VariableInitializationStatement(context);

    public override Statement VisitVariableDeclarationStatement([NotNull] AraParser.VariableDeclarationStatementContext context) =>
        NodeBuilder.VariableDeclarationStatement(context);

    public override Statement VisitReturnStatement([NotNull] AraParser.ReturnStatementContext context) =>
        NodeBuilder.ReturnStatement(context);

    public override Statement VisitIfStatement([NotNull] AraParser.IfStatementContext context) =>
        NodeBuilder.IfStatement(context);

    public override Statement VisitAssignmentStatement([NotNull] AraParser.AssignmentStatementContext context) =>
        NodeBuilder.AssignmentStatement(context);
}

class ExpressionBuilder : AraBaseVisitor<Expression>
{
    public override Expression VisitParensExpression([NotNull] AraParser.ParensExpressionContext context) =>
        NodeBuilder.ParensExpression(context);

    public override Expression VisitUnaryExpression([NotNull] AraParser.UnaryExpressionContext context) =>
        NodeBuilder.UnaryExpression(context);

    public override Expression VisitInfixExpression([NotNull] AraParser.InfixExpressionContext context) =>
        NodeBuilder.InfixExpression(context);

    public override Expression VisitFunctionCallExpression([NotNull] AraParser.FunctionCallExpressionContext context) =>
        NodeBuilder.FunctionCallExpression(context);

    public override Expression VisitAtomExpression([NotNull] AraParser.AtomExpressionContext context) =>
        NodeBuilder.AtomExpression(context);
}
