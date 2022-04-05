using System.Collections.Immutable;
using System.Text.Json;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Ara.Ast.Nodes;

var stream = CharStreams.fromString(@"
    void foo(int bar) {
      return
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
        new JsonSerializerOptions { WriteIndented = true }));

namespace Ara.Ast.Nodes
{
    public readonly record struct Program(IImmutableList<Function> Functions);

    public readonly record struct Function(string Name, IImmutableList<Parameter> Parameters);

    public readonly record struct Parameter(string Name, string Type);
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

        var args = context.parameter()
            .Select(x => new Parameter(x.name().value.Text, x.type().value.Text))
            .ToImmutableList();

        return new Function(id.ToString(), args);
    }
}
